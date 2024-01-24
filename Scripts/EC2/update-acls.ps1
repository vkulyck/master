#!/usr/bin/env pwsh
param(
  [Parameter(Mandatory=$true)]
  [String] $CidrBlock,
  [Parameter(Mandatory=$true)]
  [int] $RuleNumber,
  [Parameter(Mandatory=$false)]
  [String] $RuleAction = 'allow',
  [Parameter(Mandatory=$false)]
  [Nullable[int]] $PortFrom = $null,
  [Parameter(Mandatory=$false)]
  [Nullable[int]] $PortTo = $null,
  [Parameter(Mandatory=$false)]
  [String] $Protocol = [NullString]::Value,
  [Parameter(Mandatory=$false)]
  [Switch] $DryRun = $false,
  [Parameter(Mandatory=$false)]
  [Switch] $AllPorts = $false,
  [Parameter(Mandatory=$false)]
  [String] $IcmpType = [NullString]::Value
)

function Set-Variables {
  $script:nacl_id = 'acl-07048fa0bd23d5054'
  $script:default_protocol = 6
  $script:port_from = 0
  $script:port_to = 1000
  $script:aws_profile = 'gm-ip-whitelist-agent'
}

function Convert-IcmpType {
  param(
    [Parameter(Mandatory=$false)]
    [String] $IcmpType = [NullString]::Value
  )
  if($IcmpType -eq 'Echo Request' -or $IcmpType -eq 'Echo') {
    return "Type=8,Code=0"
  }
  if([String]::IsNullOrWhitespace($IcmpType)) {
    return "Type=8,Code=0"
  }
  return "Type=1,Code=0"
}

function Convert-Protocol {
  param(
    [Parameter(Mandatory=$false)]
    [String] $Protocol = [NullString]::Value
  )
  if($Protocol -eq 'ICMP') {
    return 1
  }
  if($Protocol -eq 'TCP') {
    return 6
  }
  if($Protocol -eq 'UDP') {
    return 17
  }
  if($Protocol -eq 'All') {
    return -1
  }
  return $script:default_protocol
}

function New-CidrAcl {
  if($null -eq $PortFrom) {
    $PortFrom = $script:port_from
  }
  if($null -eq $PortTo) {
    $PortTo = $script:port_to
  }

  $icmp_args = ''
  if($Protocol -eq 'ICMP') {
    $IcmpTypeCode = Convert-IcmpType -IcmpType $IcmpType
    $icmp_args = "--icmp-type-code $IcmpTypeCode"
  }

  $Protocol = Convert-Protocol -Protocol $Protocol

  $port_args = ''
  if(-not $AllPorts -and ($Protocol -eq 6 -or $Protocol -eq 17)) {
    $port_args = "--port-range 'From=$PortFrom,To=$PortTo'"
  }

  $dry_run_args = $DryRun ? '--dry-run' : ''

  $Command = "aws ec2 create-network-acl-entry "`
    + "--network-acl-id $script:nacl_id "`
    + "--ingress --protocol $Protocol "`
    + "$port_args $icmp_args $dry_run_args "`
    + "--cidr-block $CidrBlock "`
    + "--rule-action $RuleAction "`
    + "--rule-number $RuleNumber "`
    + "--profile $script:aws_profile"
  if($DryRun) {
    $Command
  }
  Invoke-Expression -Command $Command
}

Set-Variables
New-CidrAcl
