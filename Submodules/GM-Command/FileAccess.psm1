function Grant-IISDataAccess {
    param(
        [Parameter(Mandatory=$true)]
        [String]$SqlServer,
        [Parameter(Mandatory=$true)]
        [Alias('Catalog')]
        [String[]]$Catalogs,
        [Parameter(Mandatory=$true)]
        [Alias('AppPool')]
        [String[]]$AppPools,
        [Alias('Role')]
        [String[]]$Roles = @("db_datareader", "db_datawriter", "db_ddladmin")
    )

    foreach($pool in $AppPools) { # eg [IIS APPPOOL\.NET v4.5]
        if(-Not $pool.StartsWith('IISAPPPOOL\')) {
            $pool = "IISAPPPOOL\$pool"
        }
        foreach($role in $roles) { # eg db_datareader
            foreach($catalog in $Catalogs) {
                $query = @"
                    USE [$catalog]
                    GO
                    ALTER ROLE [$role] ADD MEMBER [$pool]
                    GO
"@
                return Invoke-SqlQuery -Query $query -Server $SqlServer
            }
        }
    }
}

function Get-IISAppPools {
    #Import-Module WebAdministration
    Get-ItemProperty IIS:\AppPools\* | Foreach-Object { "IIS APPPOOL\$($_.name)" }
}

function Grant-SQLFileAccess {
    param(
        [Parameter(Mandatory)]
        [String] $Server,
        [Parameter(Mandatory)]
        [String[]] $Paths
    )
    $Query = "SELECT ServiceName, Service_Account AS ServiceAccount FROM sys.dm_server_services"
    $Accounts = $(Invoke-SqlQuery -Query $Query -Server $Server)
    $Accounts = $($Accounts | Where-Object { 
        $valid = $_.ServiceName -match '^SQL +Server +(Agent +)?\((MS)?SQL(20[0-2][0-9]|SERVER(AGENT)?)\)$'
        return $valid
    })
    $Accounts = $Accounts.ServiceAccount
    $AccessRules = "Read, Write, ExecuteFile, ListDirectory"
    Write-Host "Granting $AccessRules to paths: $Paths"
    Grant-FileAccess -Accounts $accounts -AccessRules $AccessRules -Paths $Paths
}

function Grant-FileAccess {
    param(
        [Parameter(Mandatory)]
        [String[]] $Accounts,
        [Parameter(Mandatory)]
        [String[]] $Paths,
        [Parameter(Mandatory)]
        [String[]] $AccessRules
    )
    foreach($path in $Paths) {
        foreach($acct in $Accounts) {
            # https://win32.io/posts/How-To-Set-Perms-With-Powershell
            $InheritSettings = "Containerinherit, ObjectInherit" #Controls how permissions are inherited by children
            $PropogationSettings = "None" #Usually set to none but can setup rules that only apply to children.
            $RuleType = "Allow" #Allow or Deny.
            $acl = Get-Acl $path
            $perm = $acct, $AccessRules, $InheritSettings, $PropogationSettings, $RuleType
            $rule = New-Object -TypeName System.Security.AccessControl.FileSystemAccessRule -ArgumentList $perm
            $acl.SetAccessRule($rule)
            $acl | Set-Acl -Path $path
            $Acl = Get-Acl $path
        }
    }
}

Export-ModuleMember Grant-IISDataAccess, Get-IISAppPools, Grant-SQLFileAccess