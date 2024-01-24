using module ./Utility.psm1

$script:ENABLE_VERBOSE_RESTORE_OUTPUT = $false
$script:creds = $global:SqlServerCredentials

function Test-HasCatalog($Server, $Catalog) {
    $Query = @"
        IF DB_ID(N'$Catalog') IS NULL
            SELECT 0 AS [Exists]
        ELSE
            SELECT 1 AS [Exists]

"@
    $exists = Invoke-SqlQuery -Query $Query -Server $Server
    $exists = -Not -Not $exists.Exists
    return $exists
}

function Disconnect-Users($Server, $Catalog) {
    $Query = @"
        USE [master];
        GO

        ALTER DATABASE [$Catalog] SET SINGLE_USER WITH ROLLBACK IMMEDIATE
        ALTER DATABASE [$Catalog] SET MULTI_USER
"@
    return Invoke-SqlQuery -Server $Server -Query $Query
}

function Set-UserLogin($User, $Login, $Server, $Catalog) {

    $Query = @"
        USE [$Catalog];
        GO
        ALTER USER $User WITH LOGIN = $Login;
        GO
"@
    Invoke-SqlQuery -Query $Query -Server $Server -Catalog $Catalog
}

function Read-CatalogMetadata($Server, $Catalog) {
    $Query = @"
        DECLARE @DbID INT
        SELECT @DbID = database_id FROM sys.databases WHERE NAME = '$Catalog'

        SELECT
            (SELECT TOP 1 [physical_name] FROM sys.master_files WHERE database_id = @DbID AND TYPE_DESC = 'ROWS') AS DataPath,
            (SELECT TOP 1 [physical_name] FROM sys.master_files WHERE database_id = @DbID AND TYPE_DESC = 'LOG') AS LogPath,
            (SELECT TOP 1 [physical_name] FROM sys.master_files WHERE database_id = @DbID AND TYPE_DESC = 'FILESTREAM') AS FsPath
"@
    Invoke-SqlQuery -Query $Query -Server $Server -Catalog $Catalog
}

function Read-BackupMetadata($Backup, $Server) {
    $declare_result_table = Get-RestoreResultTableDeclaration "table"
    $Query = @"
        USE [master]
        GO
        $declare_result_table
        INSERT INTO @table
        EXEC('RESTORE FILELISTONLY FROM DISK = ''$Backup''')

        SELECT
            (SELECT TOP 1 LogicalName FROM @Table WHERE Type='D') AS DataFilename,
            (SELECT TOP 1 LogicalName FROM @Table WHERE Type='L') AS LogFilename,
            (SELECT TOP 1 LogicalName FROM @Table WHERE Type='S') AS FsFilename
"@


    Invoke-SqlQuery -Query $Query -Server $Server
}

function Read-ServerMetadata($Server) {
    $Query = @"
        DECLARE @IDDP VARCHAR(MAX), @IDLP VARCHAR(MAX)
        SELECT
            @IDDP = CONVERT(VARCHAR(MAX),SERVERPROPERTY('InstanceDefaultDataPath')),
            @IDLP = CONVERT(VARCHAR(MAX),SERVERPROPERTY('InstanceDefaultLogPath'))

        -- Remove the trailing slash
        IF RIGHT(RTRIM(@IDDP),1) = '\' OR RIGHT(RTRIM(@IDDP),1) = '/'
            SELECT @IDDP = SUBSTRING(@IDDP, 1, LEN(@IDDP) - 1)
        IF RIGHT(RTRIM(@IDLP),1) = '\' OR RIGHT(RTRIM(@IDLP),1) = '/'
            SELECT @IDLP = SUBSTRING(@IDLP, 1, LEN(@IDLP) - 1)

        SELECT @IDDP AS DefaultDataPath, @IDLP AS DefaultLogPath
"@
    Invoke-SqlQuery -Query $Query -Server $Server
}

