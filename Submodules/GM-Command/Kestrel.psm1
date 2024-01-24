function Write-ProjectKestrelConfigs {
	param(
		[String]$ProjectName,
		[String]$ProjectDomain,
		[int[]]$ProjectPorts,
		[String]$ProjectCertPath,
		[SecureString]$Password
	)
	END {
		$ProjectRoot = [System.IO.Path]::Combine($global:SolutionRoot, $ProjectName)
		Push-Location $ProjectRoot

		Write-Host "Setting Kestrel password for certificate"
		dotnet user-secrets set "Kestrel:Endpoints:HTTPS:Certificate:Password" "$($Password | ConvertFrom-SecureString -AsPlainText)"
		
		Write-ProjectKestrelAppSettings `
			-ProjectDomain $ProjectDomain -ProjectPorts $ProjectPorts `
			-ProjectRoot $ProjectRoot -ProjectCertPath $ProjectCertPath

		Write-ProjectLaunchSettings -ProjectDomain $ProjectDomain -ProjectRoot $ProjectRoot
		Pop-Location
	}
}

function Write-ProjectKestrelAppSettings {
	param(
		[String]$ProjectDomain,
		[int[]]$ProjectPorts,
		[String]$ProjectRoot,
		[String]$ProjectCertPath
	)

	$newSettingsJson = $(New-KestrelAppSettings -ProjectDomain $ProjectDomain `
		-ProjectCertPath $ProjectCertPath -ProjectPorts $ProjectPorts
	)
	$targetSettingsPath = [System.IO.Path]::Combine($ProjectRoot, "appsettings.Kestrel.json")
	Write-ProjectSettings -NewSettingsJson $newSettingsJson -TargetSettingsPath $targetSettingsPath -ProjectRoot $ProjectRoot
}

function New-KestrelAppSettings {
	param(
		[String]$ProjectDomain,
		[String]$ProjectCertPath,
		[int[]]$ProjectPorts
	)

	$json = "
		{
			'ForwardedHeaders': 'All',
			'AllowedHosts': '$ProjectDomain',
			'Kestrel': {
				'Endpoints': {
					'HTTP': {
						'Url': 'http://$ProjectDomain`:$($ProjectPorts[0])'
					},
					'HTTPS': {
						'Url': 'https://$ProjectDomain`:$($ProjectPorts[1])',
						'Certificate': {
							'Path': '$ProjectCertPath'
						}
					}
				}
			}
		}".Replace("`n`t`t", "`n").Replace("'", '"').Replace('\', '/')
	return $json
}


function Write-ProjectLaunchSettings {
	param(
		[String]$ProjectDomain,
		[String]$ProjectRoot
	)	
	$newSettingsJson = $(New-LaunchSettings -ProjectDomain $ProjectDomain)
	$targetSettingsPath = [System.IO.Path]::Combine($ProjectRoot, "Properties", "launchSettings.json")
	Write-ProjectSettings -NewSettingsJson $newSettingsJson -TargetSettingsPath $targetSettingsPath -ProjectRoot $ProjectRoot
}

function New-LaunchSettings {
	param(
		[String]$ProjectDomain
	)

	$json = "
		{
			'iisSettings': {
				'windowsAuthentication': false,
				'anonymousAuthentication': true,
				'iisExpress': {
					'applicationUrl': 'https://$ProjectDomain',
					'sslPort': 443
				}
			},
			'profiles': {
				'Debug': {
					'commandName': 'Project',
					'launchUrl': 'https://$ProjectDomain',
					'environmentVariables': {
						'ASPNETCORE_ENVIRONMENT': 'Development'
					},
					'applicationUrl': 'https://$ProjectDomain'
				}
			}
		}".Replace("`n`t`t", "`n").Replace("'", '"')
	return $json
}

function Write-ProjectSettings {
	param(
		[String]$NewSettingsJson,
		[String]$TargetSettingsPath,
		[String]$ProjectRoot
	)

	$newSettings = $($NewSettingsJson | Convert-JsonToHashtable)
	if(Test-Path $TargetSettingsPath) {
		$targetSettings = $(Read-JsonConfig $TargetSettingsPath)
		$targetSettingsUpdate = $(Merge-Objects -Original $targetSettings -Update $newSettings)
	} else {
		$targetSettingsUpdate = $newSettings
	}
	$targetSettingsUpdate | Write-JsonConfig $TargetSettingsPath
}