. "$PSScriptRoot/ConfigureSSL.ps1"

$Params = @{
  Projects = @('GmWeb.Web.Api'; 'GmWeb.Web.Identity'; 'GmWeb.Web.RHI';);
  Subdomains = @('api'; 'identity'; 'rhi';);
  Password =  $(New-CertificatePassword);
}

Enable-SSL @Params -Domains "goodmojo.test","*.goodmojo.test"

Enable-SSL @Params -Domains "lnx.goodmojo.test","*.lnx.goodmojo.test" -Outputs Certificates
