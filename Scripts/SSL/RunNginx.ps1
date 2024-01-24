#Requires -RunAsAdministrator

using namespace System.Management.Automation
using namespace System.ComponentModel
using namespace System.Collections.Specialized
[CmdletBinding(SupportsShouldProcess)]
param(
	[Parameter(HelpMessage="Enable this switch to run the master Nginx process as a background job.")]
	[Alias('m', 'Master')]
	[Switch]$StartMaster = $false,
	[Parameter(HelpMessage="Enable this switch to stop the master Nginx process.")]
	[Alias('x', 'Exit')]
	[Switch]$ExitMaster = $false,
	[Parameter(HelpMessage="Enable this switch to reload configs in the master Nginx process.")]
	[Alias('r', 'Reload')]
	[Switch]$ReloadConfigs = $false,
	[Parameter(HelpMessage="Enable this switch to reload configs in the master Nginx process.")]
	[Alias('c', 'Clear')]
	[Switch]$ClearLogs = $false
)
[PSObject].Assembly.GetType('System.Management.Automation.TypeAccelerators')::Add('Process', 'System.ComponentModel.Component')

#region Handlers
function Find-NginxProcessIds {
	[OutputType([OrderedDictionary])]
	param(
		[Parameter(ValueFromPipeline,Position=0)]
		[OrderedDictionary]$ProcIds = $null
	)
	if($null -eq $ProcIds) {
		$ProcIds = [ordered]@{}
	}
	foreach($procId in $(Get-Process nginx -ErrorAction Ignore | Foreach-Object { $_.Id })) {
		if(-Not $ProcIds.Contains("$procId")) {
			$ProcIds.Add("$procId", $procId)
		}
	}
	$ProcIds
}
function HandleProcessCulling {
	[CmdletBinding(SupportsShouldProcess=$true, ConfirmImpact = 'High')]

	$procIds = $(Find-NginxProcessIds)
	$confirmWarning = "Expected 2 active Nginx processes but found $($procIds.Count). " `
		+ "This may be due to startup errors resulting in mismatched certificates and/or browser 'Privacy Errors'.`n" `
		+ "Do you want to terminate these processes and then restart the master Nginx instance?" 
	$confirmDescription = "`n`t$(($procIds.Keys | ForEach-Object { "taskkill /F /PID $_" }) -Join "`n`t")"
	$killed = @()
	if ($PSCmdlet.ShouldProcess($confirmDescription, $confirmWarning, "Terminating Processes", [ref]($reason = ([ShouldProcessReason]::None)))) {
		$status = "Terminated 0 of $($procIds.Count) processes (0%)."
		for($i = 0; $i -lt $procIds.Count; $i++) {
			$procId = $procIds[$i]
			$progressArgs = @{
				CurrentOperation="Terminating process $($procId): 'nginx.exe'";
				Activity="Terminate conflicting Nginx processes.";
				PercentComplete=100 * $i / $procIds.Count;
			}
			Write-Progress @progressArgs -Status $status
			$command = "taskkill /F /PID $($procId)"
			Invoke-Expression $command | Out-Null
			$procIds = $($procIds | Find-NginxProcessIds)
			$result = Get-Process -Id $procId -ErrorAction Ignore
			if($null -eq $result) {
				$killed += "$procId"
			}
			$status = "Terminated $($killed.Length) of $($procIds.Count) Nginx processes"
			if($killed.Length -gt 0) {
				$status += ": $($killed -Join ', ')"
			}
		}
		Write-Progress @progressArgs -Status $status -PercentComplete (100 * $i / $procIds.Count)
		Start-Sleep -Milliseconds 2500
		$final = Find-NginxProcessIds
		if($final.Count -eq 0) {
			Write-Host "SUCCESS: $status"
		} else {
			throw "FAILED: $($final.Count) remaining Nginx processes in conflict."
		}		
	}
}

function HandleExitMaster {
	param([Job[]]$Jobs)
	if($Jobs.Length -eq 0) {
		Write-Host "Attempted to exit master process, but no active master process exists."
		exit
	}
	nginx -p "$global:NginxInstallRoot" -c "$global:NginxConfigRoot/main.conf" -s quit
	Write-Host "Quit the current Nginx master process:"
	Get-Job $script:JobName
}

