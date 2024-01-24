#!/usr/bin/env pwsh


Set-Alias certbot $(Get-Command certbot).Source

function Get-MetadataToken {
  curl -s -X PUT "http://169.254.169.254/latest/api/token" -H "X-aws-ec2-metadata-token-ttl-seconds: 21600"
}

function Read-MetadataItem {
  param(
    [Parameter(Mandatory=$true)]
    [String] $Token,
    [Parameter(Mandatory=$true)]
    [String] $ItemId
  )
  curl -s -H "X-aws-ec2-metadata-token: $Token" http://169.254.169.254/latest/meta-data/$ItemId
}

function Get-MetadataItem {
  param(
    [Parameter(Mandatory=$true)]
    [String] $Token,
    [Parameter(Mandatory=$true)]
    [String] $ItemId,
    [Parameter(Mandatory=$true)]
    [Hashtable] $Data
  )
  switch($itemId) {
    inst_id { Read-MetadataItem -Token $Token -ItemId instance-id }
    region { Read-MetadataItem -Token $Token -ItemId placement/region }
    hzone_id { $global:HOSTED_ZONE_ID }
    hzone_domain { $(aws route53 get-hosted-zone --id $global:HOSTED_ZONE_ID | ConvertFrom-Json -AsHashtable).HostedZone.Name }
    role { Read-MetadataItem -Token $Token -ItemId iam/security-credentials }
    avzone { Read-MetadataItem -Token $Token -ItemId placement/availability-zone }
    public_ip { Read-MetadataItem -Token $Token -ItemId public-ipv4 }
    inst_name {
      if([String]::IsNullOrWhiteSpace($Data.region)) {
        Set-MetadataItem -Token $Token -ItemId region -Data $Data
      }
      if([String]::IsNullOrWhiteSpace($Data.inst_id)) {
        Set-MetadataItem -Token $Token -ItemId inst_id -Data $Data
      }
      $(aws ec2 describe-tags --region $Data.region --filters "Name=resource-id,Values=$($Data.inst_id)" "Name=key,Values=Name" --output text | cut -f5)
    }
    gm_env {
      if([String]::IsNullOrWhiteSpace($Data.region)) {
        Set-MetadataItem -Token $Token -ItemId region -Data $Data
      }
      if([String]::IsNullOrWhiteSpace($Data.inst_id)) {
        Set-MetadataItem -Token $Token -ItemId inst_id -Data $Data
      }
      $(aws ec2 describe-tags --region $Data.region --filters "Name=resource-id,Values=$($Data.inst_id)" "Name=key,Values=GM_SERVER_ENV" --output text | cut -f5)
    }
    elb_name {
      if([String]::IsNullOrWhiteSpace($Data.gm_env)) {
        Set-MetadataItem -Token $Token -ItemId gm_env -Data $Data
      }
      $(aws elbv2 describe-load-balancers --names "gm-$($Data.gm_env)-lb" --query 'LoadBalancers[].LoadBalancerArn' --output text | cut -d ':' -f 6 | cut -d '/' -f 2-4)
    }
    elb_ip {
      if([String]::IsNullOrWhiteSpace($Data.elb_name)) {
        Set-MetadataItem -Token $Token -ItemId elb_name -Data $Data
      }
      $(aws ec2 describe-network-interfaces --filters Name=description,Values="ELB $($Data.elb_name)" --query 'NetworkInterfaces[*].PrivateIpAddresses[*].PrivateIpAddress' --output text)
    }
    hostname { (Get-Content /etc/hostname).Trim() }
  }
}

function Set-MetadataItem {
  param(
    [Parameter(Mandatory=$true)]
    [String] $Token,
    [Parameter(Mandatory=$true)]
    [String] $ItemId,
    [Parameter(Mandatory=$true)]
    [Hashtable] $Data
  )
  $Data[$ItemId] = Get-MetadataItem -Token $Token -ItemId $ItemId -Data $Data
}

function Get-Metadata {
  param(
    [Parameter(Mandatory=$true)]
    [String] $Token,
    [Parameter(Mandatory=$true)]
    [Alias('ItemId')]
    [String[]] $ItemIds
  )
  $data = @{}
  foreach($itemId in $ItemIds) {
    Set-MetadataItem -Token $Token -ItemId $itemId -Data $data
  }
  return $data
}

function Get-RoleCredentials {
  param(
    [Parameter(Mandatory=$true)]
    [String] $Token,
    [Parameter(Mandatory=$true)]
    [String] $Role
  )
  Read-MetadataItem -Token $Token -ItemId iam/security-credentials/$Role | ConvertFrom-Json -AsHashtable
}

