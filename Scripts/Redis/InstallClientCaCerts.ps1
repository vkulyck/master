#Requires -RunAsAdministrator

. "$PSScriptRoot\init.ps1"

function Install-WslCert {
  param(
    [Parameter(Mandatory=$true)]
    [String] $CommonName,
    [Parameter(Mandatory=$false)]
    [Switch] $IsCA = $false,
    [Parameter(Mandatory=$true)]
    [String] $CertRoot
  )
  $CertFilename = "$CommonName.pem"
  if($IsCA) {
    $cert_store_loc = 'Cert:\LocalMachine\Root'
    $CertFilename = "$CommonName.ca.pem"
  } else {
    $cert_store_loc = 'Cert:\LocalMachine\CA'
  }
  
  $cert_path = "$(Convert-WslToUnc $CertRoot)/$CertFilename"
  Remove-RootWebCertificate -Domains @($CommonName;) -CertStoreLocation $cert_store_loc
  Import-Certificate -FilePath "$cert_path" -CertStoreLocation $cert_store_loc
}

Install-WslCert -CommonName 'goodmojo' -IsCA -CertRoot '~/certs'
Install-WslCert -CommonName 'jorn.web.home' -CertRoot '~/certs'
Install-WslCert -CommonName 'jwsl.web.home' -CertRoot '~/certs'