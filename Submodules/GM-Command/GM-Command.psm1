
if(-Not $global:EnvironmentInitialized) {
	. "$PSScriptRoot/Environment.ps1"
	Initialize-Globals
	Initialize-Directories
	Initialize-ScriptEnvironment
	$global:EnvironmentInitialized = $true
}

$script:ExcludedModules = switch($true) {
	{ $IsLinux } { @(
		'WSL';
	)}
	{ $IsWindows } { @(
		'EC2';
	)}
	{ $true } { @(
		'GM-Command'
	)}
}

foreach($module in $(Get-ChildItem "$PSScriptRoot/*.psm1")) {
	if($script:ExcludedModules -contains $module.BaseName) {
		continue
	}
	Import-Module "$($module.FullName)" -Force
}

Export-ModuleMember -Function *
