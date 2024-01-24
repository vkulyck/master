Import-Module "$PSScriptRoot/SQL"
Import-Module "$PSScriptRoot/FileIO"

$script:ProjectFileExtensions = @{
    "SourceCode" = @(
        "asax"; "ascx"; "aspx"; "config"; "css"; 
        "htm"; "html";  "js"; "json"; "jsonp"; "master"; "md"; 
        "resx"; "sql";  "txt"; "vb"; "xml"
    );
    "ProjectCode" = @(
        "asax"; "ascx"; "aspx"; "config"; "css"; 
        "htm"; "html";  "js"; "json"; "jsonp"; "master"; "md"; 
        "resx"; "sql";  "txt"; "vb"; "xml"
    );
    "ServiceDef" = @(
        "disco"; "discomap";"wsdl"; 
    );
    "Lib" = @(
        "dll"; "refresh"; "thmx"
    );
    "Image" = @(
        "gif"; "jpg"; "pdb"; "pdf"; "png"; 
    )
    "Doc" = @(
        "pdb"; "pdf";"xls"; "xlsx"; "doc"; "docx"; "flv";
    )
    "SourceControl" = @(
        "aspx1"; "aspx2"; "vb1"; "vb2"; "scc";
    )
};
$script:InstanceList = @("GMLIVE", "GMPLAY", "MOCDBase", "NPCOM", "TGSEWD", "TGSEWDTST", "TGSSFO");
$script:InstancePattern = '^/(|' + [string]::Join('|', $script:InstanceList) + ')(?:$|/)';


function Get-ProjectFiles($Root, $IncludeFilters) {
    if($null -ne $IncludeFilters) {
        $IncludeFilters = $($script:ProjectFileExtensions.ProjectCode | % { "*.$_"})
    }
    if($null -ne $Root) {
        $Root = $global:WebDeployRoot
    }

    Push-Location $Root
    $excludes = @( "RadControls", "plugins", "bin", "Database")
    $files = Get-ChildItem -File -Recurse -Include $IncludeFilters -Exclude $excludes | %{ 
        $rel = $_.FullName.Substring($Root.Length, $_.FullName.Length - $Root.Length)
        foreach ($exclude in $excludes) { 
            if ((Split-Path $_.FullName -Parent) -ilike "*\$exclude*") { 
                Write-Debug "Excluding '$rel': $exclude" -ForegroundColor Red
                return
            }
        }
        Write-Debug "Keeping '$_'" -ForegroundColor Green
        return $_
    }
    Pop-Location
    return $files
}

function Update-SwitcherReferences($Root) {
    # Update Switcher assembly registration with replacement:
    $Replacement = 'Register Assembly="GmWeb.IdentityAPI, Version=1.0.0.0, Culture=${culture}, PublicKeyToken=7618f81aa2ee888c" Namespace="GmWeb.IdentityAPI"'
    $IncludeFilters = @("*.aspx", "*.master")
    $NamePattern = '(CustomControls(NP)?|(ClientServices\.)?(IdentityCache|IdentityAPI))'
    $Pattern = 
        'Register\s+Assembly="' +
            '(?<name>' + $NamePattern + '),\s+' +
            'Version=(?<version>\d+\.\d+\.\d+\.\d+),\s+' +
            'Culture=(?<culture>\w+),\s+' +
            'PublicKeyToken=(?<token>[abcdef0-9]+)"\s+' +
        'Namespace="(?<ns>' + $NamePattern + ')"'
    ;
    Update-MatchingFileContents -Pattern $Pattern -IncludeFilters $IncludeFilters -Replacement $Replacement -Root $Root

    # Update Switcher assembly import with replacement:
    $Replacement = '<add assembly="GmWeb.IdentityAPI, Version=1.0.0.0, Culture=${culture}, PublicKeyToken=7618f81aa2ee888c" />'
    $IncludeFilters = @("Web.config")
    $NamePattern = '(CustomControls(NP)?|(ClientServices\.)?(IdentityCache|IdentityAPI))'
    $Pattern = 
        '<add\s+assembly="' +
            '(?<name>' + $NamePattern + '),\s+' +
            'Version=(?<version>\d+\.\d+\.\d+\.\d+),\s+' +
            'Culture=(?<culture>\w+),\s+' +
            'PublicKeyToken=(?<token>[abcdef0-9]+)"' +
        '\s*/>'
    ;
    Update-MatchingFileContents -Pattern $Pattern -IncludeFilters $IncludeFilters -Replacement $Replacement -Root $Root -IgnoreCase
}


