using namespace System.Security.Cryptography.X509Certificates
using namespace System.Security.Principal
if($IsWindows) {
	Set-Alias -Name "openssl" -Value "C:\Program Files\Git\usr\bin\openssl.exe"
} elseif($IsLinux) {
	Set-Alias -Name "openssl" -Value "/bin/openssl"
}


function New-RootWebCertificate {
	param(
		[Alias("d", "domain")]
		[String[]]$Domains
	)
	$cert = New-SelfSignedCertificate -DnsName $Domains -CertStoreLocation "Cert:\LocalMachine\My"

	$store = New-Object X509Store -argumentlist "Root", LocalMachine
	$store.Open([OpenFlags]"ReadWrite")
	$store.Add($cert)

	return $cert
}

function Remove-RootWebCertificate{
	param(
		[Parameter(Mandatory=$false)]
		[Alias("d", "Domain")]
		[String[]]$Domains = @(),

		[Parameter(Mandatory=$false)]
		[Alias("t")]
		[String]$Thumbprint = $null,

		[Parameter(Mandatory=$false)]
		[String]$CertStoreLocation = 'Cert:LocalMachine\**'
	)
	$certs = $(Select-WebCertificates -Domains $Domains -Thumbprint $Thumbprint $CertStoreLocation)
	foreach($cert in $certs) {
		Write-Host "Removing certificate: $($cert.PSPath)"
		Remove-Item $cert.PSPath
	}
	
}

function Select-WebCertificates {
	param(
		[Parameter(Mandatory=$false)]
		[Alias("d", "Domain")]
		[String[]]$Domains = @(),

		[Parameter(Mandatory=$false)]
		[Alias("t")]
		[String]$Thumbprint = $null,

		[Parameter(Mandatory=$false)]
		[String]$CertStoreLocation = 'Cert:LocalMachine\**'
	)

	if([String]::IsNullOrWhiteSpace($Thumbprint) -And ($Domains.Length -eq 0)) {
		throw "A thumbprint or domain list must be provided."
	}
	if([String]::IsNullOrWhiteSpace($Thumbprint)) {
		return Get-ChildItem "$CertStoreLocation\*" | Where-Object { $_.Subject -match "CN=$($Domains[0])" }
	}
	else {
		return Get-ChildItem "Cert:LocalMachine\**\$Thumbprint" 
	}
}

function Select-WebCertificatePaths {
	param(
		[Alias("d", "Domain")]
		[String[]]$Domains = @(),

		[Alias("t")]
		[String]$Thumbprint = $null
	)
	return Select-WebCertificates -Domains $Domains -Thumbprint $Thumbprint | Foreach-OBject { $_.PSPath }
}

enum ConfirmResult { 
	Yes = 1;
	Y = 1;
	No = 2;
	N = 2;
	All = 3;
	A = 3;
}
function Confirm-SingleCertOverwrite {
	param(
		$Certificate
	)

	function ul {
		param (
			[Parameter(ValueFromPipeline=$true)]
			$text
		)
		Process {
			Write-Output "`e[4m$text`e[24m"
		}
	}
	function ulFirst {
		param (
			[Parameter(ValueFromPipeline=$true)]
			$text
		)
		Process { 
			$head = $(ul $text[0])
			$tail = $text.Substring(1)
			Write-Output "$head$tail"
		}
	}

	$options = $(@("yes", "no", "all") | ulFirst)
	$query = "Certificate at $($Certificate.PSPath) would be overwritten; continue? [$options]"
	while($true) {
		try {
			$response = Read-Host $query
			$result = [ConfirmResult]$response
			return $result
		}
		catch { continue; }
	}
}

function Confirm-AllCertOverwrites {
	param(
		[Parameter(Mandatory=$true)]
		[Alias("d", "Domain")]
		[String[]]$Domains
	)
	$certs = $(Select-WebCertificates -Domains $Domains)
	if($certs.Length -eq 0) {
		return
	}
	foreach($cert in $certs) {
		$result = $(Confirm-SingleCertOverwrite -Certificate $cert)
		if($result -eq [ConfirmResult]::Yes) {
			continue;
		}
		if($result -eq [ConfirmResult]::No) {
			$msg = $cert -Join "`n"
			Write-Host "Certificate creation cancelled because it would overwrite existing certificate:`n $msg"
			exit;
		}
		if($result -eq [ConfirmResult]::All) {
			break;
		}
	}
}
function Export-ProtectedCertificate {
	param(
		[SecureString]$Password = $null,
		[Parameter(Mandatory=$true)]
		[String]$StorePath,
		[String]$OutputPath
	)
	Write-Host Exporting certificate to resolved output path: $OutputPath
	Export-PfxCertificate -Cert $StorePath -FilePath $OutputPath -Password $Password
}

