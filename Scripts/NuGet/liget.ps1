. "$PSScriptRoot/../init.ps1"

# TODO: If NuGet is not installed at ${Env:ProgramFiles(x86)}\NuGet\nuget.exe, then download it from
# https://dist.nuget.org/win-x86-commandline/latest/nuget.exe
# then save it to the aforementioned location and add it to the PATH.
$creds = Read-JsonConfig "$PSScriptRoot/creds.json"
$source = "goodmojo"
$endpoint = "https://liget.internal.goodmojo.io/api/v3/index.json"
$nuget_config_path = "$Env:APPDATA\NuGet\nuget.Config"

nuget sources Remove -Name $source -NonInteractive -ConfigFile "$nuget_config_path"
nuget sources Add -Name $source -Source $endpoint -ConfigFile "$nuget_config_path"
nuget sources update -Name $source -UserName $creds.username -Password $creds.password -ConfigFile "$nuget_config_path"
nuget setapikey $creds.api_key -Source $source -ConfigFile "$nuget_config_path"