function Update-PublicSigningTokens($Root) {
    $IncludeFilters = @("*.aspx", "Web.config", "*.master")
    $Pattern = "PublicKeyToken=255b635bcd268c80"
    $Pattern = Add-WhitespacePatterns -Pattern $Pattern -BoundaryTokens @("=", "/>")
    $Replacement = "PublicKeyToken=7618f81aa2ee888c"
    Update-MatchingLineData -Pattern $Pattern -IncludeFilters $IncludeFilters -Replacement $Replacement -Root $root
}

function Update-ConnectionStrings($Root, [switch]$Update) {
    $IncludeFilters = @("Web.config")
    $Pattern = '<add key="ConnectionString" value="[^"]+"\s*/>'
    $Pattern = Add-WhitespacePatterns -Pattern $Pattern -BoundaryTokens @("=", "/>")
    #$Replacement = "<add key=`"ConnectionString`" value=`"data source=localhost\sql2017;initial catalog=GMPLAY;Integrated Security=SSPI;`" />"
    $Replacement = "";
    if($Update) {
        Update-MatchingFileContents -Pattern $Pattern -IncludeFilters $IncludeFilters -Root $Root -Replacement $Replacement
    } else {
        Update-MatchingFileContents -Pattern $Pattern -IncludeFilters $IncludeFilters -Root $Root
    }
}

function Update-InstanceSetup($Instance) {
    Info "Processing: $Instance"
    $root = "$($global:WebDeployRoot)\$Instance";
    Info "Modifying connection strings"
    Update-ConnectionStrings -Root $root -Update
    Info "Updating switcher signing tokens"
    Update-PublicSigningTokens -Root $root
    Info "Generating instance config"
    New-InstanceConfig -Instance $Instance
    Info "Swapping custom control references"
    Update-SwitcherReferences -Root $root
    Info "Ensuring UTF8 encodings"
    $IncludeFilters = $($script:ProjectFileExtensions.ProjectCode | foreach { "*.$_"})
    ConvertTo-UTF8 -Root $root -IncludeFilters $IncludeFilters
}

function Move-QuarantineFiles($Instance, $Extensions) {
    if($null -eq $Instance) {
        throw "No instance supplied to Quarantine-Files"
    }
    $SourceRoot = "$global:WebDeployRoot\$Instance"
    if($Extensions -is [string]) {
        $Group = $Extensions
        $Extensions = $script:ProjectFileExtensions[$Group];
        Trace "Quarantining $Group extensions: $Extensions"
        $TargetRoot = "$($global:QuarantineRoot)\$Group\$Instance"
    }
    elseif($Extensions -is [array]) {
        Trace "Quarantining file extensions: $Extensions"
        $TargetRoot = "$($global:QuarantineRoot)\Files\$Instance"
    } else {
        throw "Invalid extensions provided for file quarantine: $Extensions"
    }
    Push-Location $SourceRoot
    $Filters = $Extensions | ForEach-Object { "*.$_" }
    $files = Get-ChildItem -Recurse -Include $Filters -File
    foreach($file in $files) {
        $rel_dir = Resolve-Path $file.FullName -Relative | Split-Path -Parent
        $target_dir = Join-Path $TargetRoot $rel_dir
        if(-Not $(Test-Path $target_dir)) {
            New-Item -ItemType Directory -Path $target_dir
        }
        Move-Item $file -Destination $target_dir
    }
    Pop-Location
}

function Move-QuarantineApps($Instance, $Whitelist, $Blacklist) {
    $SourceRoot = "$global:WebDeployRoot\$Instance"
    $TargetRoot = "$global:QuarantineRoot\Apps\$Instance"
    if(-Not $(Test-Path $TargetRoot)) {
        New-Item -ItemType Directory -Path $TargetRoot
    }
    if($null -eq $Whitelist) {
        $Whitelist = Get-Instance-Apps -Instance $Instance
    }
    $Whitelist += @(".vs", "bin", "css", "fonts", "images", "Scripts", "stylesheets");
    if($null -ne $Blacklist) {
        $Whitelist = $Whitelist | Where-Object { $_ -Notin $Blacklist }
    }
    $directories = Get-ChildItem $SourceRoot -Directory
    foreach($dir in $directories) {
        [bool]$quarantine = $($dir -Notin $Whitelist);
        if($quarantine) {
            Trace "Quarantining $Instance\$dir"
            Move-Item "$SourceRoot\$dir" -Destination "$TargetRoot\" -ErrorAction "Ignore"
        }
    }
}