function New-CertificateEncryptionKey {
	param(
		[Alias('len')]
		[int]$Length = 32,
		[Alias('lc')]
		[Switch]$UseLowerCase = $false,
		[Alias('uc')]
		[Switch]$UseUpperCase = $false,
		[Alias('num','d')]
		[Switch]$UseNumericDigits = $false,
		[Alias('sym', 'sp')]
		[Switch]$UseSpecialSymbols = $false
	)
	END {
		$charSet = @()
		$any = $UseLowerCase -Or $UseUpperCase -Or $UseNumericDigits -Or $UseSpecialSymbols
		if(-Not $any) {
			Write-Host "You must specify at least one character set."
			exit
		}
		if($UseLowerCase) {
			$charSet += (97..122)
		}
		if($UseUpperCase) {
			$charSet += (65..90)
		}
		if($UseNumericDigits) {
			$charSet +=  (48..57)
		}
		if($UseSpecialSymbols) {
			$charSet += (33..47) + (58..64) + (91..96) + (123..126)
		}
		$(-join ($charSet | Get-Random -Count $Length | % {[char]$_}))
	}
}

function New-CertificatePassword {
	return New-CertificateEncryptionKey -len 32 -UseNumericDigits -UseUpperCase | ConvertTo-SecureString -AsPlainText
}

function New-SolutionCertificate {
	param(
		[Switch]$ConfirmOverwrite = $false,
		[Parameter(Mandatory=$true)]
		[String[]]$Domains,
		[Parameter(Mandatory=$true)]
		[String]$OutputPath,
		[Parameter(Mandatory=$true)]
		[SecureString]$Password
	)

	# Validate overwrites
	if($ConfirmOverwrite) {
		Confirm-AllCertOverwrites -Domains $Domains
	}

	# Clean up old files
	Remove-RootWebCertificate -Domains $Domains
	Remove-Item $OutputPath -ErrorAction SilentlyContinue
	
	$cert = $(New-RootWebCertificate -Domains $Domains)
	Export-ProtectedCertificate -StorePath $cert.PSPath -OutputPath $OutputPath -Password $Password | Out-Null
	return @($cert;$Password;)
}

function New-CrtCertificate {
	param(
		[Parameter(Mandatory=$true)]
		[String]$ProjectDomain,
		[Parameter(Mandatory=$true)]
		[String]$ProjectCertPath,
		[Parameter(Mandatory=$true)]
		[String]$CrtCertPath,
		[Parameter(Mandatory=$true)]
		[SecureString]$Password
	)
	Write-Host "Generating a CRT certificate for $ProjectDomain"
	Remove-Item $CrtCertPath -ErrorAction SilentlyContinue
	openssl pkcs12 -in "$ProjectCertPath" -clcerts -nokeys -out "$CrtCertPath" -password pass:$($Password | ConvertFrom-SecureString -AsPlainText)
	return $CrtCertPath
}

function New-RsaCertificate {
	param(
		[Parameter(Mandatory=$true)]
		[String]$ProjectDomain,
		[Parameter(Mandatory=$true)]
		[String]$ProjectCertPath,
		[Parameter(Mandatory=$true)]
		[String]$RsaCertPath,
		[Parameter(Mandatory=$true)]
		[SecureString]$Password
	)
	Write-Host "Generating an RSA certificate for $ProjectDomain"
	Remove-Item $RsaCertPath -ErrorAction SilentlyContinue
	openssl pkcs12 -in "$ProjectCertPath" -nocerts -nodes -out "$RsaCertPath" -password pass:$($Password | ConvertFrom-SecureString -AsPlainText)
	return $RsaCertPath
}

Export-ModuleMember -Function *
