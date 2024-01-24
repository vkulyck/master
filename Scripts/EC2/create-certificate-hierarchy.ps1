#!/usr/bin/env pwsh

Set-Alias certbot $(Get-Command certbot).Source
enum CertbotAuthMethod { None; Apache; Nginx; Standalone; Webroot; Route53; }

$env:AWS_PROFILE = 'gm-web-agent'
$script:EnableDryRun = $false
$script:Email = 'jake@goodmojo.us'
$script:DomainArgs = @{
  Roots = @('goodmojo.io';)
  Envs = @('team';)
  Apps = @('identity';)
}
$script:CertAuth = [CertbotAuthMethod]::Route53


function Get-CommonArgs {
  param(
      [Parameter(Mandatory=$true)]
      [String] $Email,
      [Parameter(Mandatory=$false)]
      [Switch] $EnableDryRun = $false,
      [Parameter(Mandatory=$false)]
      [CertbotAuthMethod] $Auth = [CertbotAuthMethod]::Nginx
  )
  $common_args = @(
      '--agree-tos';
      '--non-interactive';
  );

  if($EnableDryRun) {
    $common_args += @( '--dry-run'; );
  }
  if([CertbotAuthMethod]::None -ne $Auth) {
    if([CertbotAuthMethod]::Route53 -eq $Auth) {
      $common_args += '--dns-route53';
    } else {
      $common_args += @( "--$($Auth.ToString().ToLower())"; );
    }
  }

  $common_args += @( "--email"; $Email; )
  return $common_args
}

function Get-CertDomainArgs {
  param(
      [Parameter(Mandatory=$true)]
      [String[]] $Roots,
      [Parameter(Mandatory=$true)]
      [String[]] $Envs,
      [Parameter(Mandatory=$true)]
      [String[]] $Apps,
      [Parameter(Mandatory=$false)]
      [String[]] $AppIDs = @()
  )
  $domains = @()
  foreach($root in $Roots) {
    foreach($env in $Envs) {
      $env_domain = "$env.$root"
      $domains += $env_domain
      if('*' -eq $env) {
        continue;
      }
      foreach($app in $Apps) {
        $app_domain = "$app.$env_domain"
        $domains += $app_domain
        if('*' -eq $app) {
          continue;
        }
        foreach($id in $AppIDs) {
          $id_domain = "$id.$app_domain"
          $domains += $id_domain
        }
      }
    }
  }
  $domain_args = @($domains | % { @( '-d'; $_; ) } )
  return $domain_args
}

$cmd_args = @()
$cmd_args += $(Get-CommonArgs `
  -Email $script:Email `
  -Auth $script:CertAuth `
  -EnableDryRun:$script:EnableDryRun
)
$cmd_args += $(Get-CertDomainArgs @script:DomainArgs)

$command = "certbot certonly $($cmd_args -Join ' ')"

Write-Host "Invoking:`n`t$command"
Invoke-Expression $command