function Update-SiteConfigPaths($SourcePath, $TargetPath) {
    [xml]$doc = Get-Content $SourcePath;
    $site = $doc.SelectNodes("configuration/system.applicationHost/sites/site[@name='$global:ActiveWebsite']")
    $sanitized = $site.ChildNodes | Sort-Object path | ? { $_.path -match $script:InstancePattern }
    while($site.ChildNodes.Count -gt 0) {
        $site.RemoveChild($site.FirstChild) | Out-Null;
    }
    foreach($node in $sanitized) {
        $node.virtualDirectory.physicalPath = $node.virtualDirectory.physicalPath.Replace("D:\inetpub\wwwroot", "%GMWEB_WWWROOT%");
        $site.AppendChild($node) | Out-Null;
    }
    $site.OuterXml | Format-Xml | Out-File -FilePath $TargetPath
}

function New-InstanceConfig($Instance) {
    $Catalog = $Instance
    if($Catalog -eq "MOCDBase") {
        $Catalog = "MOCD";
    }
    $SourcePath = "$global:LegacyRoot\Scripts\instance.web.config"
    $TargetPath = "$global:WebDeployRoot\$Instance\web.config"
    [xml]$doc = Get-Content $SourcePath;
    $node = $doc.SelectNodes("configuration/appSettings/add[@key='ConnectionString']")[0]
    $node.value = $node.value -ireplace 'initial catalog=([^;]+);',"initial catalog=$Catalog;"
    $node.value = $node.value -ireplace 'data source=([^;]+);',"data source=$($global:SqlServer);"
    $doc.OuterXml | Format-Xml | Out-File -FilePath $TargetPath
}

function Update-LocalSiteConfigPaths {
    $SourcePath = "$PSScriptRoot\site-apphost.config"
    $TargetPath = [System.IO.Path]::Combine(
        [System.IO.Path]::GetDirectoryName($SourcePath),
        [System.IO.Path]::GetFileNameWithoutExtension($SourcePath) + "-sanitized.config"
    );
    Update-SiteConfigPaths -SourcePath $SourcePath -TargetPath $TargetPath
}

function Get-InstanceWebApps($ConfigPath, $Instance) {
    if($null -ne $ConfigPath) {
        $ConfigPath = $global:IISAppHostConfigPath
    }
    [xml]$doc = Get-Content $ConfigPath;
    $site = $doc.SelectNodes("configuration/system.applicationHost/sites/site[@name='$global:ActiveWebsite']")
    $apps = @();
    foreach($node in $site.ChildNodes) {
        $path = $node.path
        $match = [regex]::Match($path,'^/([^/]+)/([^/]+)$')
        if(-Not $match.Success) { 
            continue; 
        }
        $inst = $match.Groups[1].Value
        $app = $match.Groups[2].Value
        if($inst -ne $Instance) {
            continue;
        }
        $apps += $app;
    }
    return $apps;
}


enum SyncAction {
    Create
    CreateUnlessExists
    Delete
}

function Sync-BuildFiles {
    param(
        [String[]]$Sources,
        [Parameter(Mandatory=$true)]
        [String]$WebRoot,
        [String[]]$TargetFilters,
        [SyncAction]$Action
    )
    if($null -eq $TargetFilters) {
        $targets = @( $WebRoot; )
    } else {
        $targets = $(Get-ChildItem $WebRoot -Include $TargetFilters -Directory -Recurse)
    }
    foreach($source in $Sources) {
        Write-Host "Performing $action on $source in $($targets.Length) target directories"
        foreach($target_dir in $targets) {
            $target = Join-Path -Path $target_dir -ChildPath $source
            switch($action) {
                Create { 
                    New-Item -Path "$target" -ItemType File -Force | Out-Null # Copy source to target
                    Copy-Item "$source" "$target" -Recurse
                }
                CreateUnlessExists { 
                    if(Test-Path $target) {
                        break
                    }
                    New-Item -Path "$target" -ItemType File -Force | Out-Null # Copy source to target
                    Copy-Item "$source" "$target" -Recurse
                }
                Delete {
                    Remove-Item $target -ErrorAction SilentlyContinue
                }
            }
        }
    }
}

function Install-Library($Sources, $WebRoot) {
    Sync-BuildFiles -Sources $Sources -WebRoot $WebRoot -TargetFilter "*Bin" -Action Create
}

function Install-Config($Sources, $WebRoot) {
    Sync-BuildFiles -Sources $Sources -WebRoot $WebRoot -TargetFilter "*App_Data" -Action Create
}

function Remove-Library($Sources, $WebRoot) {
    Sync-BuildFiles -Sources $Sources -WebRoot $WebRoot -TargetFilter "*Bin" -Action Delete
}

Export-ModuleMember Update-Instances, Update-InstanceSetup, Install-Config, Install-Library, Remove-Library
