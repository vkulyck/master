# For use on Windows version prior to 1803
Set-Alias wsl $(Get-Command wsl.exe).Source
function Convert-WslToUnc {
  param(
    [Parameter(ValueFromPipeline=$true,Mandatory=$true)]
    [String] $LinuxPath
  )
  process {
    $unc_path = $(wsl wslpath -w "$LinuxPath")
    return $unc_path
  }
}

function Convert-WinToWsl {
  param(
    [Parameter(Mandatory,ValueFromPipeline)]
    [String] $WindowsPath
  )
  process {
    "$(wsl wslpath -ua ($WindowsPath -replace '\\','/'))"
  }
}

function Get-WslHostIP {
  param(
    [Parameter(Mandatory=$false)]
    [Alias("Prefix")]
    [string]$FirstOctet = "172"
  )
  $ifconfig_result = (wsl ifconfig)
  $initial_grep_result = ($ifconfig_result | wsl grep -oP "'inet\s+[\.\d]+\s+netmask'")
  $final_grep_result = ($initial_grep_result | wsl grep -oP "'$FirstOctet[\.\d]+'")
  return $final_grep_result
}
function Test-WslHostsMapping {
  param(
    [String] $WslHost = [NullString]::Value
  )
  if([String]::IsNullOrWhiteSpace($WslHost)) {
    $WslHost = $global:SubsystemHost
  }
  try {
    $dns_ip = [System.Net.Dns]::GetHostAddresses($WslHost).IPAddressToString
  $cli_ip = Get-WslHostIP
  $is_dns_valid = ($cli_ip -eq $dns_ip)
  return $is_dns_valid
}
  catch {
    return $false
  }
}

function Update-WslHostsMapping {
  param(
    [Alias('WslHost')]
    [Alias('WslDomains')]
    [String[]] $WslHosts = @(),
    [String] $HostsSectionName = [NullString]::Value
  )
  if($WslHosts.Length -eq 0) {
    $WslHosts = @($global:SubsystemHost;)
  }
  if([String]::IsNullOrWhiteSpace($HostsSectionName)) {
    $HostsSectionName = "Local WSL Instance for SSH"
  }
  $wsl_host_ip = Get-WslHostIP
  $UpdateHostsArgs = @{
    RemoteDomains = @{
      "$(Get-WslHostIP)" = $WslHosts
    };
    HostsSectionName = $HostsSectionName
  }
  Update-HostsEntries @UpdateHostsArgs
}

function Test-WslSshStatus {
  $wsl_ssh_status = $(wsl service ssh status)
  if($wsl_ssh_status -match "sshd is running") {
    return $true
  } elseif($wsl_ssh_status -match "sshd is not running") {
    return $false
  } else {
    throw "Unknown SSH status on WSL instance: '$wsl_ssh_status'"
  }
}

function Start-WslSsh {
  (wsl sudo service ssh start)
}

Export-ModuleMember *