function Get-RestoreResultTableDeclaration($table_name) {
    $decl = @"
    DECLARE @$table_name TABLE (
        LogicalName varchar(128),
        [PhysicalName] varchar(128), 
        [Type] varchar, 
        [FileGroupName] varchar(128), 
        [Size] varchar(128),
        [MaxSize] varchar(128), 
        [FileId]varchar(128), 
        [CreateLSN]varchar(128), 
        [DropLSN]varchar(128), 
        [UniqueId]varchar(128), 
        [ReadOnlyLSN]varchar(128), 
        [ReadWriteLSN]varchar(128),
        [BackupSizeInBytes]varchar(128), 
        [SourceBlockSize]varchar(128), 
        [FileGroupId]varchar(128), 
        [LogGroupGUID]varchar(128), 
        [DifferentialBaseLSN]varchar(128), 
        [DifferentialBaseGUID]varchar(128), 
        [IsReadOnly]varchar(128), 
        [IsPresent]varchar(128), 
        [TDEThumbprint]varchar(128),
        [SnapshotUrl]varchar(128)
    )
"@
    return $decl
}

function Invoke-SqlQuery {
    param(
        [Parameter(Mandatory,ValueFromPipeline,Position=0)]
        [String] $Query,
        [Parameter(Mandatory)]
        [Alias('Host')]
        [String] $Server,
        [String] $Catalog = 'master',
        [Alias('Variable')]
        [String[]] $Variables = @()
    )
    process {
        $command = "Invoke-SqlCmd -ServerInstance '$Server' -Database '$Catalog' -Verbose -QueryTimeout 0 "
        foreach($key in $script:creds.Keys) {
            $value = $script:creds[$key]
            $command += "-$key '$value' "
        }
        foreach($var in $Variables) {
            $command += "$var "
        }
        $command += "-Query @`"`n$Query`n`"@"
        [System.Data.DataRow[]] $rows = $(Invoke-Expression -Command $command)
        foreach($row in $rows){
            $data = @{}
            foreach($column in $row.Table.Columns) {
                $data[$column.ColumnName] = $row[$column]
            }
            Write-Output $data
        }
    }
}

function Get-CreateCatalogTemplate($Server, $Catalog) {
    $server_md = Read-ServerMetadata -Server $Server
    $Template = @"
        CREATE DATABASE [$Catalog]
        CONTAINMENT = NONE
        ON PRIMARY ( 
            NAME = N'$Catalog', 
            FILENAME = N'$(Join-Path -Path $server_md.DefaultDataPath -ChildPath $Catalog.mdf)' , 
            SIZE = 8192KB , FILEGROWTH = 65536KB 
        )
        LOG ON ( 
            NAME = N'$($Catalog)_log', 
            FILENAME = N'$(Join-Path -Path $server_md.DefaultLogPath -ChildPath $($Catalog)_log.ldf)', 
            SIZE = 8192KB , FILEGROWTH = 65536KB
        )
        FILEGROUP [GMCOM-FG] CONTAINS FILESTREAM ( 
            NAME = N'$($Catalog).FS', 
            FILENAME = N'$(Join-Path -Path  $server_md.DefaultDataPath -ChildPath $($Catalog).FS)',
        )
        GO
        <<CATALOG-OPTIONS>>
        GO
        USE [$Catalog]
        GO
        ALTER DATABASE [$Catalog] SET FILESTREAM(NON_TRANSACTED_ACCESS = FULL, DIRECTORY_NAME = N'$($Catalog).FS' ) 
        GO
        IF NOT EXISTS (SELECT name FROM sys.filegroups WHERE is_default=1 AND name = N'PRIMARY') 
            ALTER DATABASE [$Catalog] MODIFY FILEGROUP [PRIMARY] DEFAULT
        GO
"@;
    return $Template
}

function VersionCompatibility($Version) {
    switch($Version) {
        2017 { return 140; }
        2016 { return 130; }
        2014 { return 120; }
        2012 { return 110; }
        2008 { return 100; }
    }
}

function Get-CreateCatalogOptions($Version) {
    $Compatibility = VersionCompatibility -Version $version
    $Options = @{
        EnabledOptions = @("AUTO_UPDATE_STATISTICS");
        DisabledOptions = @(
            "ANSI_NULL_DEFAULT", "ANSI_NULLS", "ANSI_PADDING", "ANSI_WARNINGS", "ARITHABORT", "AUTO_CLOSE", "AUTO_SHRINK",
            "CURSOR_CLOSE_ON_COMMIT", "CONCAT_NULL_YIELDS_NULL", "NUMERIC_ROUNDABORT", "QUOTED_IDENTIFIER", "RECURSIVE_TRIGGERS",
            "AUTO_UPDATE_STATISTICS_ASYNC", "DATE_CORRELATION_OPTIMIZATION", "READ_COMMITTED_SNAPSHOT"
        );
        ToggledOptions = @("DISABLE_BROKER", "READ_WRITE", "MULTI_USER");
        ValueOptions = @(
            @("COMPATIBILITY_LEVEL", "$Compatibility"), @("AUTO_CREATE_STATISTICS", "ON(INCREMENTAL = OFF)"), @("CURSOR_DEFAULT", "GLOBAL"),
            @("PARAMETERIZATION", "SIMPLE"), @("RECOVERY", "FULL"), @("PAGE_VERIFY", "CHECKSUM"), @("TARGET_RECOVERY_TIME", "= 60 SECONDS "),
            @("DELAYED_DURABILITY", "= DISABLED") # = DISABLED originally
        );
        ScopedConfigOptions = @(
            @("LEGACY_CARDINALITY_ESTIMATION", "Off"), @("MAXDOP", "0"), @("PARAMETER_SNIFFING", "On"), @("QUERY_OPTIMIZER_HOTFIXES", "Off")
        );
    }
    return $Options;
}

function Get-CreateCatalogQuery($Server, $Catalog, $Version = 2017) {
    $Options = Get-CreateCatalogOptions -Version $Version
    $OptionStatements = @();
    foreach($key in $Options.EnabledOptions) {
        $OptionStatements += "ALTER DATABASE [$Catalog] SET $key ON"
    }
    foreach($key in $Options.DisabledOptions) {
        $OptionStatements += "ALTER DATABASE [$Catalog] SET $key OFF"
    }
    foreach($key in $Options.ToggledOptions) {
        $OptionStatements += "ALTER DATABASE [$Catalog] SET $key"
    }
    foreach($keyval in $Options.ValueOptions) {
        $OptionStatements += "ALTER DATABASE [$Catalog] SET $($keyval[0]) $($keyval[1])";
    }
    $OptionStatements += "USE [$Catalog]"
    foreach($keyval in $Options.ScopedConfigOptions) {
        $OptionStatements += "ALTER DATABASE SCOPED CONFIGURATION SET $($keyval[0]) = $($keyval[1])";
        $OptionStatements += "ALTER DATABASE SCOPED CONFIGURATION FOR SECONDARY SET $($keyval[0]) = PRIMARY";
    }
    $JoinedStatements = $OptionStatements -join "`n        GO`n        ";
    $Template = Generate-CreateCatalogTemplate -Server $Server -Catalog $Catalog
    $Query = $Template -replace "<<CATALOG-OPTIONS>>", $JoinedStatements
    return $Query
}

function New-Catalog($Server, $Catalog, $Version = 2017, [switch]$Overwrite, [switch]$GenerateOnly) {
    # TODO: If more parameters are needed than just 'Version', consider adding
    # a parameter type/class to encapsulate all db creation parameters.
    $exists = Test-HasCatalog -Server $Server -Catalog $Catalog
    if($exists) {
        if($Overwrite) {
            Remove-Catalog -Server $Server -Catalog $Catalog
        } else {
            Write-Error "Catalog '$Catalog' exists and overwrites are disabled";
            exit
        }
    }
    $Query = Get-CreateCatalogQuery -Server $Server -Catalog $Catalog -Version $Version
    if($GenerateOnly) {
        return $Query
    }
    return Invoke-SqlQuery -Server $Server -Query $Query
}

function Remove-Catalog($Server, $Catalog) {
    Disconnect-Users -Server $Server -Catalog $Catalog
    $Query = "DROP DATABASE [$Catalog]";
    return Invoke-SqlQuery -Server $Server -Query $Query
}

function Backup-Catalog($Server, $Catalog, $BackupPath) {
    Write-Host "Backing up $Server`:$Catalog to archive $BackupPath"
    $Query = @"
        USE [master]
        GO
        BACKUP DATABASE [$Catalog] TO DISK = N'$BackupPath' WITH NAME = N'$Catalog-Full Database Backup', 
            NOFORMAT, INIT, SKIP, NOREWIND, NOUNLOAD, STATS = 5
        GO
"@

    Invoke-SqlQuery -Server $Server -Catalog $Catalog -Query $Query
    Write-Host "Backup complete: $BackupPath"
    $backup = Get-Item $BackupPath
    return $backup
}

function Restore-Catalog($Server, $Catalog, $Backup, [switch]$Overwrite) {
    $Exists = Test-HasCatalog -Server $Server -Catalog $Catalog
    if($Exists) {
        if(-Not $Overwrite) {
            Write-Error "Catalog '$Catalog' exists and overwrites are disabled";
            exit
        }
        $catalog_md = Read-CatalogMetadata -Server $Server -Catalog $Catalog
        $DataPath = "$($catalog_md.DataPath)";
        $LogPath = "$($catalog_md.LogPath)";
        $FsPath = "$($catalog_md.FsPath)";
        Disconnect-Users -Server $Server -Catalog $Catalog
    } else {
        $server_md = Read-ServerMetadata -Server $Server
        $DataPath = "$(Join-Path -Path $server_md.DefaultDataPath -ChildPath "$Catalog-Rows.mdf")";
        $LogPath = "$(Join-Path -Path $server_md.DefaultLogPath -ChildPath "$Catalog-Log.ldf")";
        $FsPath = "$(Join-Path -Path $server_md.DefaultDataPath -ChildPath "$Catalog-FS")";
    }

    $backup_md = Read-BackupMetadata -Backup $Backup -Server $Server
    Write-Host "Restoring $Catalog < '$Backup...'"
    $Query = @"
        USE [master];
        GO
        
        PRINT 'Restoring $Catalog database';
        RESTORE DATABASE [$Catalog] FROM DISK = N'$Backup'
        WITH
            FILE = 1, NOUNLOAD,  REPLACE, STATS = 5,
            MOVE N'$($backup_md.DataFilename)' TO N'$DataPath'
            ,MOVE N'$($backup_md.LogFilename)' TO N'$LogPath'
"@
    if(-Not [string]::IsNullOrWhiteSpace($backup_md.FsFilename)) {
        $Query += ",MOVE N'$($backup_md.FsFilename)' TO N'$FsPath'

        ALTER DATABASE [$Catalog] SET FILESTREAM(NON_TRANSACTED_ACCESS = FULL, DIRECTORY_NAME = N'$($Catalog).FS' ) 
        ";
    }
    if($Exists) {
        Invoke-SqlQuery -Server $Server -Query $Query -Catalog $Catalog
    } else {
        Invoke-SqlQuery -Server $Server -Query $Query
    }
    
}

function Get-IdentityInsertScript {
    param(
        [Parameter(Mandatory=$false)]
        [String] $Env = [NullString]::Value,
        [Parameter(Mandatory=$true)]
        [Alias('EmailAddress')]
        [String] $Email,
        [Parameter(Mandatory=$false)]
        [String] $Password = [NullString]::Value,
        [Parameter(Mandatory=$false)]
        [Nullable[Guid]] $AccountId = $null,
        [Parameter(Mandatory=$false)]
        [String] $FirstName = 'FirstName',
        [Parameter(Mandatory=$false)]
        [String] $LastName = 'LastName',
        [Parameter(Mandatory=$false)]
        [Alias('Phone')]
        [String] $PhoneNumber = [NullString]::Value,
        [Parameter(Mandatory=$true)]
        [ValidateSet('male','female','m','f')]
        [String] $Gender
    )
    if($null -eq $AccountId) {
        $AccountId = [Guid]::NewGuid()
    }
    $EmailConfirmed = 1
    if($null -eq $Password) {
        $Password = Get-XkcdPassword -MinWordLength 8 -WordCount 3
    }
    if($null -eq $Env) {
        $EnvDot = ''
    } else {
        $EnvDot = "$Env."
    }
    $PasswordHash = "[$($EnvDot)GmCommon].[clr].ComputeDataHash('$Password')"

    $data = [ordered]@{
        IdentityUsers = @{
            TableName = "[$($EnvDot)GmIdentity].[dbo].[AspNetUsers]";
            FieldValues = [ordered]@{
                Id = $AccountId.ToString();
                UserName = $Email;
                NormalizedUserName = $Email.ToUpper();
                Email = $Email;
                NormalizedEmail = $Email.ToUpper();
                EmailConfirmed = 1;
                PasswordHash = "[$($EnvDot)GmCommon].[clr].ComputeDataHash('$Password')";
                SecurityStamp = 'NULL';
                ConcurrencyStamp = 'NULL';
                PhoneNumber = $PhoneNumber;
                PhoneNumberConfirmed = 0;
                TwoFactorEnabled = 0;
                LockoutEnd = 'NULL';
                LockoutEnabled = 0;
                AccessFailedCount = 0;
            };
            Literals = @{
                PasswordHash = $true;
            ;}
        };
        ComUsers = [ordered]@{
            TableName = "[$($EnvDot)GmCommon].[carma].[Users]";
            FieldValues = @{
                AgencyID = 6;
                AccountID = $AccountId.ToString();
                LookupID = [Guid]::NewGuid().ToString();
                Email = $Email;
                FirstName = $FirstName;
                LastName = $LastName;
                Phone = $PhoneNumber;
                UserRole = 3;
                Gender = ($Gender -eq 'male' -or $Gender -eq 'm' ? 1 : 2);
                LanguageCode = 'en';
                Profile = '{}';
            };
            Literals = @{};
        };
        IdentityClaims = [ordered]@{
            TableName = "[$($EnvDot)GmIdentity].[dbo].[AspNetUserClaims]";
            FieldValues = @{
                UserId = $AccountId.ToString();
                ClaimType = 'AgencyId';
                ClaimValue = '6';
            };
            Literals = @{};
        };
    }

    $script = [String]::Empty
    foreach($tableId in $data.Keys) {
        $table = $data[$tableId]
        $fields = @($table.FieldValues.Keys)
        $values = $table.FieldValues
        $literals = $table.Literals
        foreach($field in $fields) {
            $value = $values[$field]
            if($value -eq 'NULL') {
                $literals[$field] = $true
            }
            elseif($value -isnot [String]) {
                $literals[$field] = $true
            }
        }
        $field_list = $(@($fields | % { "[$_]" }) -Join ', ')
        $value_list = $(@($fields | % { $literals.ContainsKey($_) ? $values[$_] : "'$($values[$_])'" }) -Join ', ')
        $command = "INSERT INTO $($table.TableName) ($field_list)`nVALUES ($value_list)`nGO"
        $script += "$command`n"
    }
    $script
}

Export-ModuleMember New-Catalog, Remove-Catalog, Restore-Catalog, Backup-Catalog, Invoke-SqlQuery, Get-IdentityInsertScript
