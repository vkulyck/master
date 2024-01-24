param (
    [string]$CertStorePath  = "Cert:\LocalMachine\My",
    [string]$UserName,
    [string]$CertThumbprint
)

#Import-Module WebAdministration

$certificate = Get-ChildItem $CertStorePath | Where thumbprint -eq $CertThumbprint

if ($certificate -eq $null)
{
    $message="Certificate with thumbprint:"+$CertThumbprint+" does not exist at "+$CertStorePath
    Write-Host $message -ForegroundColor Red
    exit 1;
}else
{
    $rsaCert = [System.Security.Cryptography.X509Certificates.RSACertificateExtensions]::GetRSAPrivateKey($certificate)
    $fileName = $rsaCert.key.UniqueName
    $file =  (Get-ChildItem -Path 'C:\ProgramData\Microsoft\Crypto' -Recurse -Include "*$fileName*" | Select -First 1)
    $path = $file.FullName
    $permissions = Get-Acl -Path $path
    $access_rule = New-Object System.Security.AccessControl.FileSystemAccessRule("$UserName", 'Read', 'None', 'None', 'Allow')
    $permissions.AddAccessRule($access_rule)
    Set-Acl -Path $path -AclObject $permissions
}