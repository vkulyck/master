. "$PSScriptRoot\init.ps1"

$script:wsl_host = "jwsl.web.home"
$script:wsl_user = "jake"

$script_name = "ConfigureRedisTls.ps1"
$linux_script_directory = "~/code/scripts/redis"
$wsl_script_directory = "$(Convert-WslToUnc $linux_script_directory)"
$wsl_script_path = "$wsl_script_directory\$script_name"
$linux_script_path = "$linux_script_directory/$script_name"
New-Item -ItemType Directory -Path $wsl_script_directory -ErrorAction SilentlyContinue
Copy-Item "$PSScriptRoot/$script_name" $wsl_script_path

$session = New-PSSession -HostName $script:wsl_host -UserName $script:wsl_user
Invoke-Command -Session $session -Scriptblock {
  /bin/pwsh $using:linux_script_path
}
Remove-PSSession $session