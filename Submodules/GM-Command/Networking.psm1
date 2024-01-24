function Update-HostsEntries {
	param(
		[Parameter(HelpMessage="A hash map from IP addresses to their associated domain groups. Each entry " `
                + "represents one line in the Hosts file, consisting of a single IP address and one or " `
                + "more domains.")]
		[Hashtable]$RemoteDomains = @{},
		[Parameter(HelpMessage=	"A list of (groups of) domains that should be mapped locally (i.e. to " `
                + "127.0.0.1). If the value supplied is an array of arrays, each sub-array represents " `
                + "one line in the Hosts file, consisting of the local IP address and one or more " `
							  + "domains. Otherwise, the entire list represents one line in the hosts file."
		)]
		[Array]$LocalDomains = @(),
		[Parameter(HelpMessage=	"The IP address to which local domains are assigned; typically 127.0.0.1.")]
		[String]$LocalIP = '127.0.0.1',
		[String]$HostsSectionName = $null
	)
	# Normalize $LocalDomains
	if($LocalDomains.Count -gt 0) {
		if($LocalDomains[0] -isnot [Array]) {
			$LocalDomains = @(,$LocalDomains)
		}
	}
	# Normalize $RemoteDomains
	foreach($ip in $($RemoteDomains.Keys)) {
		if($RemoteDomains[$ip] -isnot [Array]) {
			$RemoteDomains[$ip] = @(,$RemoteDomains[$ip])
		}
		if($RemoteDomains[$ip].Count -gt 0) {
			if($RemoteDomains[$ip][0] -isnot [Array]) {
				$RemoteDomains[$ip] = @(,$RemoteDomains[$ip])
			}
		}
	}
	Push-Location -Path $global:EntryScriptRoot
	if([string]::IsNullOrWhiteSpace($HostsSectionName)) {
		$HostsSectionName = $(git config --get remote.origin.url) -replace '^(https?://[^/]+/|.*?:)',''
	}
  if($IsLinux) {
    $hostsPath = "/etc/hosts"
  } elseif($IsWindows) {
	  $hostsPath = "$Env:SystemRoot/System32/drivers/etc/hosts"
  } elseif($IsMacOS) {
    throw "This function is only configured for Windows and Linux systems."
  } else {
    throw "The operating system could not be identified."
  }
	$content = Get-Content -Raw -Path $hostsPath
	$startTag = "### START AUTOCONFIG: $HostsSectionName ###"
	$endTag = "### END AUTOCONFIG: $HostsSectionName   ###"
	$regexOptions = '(?ms)' # m = Multi-line: ^/$ match the start/end of each line; 
							# s = Single-line: . matches all characters, including `n (newline)
	$tagPattern = $regexOptions + [Regex]::Escape($startTag) + '(.*?)' + [Regex]::Escape($endTag)
	$entries = "$startTag`r`n"
	$entries += "### Last Updated: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')            ###`r`n"
	foreach($domainGroup in $LocalDomains) {
		$ip = $LocalIP
		if($RemoteDomains.Keys -notcontains $ip) {
			$RemoteDomains[$ip] = @()
		}
		$RemoteDomains[$ip] += @(,$domainGroup)
	}
	foreach($ip in $RemoteDomains.Keys) {
		foreach($domainGroup in $RemoteDomains[$ip]) {
			$entry = "$ip $($domainGroup -join ' ')"
			$entries += "$entry`r`n"
		}
	}
	$entries += $endTag
	if($content -match $tagPattern) {
		$content = $content -replace $tagPattern,$entries
		$content = $content -replace '\s+$',''
	} else {
		$content += "`r`n$entries"
	}
	$content | Set-Content -Path $hostsPath
}

Export-ModuleMember -Function *