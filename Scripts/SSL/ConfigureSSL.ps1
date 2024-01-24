using namespace System.IO
. "$PSScriptRoot\..\init.ps1"
Assert-AdminAccess

[Flags()]
enum SslComponents {
	None = 0
	Certificates = 1
	Kestrel = 2
	Nginx = 4
	Hosts = 8
	NoHosts = 7
	All = 15
}

function Enable-SSL {
	param(
		[Parameter(Mandatory=$true, HelpMessage="The domain, or list of domains, that will comprise the Subject and list of DNS Names in the forthcoming certificate.")]
		[Alias("d", "Domain")]
		[String[]]$Domains,
		[Parameter(HelpMessage="Enable this flag to request user confirmation before overwriting previously created certificates.")]
		[Alias("c", "Confirm")]
		[Switch]$ConfirmOverwrite = $false,
		[Parameter(Mandatory=$true, HelpMessage="The project(s) within the specified solution that will use this certificate.")]
		[Alias("p", "Project")]
		[String[]]$Projects,
		[Parameter(Mandatory=$true, HelpMessage="The project(s) within the specified solution that will use this certificate.")]
		[Alias("s", "Subdomain")]
		[String[]]$Subdomains,
		[Parameter(Mandatory=$false, HelpMessage="An optional password to use for this certificate; useful when synchronizing multiple configurations.")]
		[SecureString]$Password,
		[Parameter(HelpMessage="The port on which the project will be hosted for HTTP web access.")]
		[int]$HttpPort = 5000,
		[Parameter(HelpMessage="The port on which the project will be hosted for HTTPS web access.")]
		[int]$HttpsPort = 5001,
		[Parameter(Mandatory=$false, HelpMessage="An explicit selection of output files that will be generated during the configuration process.")]
		[SslComponents]$Outputs = [SslComponents]::All
	)
	if($Projects.length -gt $Subdomains.length) {
		Write-Host "You must specify exactly one unique subdomain in the Subdomains parameter for each project defined in the Projects parameter."
		exit
	}

	$ProjectDomains = $($Subdomains | Foreach-Object { "$($_).$($Domains[0])" })
	if($Outputs.HasFlag([SslComponents]::Hosts)) {
		Write-Host "Generating Hosts entries for $($Domains[0])"
		Update-HostsEntries -LocalDomains $ProjectDomains -HostsSectionName $Domains[0]
	}

	# Validate projects
	if($null -eq $Password) {
		$Password =  $(New-CertificatePassword)
	}

	$SolutionCertPath = "$global:CertRoot/$($Domains[0]).pfx".Replace('\', '/')
	if($Outputs.HasFlag([SslComponents]::Certificates)) {
		Write-Host "Generating a primary certificate for $($Domains[0])"
		$SolutionCert = $(New-SolutionCertificate -Domains $Domains -OutputPath $SolutionCertPath -Password $Password)
	}

	# Configure solution with new certificate
	if($Outputs.HasFlag([SslComponents]::Nginx)) {
		New-MainNginxConfig -Projects $Projects
	}

	# Configure each project with new certificate
	for($i=0; $i -lt $Projects.length; $i++) {
		$ProjectName = $Projects[$i]
		$ProjectPorts = @($HttpPort + ($i * 10); $HttpsPort + ($i * 10););
		$ProjectDomain = $ProjectDomains[$i]
		
		$ProjectCertPath = "$global:CertRoot/$($ProjectDomain).pfx";
		$CrtCertPath = "$global:CertRoot/$($ProjectDomain).crt" -Replace '\\','/';
		$RsaCertPath = "$global:CertRoot/$($ProjectDomain).rsa" -Replace '\\','/';

		if($Outputs.HasFlag([SslComponents]::Certificates)) {
			Write-Host "Generating certificates for $ProjectDomain"
			Copy-Item $SolutionCertPath $ProjectCertPath
			New-CrtCertificate -ProjectDomain $ProjectDomain -ProjectCertPath $ProjectCertPath -CrtCertPath $CrtCertPath -Password $Password
			New-RsaCertificate -ProjectDomain $ProjectDomain -ProjectCertPath $ProjectCertPath -RsaCertPath $RsaCertPath -Password $Password
		}

		if($Outputs.HasFlag([SslComponents]::Kestrel)) {
			Write-Host "Generating Kestrel configs for $ProjectName"
			Write-ProjectKestrelConfigs -ProjectName $ProjectName `
				-ProjectDomain $ProjectDomain -ProjectPorts  $ProjectPorts `
				-ProjectCertPath $ProjectCertPath -Password $Password
		}

		if($Outputs.HasFlag([SslComponents]::Nginx)) {
			Write-Host "Generating Nginx configs for $ProjectName"
			Write-ProjectNginxConfig -ProjectName $ProjectName `
				-ProjectDomain $ProjectDomain -ProjectPorts $ProjectPorts `
				-CrtCertPath $CrtCertPath -RsaCertPath $RsaCertPath
		}
	}

	Write-Host New certificate details:
	Write-Host Password: $($Password | ConvertFrom-SecureString -AsPlainText)
	Write-Host $SolutionCert

	Write-Host The new certificate was successfully installed to the following machine locations:
	Select-WebCertificatePaths -Domains $Domains

}