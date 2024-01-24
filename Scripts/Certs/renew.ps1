# Before running this script you need to renew the certificate. This should happen automatically,
# but if it doesn't, just run wacs --renew. See included instructions.txt

Set-Location $PSScriptRoot

$CertUser = 'NT Service\MSSQLSERVER'
$CertPath = 'Cert:\LocalMachine\MY'
$CertThumbprint = $(Get-ChildItem  -Path $CertPath `
	| Where-Object {$_.Subject -Match "d0i0.staging.goodmojo.io"} `
	| Where-Object {$_.Issuer -notmatch 'STAGING'} `
	| Select-Object Thumbprint `
).Thumbprint

# For Permissions
./AddCertificatePrivateKeyPermissions.ps1 -UserName $CertUser -CertStorePath $CertPath -CertThumbprint $CertThumbprint

# For Remote Desktop
# This should work but it doesn't because we still can't find Set-WmiInstance
# Import-Module Microsoft.Powershell.Management
# $PATH = (Get-WmiObject -class "Win32_TSGeneralSetting" -Namespace root\cimv2\terminalservices)
# Set-WmiInstance -Path $PATH -argument @{SSLCertificateSHA1Hash=$CertThumbprint} -userName $CertUser -permission read -certStoreLocation $CertPath -certThumbprint $CertThumbprint

# This is the workaround
# wmic /namespace:\\root\cimv2\TerminalServices PATH Win32_TSGeneralSetting Set SSLCertificateSHA1Hash="$CertThumbprint"

Pop-Location