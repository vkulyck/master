#!/usr/bin/env pwsh
Set-Alias openssl '/usr/bin/openssl'
Set-Alias cat '/bin/cat'

function Get-CertSubject {
  param(
    [Parameter(Mandatory=$true)]
    [String] $CommonName
  )
  $details = [ordered]@{
    'C' = 'US';
    'ST' = 'CA';
    'L' = 'San Francisco';
    'O' = 'Goodmojo Corp.';
    'OU' = 'Dev Dept.';
    'CN' = $CommonName
  }
  $subject = "/$($($details.Keys | % { "$_=$($details[$_])"}) -Join '/')"
  return $subject
}

function Write-CertExtConfig {
  param(
    [Parameter(Mandatory=$true)]
    [String] $MachineName,
    [Parameter(Mandatory=$true)]
    [String] $ConfigPath
  )
  $content = @"
authorityKeyIdentifier=keyid,issuer
basicConstraints=CA:FALSE
keyUsage = digitalSignature, nonRepudiation, keyEncipherment, dataEncipherment
extendedKeyUsage = critical, serverAuth, clientAuth
subjectAltName = @alt_names

[alt_names]
DNS.1 = $MachineName
DNS.2 = $MachineName.web.home
"@
  $content | Set-Content -Path $ConfigPath
}

function New-MachineKeyPair {
  param(
    [Parameter(Mandatory=$true)]
    [String] $MachineName,
    [Parameter(Mandatory=$true)]
    [String] $MachineKeypass,
    [Parameter(Mandatory=$true)]
    [String] $CaKeypass,
    [Parameter(Mandatory=$true)]
    [String] $CaKeyfile,
    [Parameter(Mandatory=$true)]
    [String] $CaCertfile
  )
  $MachineHost = "$MachineName.web.home"
  openssl genrsa -passout pass:$MachineKeypass -aes128 -out "$MachineHost.key" 2048
  openssl req -new -passin pass:$MachineKeypass -subj $(Get-CertSubject -CommonName $MachineHost)  -key "$MachineHost.key" -out "$MachineHost.csr"
  Write-CertExtConfig -MachineName $MachineName -ConfigPath "$MachineHost.ext"
  openssl x509 -req -in "$MachineHost.csr" `
    -CA $CaCertfile -CAkey $CaKeyfile `
    -CAcreateserial -out "$MachineHost.crt" `
    -days 825 -passin pass:$CaKeypass -sha256 -extfile "$MachineHost.ext"
  cat "$MachineHost.crt" "$MachineHost.key" > "$MachineHost.pem"
}

sudo rm -r ~/certs
New-Item -Path "$env:HOME/certs" -ItemType Directory -ErrorAction SilentlyContinue
Push-Location "$env:HOME/certs"

$script:ca_name = "goodmojo"
$script:server_name = "jwsl"
$script:client_name = "jorn"
$pass = $("$(openssl rand -base64 24)" -replace '[\/\+]','_')

openssl genrsa -passout pass:$pass -des3 -out "$script:ca_name.ca.key" 2048
openssl req -x509 -new -passin pass:$pass -subj $(Get-CertSubject -CommonName $script:ca_name) -nodes -key "$script:ca_name.ca.key" -sha256 -days 1825 -out "$script:ca_name.ca.crt"
cat "$script:ca_name.ca.crt" "$script:ca_name.ca.key" > "$script:ca_name.ca.pem"

New-MachineKeyPair -MachineName $script:server_name -MachineKeypass $pass -CaKeypass $pass -CaKeyfile "$script:ca_name.ca.key" -CaCertfile "$script:ca_name.ca.crt"
New-MachineKeyPair -MachineName $script:client_name -MachineKeypass $pass -CaKeypass $pass -CaKeyfile "$script:ca_name.ca.key" -CaCertfile "$script:ca_name.ca.crt"

@"
port 0
tls-port 6379
tls-cert-file $PWD/$script:server_name.web.home.crt
tls-key-file $PWD/$script:server_name.web.home.key
tls-ca-cert-file $PWD/$script:ca_name.ca.crt
tls-key-file-pass $pass
"@ | Set-Content "tls.conf"
chmod 644 "$script:server_name.web.home.crt" "$script:ca_name.ca.crt" "tls.conf"
chmod 440 "$script:server_name.web.home.key" "$script:ca_name.ca.key"
sudo chown redis:redis "$script:server_name.web.home.crt" "$script:ca_name.ca.crt" "tls.conf" "$script:server_name.web.home.key" "$script:ca_name.ca.key"

Pop-Location