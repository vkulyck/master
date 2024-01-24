$global:CertRoot = [System.IO.Path]::Combine($global:DataRoot, "Certificates")
$global:NginxConfigRoot = [System.IO.Path]::Combine($global:DataRoot, "Nginx")
$global:NginxLogRoot = [System.IO.Path]::Combine($global:LogRoot, "Nginx")
$global:NginxInstallRoot = 	[System.IO.Path]::Combine($Env:ProgramW6432, "Nginx")

Initialize-Directories @(
	$global:CertRoot;
	$global:NginxConfigRoot;
	$global:NginxLogRoot;
)