function Set-AwsCredentials {
  param(
    [Parameter(Mandatory,ValueFromPipeline,Position=0)]
    [Object[]] $Credentials,
    [Parameter(Mandatory)]
    [String] $Role,
    [Parameter(Mandatory)]
    [String] $Region
  )
  $Env:AWS_ACCESS_KEY_ID = $Credentials.AccessKeyId
  $Env:AWS_SECRET_ACCESS_KEY = $Credentials.SecretAccessKey
  $Env:AWS_SESSION_TOKEN = $Credentials.Token
  $Env:AWS_ROLE_SESSION_NAME = $Role
  $Env:AWS_DEFAULT_REGION = $Region
}

function Update-DnsHostRecords {
  param(
    [Parameter(Mandatory=$true)]
    [String] $Hostname,
    [Parameter(Mandatory=$true)]
    [String] $IpAddress,
    [Parameter(Mandatory=$true)]
    [String] $HostedZoneId,
    [Switch] $Echo = $false
  )
  $changes = ConvertTo-Json -Depth 100 -InputObject @{
    Changes = @([ordered]@{
      Action = 'UPSERT';
      ResourceRecordSet = [ordered]@{
        Name = "$Hostname";
        Type = 'A';
        TTL = 300;
        ResourceRecords = @([ordered]@{
          Value = "$IpAddress";
        };);
      };
    };);
  }

  $parts = @(
    'aws';
    'route53';
    'change-resource-record-sets';
    "--hosted-zone-id $HostedZoneId";
    "--change-batch '$($changes -replace "`n *"," " -replace '"','\"' -Join " ")'";
  );
  $command = $parts -Join ' '
  if($Echo) {
    Write-Output $command
  }
  Invoke-Expression $command
}

function Update-InstanceDns {
  $token = $(Get-MetadataToken)
  $data = $(Get-Metadata -Token $token -ItemIds role,region,hostname,gm_env,public_ip,hzone_id,hzone_domain)
  write-output $data
  $creds = $(Get-RoleCredentials -Token $token -Role $data.role)
  Set-AwsCredentials -Credentials $creds -Role $data.role -Region $data.region
  Write-Output "Upserting DNS mapping in hosted zone $($data.hzone_id):"
  Write-Output "`t$($data.hostname).$($data.gm_env).$($data.hzone_domain) => $($data.public_ip)"
  Write-Output "Response:"
  Update-DnsHostRecords `
    -Hostname "$($data.hostname).$($data.gm_env).$($data.hzone_domain)" `
    -IpAddress $data.public_ip `
    -HostedZoneId $data.hzone_id
}

function Get-ElbIp {
  $token = $(Get-MetadataToken)
  $data = $(Get-Metadata -Token $token -ItemIds elb_ip)
  $data.elb_ip
}

enum ServiceId {
  api
  identity
  rhi
  nginx
}
$global:ServiceIds = @([ServiceId].GetEnumNames())
$global:ReverseServiceIds = $global:ServiceIds[$($global:ServiceIds.Length-1)..0]

function Get-InstanceCertificates {
  $token = $(Get-MetadataToken)
  $data = $(Get-Metadata -Token $token -ItemIds hzone_domain)
  $Env:AWS_PROFILE = 'gm-web-agent'
  foreach($svcId in $script:ServiceIds) {
    $parts = @(
      'certbot';
      'certonly';
      '--agree-tos --non-interactive --dns-route53';
      "--email $script:DNS_EMAIL";
      "-d $svcId.$($data.gm_env).$($data.hzone_domain)"
    )
    $command = $parts -Join ' '
    Invoke-Expression $command
  }
}

enum ServiceAction {
  Start
  Stop
  Restart
  Status
}

function Invoke-SystemdAction {
  param(
    [Parameter(Mandatory=$true)]
    [Alias('Action')]
    [Alias('Actions')]
    [ServiceAction] $ServiceActions,
    [Parameter(Mandatory=$true)]
    [Alias('ServiceId')]
    [Alias('Ids')]
    [ServiceId[]] $ServiceIds
  )
  foreach($svcAction in $ServiceActions) {
    foreach($svcId in $ServiceIds) {
      $svcName = $svcId.ToString()
      if($svcId -ne [ServiceId]::nginx) {
        $svcName = "gmweb-$svcName"
        if(-not $(Test-Path "/etc/systemd/system/$svcName.service")) {
          continue
        }
      }
      $command = "service $svcName $($svcAction.ToString().ToLower())"
      Write-Host $command
      Invoke-Expression $command
    }
  }
}

Export-ModuleMember *