function HandleClearLogs {
	param([Job[]]$Jobs)
	if($Jobs.Length -eq 0) {
		Write-Host "Attempted to clear Nginx logs, but no active master process exists to perform the action."
		exit
	}
	nginx -p "$global:NginxInstallRoot" -c "$global:NginxConfigRoot/main.conf" -s reopen
	Write-Host "Cleared Nginx logs from $global:NginxLogRoot:"
	Get-ChildItem "$global:NginxLogRoot"
}

function HandleStartMaster {
	param([Job[]]$Jobs)
	if($Jobs.Length -gt 0) {
		Write-Host "Attempted to start master process, but another active master process already exists."
		exit
	}
	Remove-Job -Name $script:JobName -ErrorAction SilentlyContinue
	Remove-Item "$global:NginxLogRoot\*.log"
	$results = $(& nginx -t -p "$global:NginxInstallRoot" -c "$global:NginxConfigRoot\main.conf" 2>&1)
	# [String[]]$results = $(& nginx -t -p "$global:NginxInstallRoot" -c "$global:NginxConfigRoot\main.conf" -g "echo 'hi';" 2>&1)
	$successPattern = "^nginx: configuration file .* test is successful$"
	if($results.Length -ne 2 -Or $results[1] -notmatch $successPattern) {
		$results| Write-Host -ForegroundColor Red
		throw "Error starting Nginx master process."
	}
	$job = Start-Job -Name $script:JobName -ScriptBlock { 
		& "$using:NginxInstallRoot\nginx.exe" -p "$using:NginxInstallRoot" -c "$using:NginxConfigRoot\main.conf"
	}
	Start-Sleep -Milliseconds 500
	Receive-Job -Job $job
	if($job.ChildJobs.Count -gt 0 -And $job.ChildJobs[0].Error.Count -eq 0) {
		Write-Host "Started a new Nginx master process:"
		Get-Job -Id $job.Id
	} else {
		throw "Error starting Nginx master process."
	}
}

function HandleReloadConfigs {
	param([Job[]]$Jobs)
	if($Jobs.Length -eq 0) {
		Write-Host "Attempted to reload Nginx configs in the master process, but no active master process exists."
		exit
	}
	nginx -p "$global:NginxInstallRoot" -c "$global:NginxConfigRoot/main.conf" -s reload
	Write-Host "Reloaded configs for the current master process:"
	Get-Job $script:JobName
}

#endregion

#region Request Routing

enum NginxRequest {
	CullExcessProcesses
	ExitMaster
	ClearLogs
	StartMaster
	ReloadConfigs
	CompleteScript
}

function Send-NginxActionRequest {
	[OutputType([NginxRequest])]
	$processes = $(Get-Process nginx -ErrorAction Ignore)
	$jobs = $(Get-Job -Name $script:JobName -ErrorAction SilentlyContinue | Where-Object { $_.State -eq "Running" })
	if($null -ne $processes -And $processes.Length -gt 2) {
		[NginxRequest]::CullExcessProcesses
	} elseif($ExitMaster) {
		[NginxRequest]::ExitMaster
	} elseif($ClearLogs) {
		[NginxRequest]::ClearLogs
	} elseif($StartMaster -Or $Jobs.Length -eq 0) {
		[NginxRequest]::StartMaster
	} else {
		[NginxRequest]::ReloadConfigs
	}
}

function Receive-NginxActionRequest {
	param(
		[Parameter(Mandatory=$true,Position=0,ValueFromPipeline=$true)]
		[NginxRequest]$Request
	)
	$jobs = $(Get-Job -Name $script:JobName -ErrorAction SilentlyContinue | Where-Object { $_.State -eq "Running" })
	switch($Request) {
		([NginxRequest]::CullExcessProcesses) {
			HandleProcessCulling
			HandleStartMaster -Jobs $jobs
		} 
		([NginxRequest]::ExitMaster) {
			HandleExitMaster -Jobs $jobs
		}
		([NginxRequest]::ClearLogs) {
			HandleClearLogs -Jobs $jobs
		}
		([NginxRequest]::StartMaster) {
			HandleStartMaster -Jobs $jobs
		}
		([NginxRequest]::ReloadConfigs) {
			HandleReloadConfigs -Jobs $jobs
		}
	}
}

#endregion

. "$PSScriptRoot/../Init.ps1"
Assert-AdminAccess
Set-Alias -Name "nginx" -Value "$global:NginxInstallRoot\nginx.exe"
$script:JobName = "nginx-gmweb"

Send-NginxActionRequest -Jobs $jobs -Processes $processes | Receive-NginxActionRequest
