$script:AnsiEncoding = [System.Text.Encoding]::Default;
$script:Utf8NoBom = New-Object System.Text.UTF8Encoding $false

# Iterate files in their entirety and replace pattern matches irrespective of line breaks
function Update-MatchingFileContents($Pattern, $Replacement, $IncludeFilters, $Root, [switch]$IgnoreCase, [switch]$DryRun) {
    if($null -eq $Root) {
        $Root = $global:WebDeployRoot
    }
    Push-Location $Root
    $files = Get-ChildItem -Recurse -Include $IncludeFilters
    foreach($file in $files) {
        Write-Debug "Checking file: $file"
        $rel_path = Resolve-Path $file.FullName -Relative
        $content = Get-Content -Raw "$file"
        if($content -match $Pattern) {
            Write-Debug "Replacing content in file: $file"
            if($DryRun -Or ($null -eq $Replacement)) {
                continue;
            }
            if($IgnoreCase) {
                $content = $content -ireplace $Pattern,$Replacement
            }
            else {
                $content = $content -replace $Pattern,$Replacement
            }
            
            $content | Set-Content -NoNewline $file
        }
    }
    Pop-Location
}


# Iterate files line by line and replace pattern matches in each line with the provided replacement
function Update-MatchingLineData($Root, $Pattern, $Replacement, $IncludeFilters, [switch]$EntireLine, [switch]$DryRun) {
    if($null -eq $Root) {
        $Root = $global:WebDeployRoot
    }
    if($null -eq $Pattern) {
        Write-Error "No pattern supplied to Update-MatchingLineData";
        exit
    }
    if($DryRun) {
        Write-Debug "Called Update-MatchingLineData("
        Write-Debug "  Pattern: $Pattern"
        Write-Debug "  IncludeFilters: $IncludeFilters"
        Write-Debug "  Root: $Root"
        Write-Debug "  Replacement: $Replacement"
        Write-Debug "  EntireLine: $EntireLine"
        Write-Debug "  DryRun: $DryRun"
        Write-Debug ")"
    }
    $update_data = @()
    $search_data = $(Search-MatchingLineData -Root $Root -Pattern $Pattern -IncludeFilters $IncludeFilters)
    foreach($file_data in $search_data) {
        if($DryRun) {
            Write-Debug "Matched file with $($file_data.lines.Length) lines: $($file_data.rel_file)"
            Write-Debug "------------------------------------"
        }
        foreach($match_data in $file_data.match_data) {
            if($EntireLine) {
                $content = $match_data.content -replace '^(?<indent>\s*)\S.*$',$Replacement
            } else {
                $content = $match_data.content -creplace $Pattern,$Replacement
            }
            if($DryRun) {
                Write-Debug "Matched line $($match_data.index)"
                Write-Debug "Line Content: $($match_data.content)"
                Write-Debug "Replacement Content: $($content)"
            } else {
                $match_data.old_content = $match_data.content
                $match_data.content = $content
                $file_data.lines[$match_data.index] = $content
            }
        }
        if($DryRun) {
            Write-Debug "------------------------------------"
        } else {
            $file_data.lines | Set-Content $file_data.file
        }
    }
    return $search_data
}

function Search-MatchingLineData($Root, $Pattern, $IncludeFilters) {
    if($null -eq $Root) {
        $Root = $global:WebDeployRoot
    }
    Push-Location $Root
    $files = Get-ChildItem $Root -Recurse -Include $IncludeFilters
    $search_data = @();
    foreach($file in $files) {
        $lines = Get-Content $file
        $file_data = @{
            file = $file.FullName;
            rel_file = $file.FullName.Substring($Root.Length, $file.FullName.Length - $Root.Length);
            match_data = @();
        }
        for($i = 0; $i -lt $lines.Length; $i++) {
            if($lines[$i] -match $Pattern) {
                $match_data = @{
                    content = $lines[$i];
                    index = $i;
                    match = $matches[0];
                }
                $file_data.match_data += $match_data
            }
        }
        if($file_data.match_data.Length -gt 0) {
            $file_data.lines = $lines
            $search_data += $file_data
        }
    }
    Pop-Location
    return $search_data
}

# Insert whitespace matches at appropriate locations in the provided Regex pattern.
function Add-WhitespacePatterns($Pattern, $BoundaryTokens) {
    $Pattern = $Pattern -replace "\s+",'\s+'
    foreach($bt in $BoundaryTokens) {
        $Pattern = $Pattern -replace "$bt","`\`s`*$bt`\`s`*"
    }
    $Pattern = $Pattern -replace '(\\s\+)+','\s+'
    $Pattern = $Pattern -replace '(\\s\*)+','\s*'
    $Pattern = $Pattern -replace '(\\s\*|\\s\+)$',''
    return $Pattern
}

# Check if the file contains a byte order mark
function Test-EncodingBOM {
    # Via: https://superuser.com/a/418520
    return $input | where {
        $contents = new-object byte[] 3
        $stream = [System.IO.File]::OpenRead($_.FullName)
        $stream.Read($contents, 0, 3) | Out-Null
        $stream.Close()
        $contents[0] -eq 0xEF -and $contents[1] -eq 0xBB -and $contents[2] -eq 0xBF 
    }
}

function Remove-EncodingBOM($Root, $IncludeFilters) {
    if($null -eq $Root) {
        $Root = $global:WebDeployRoot
    }
    if($null -eq $IncludeFilters) {
        $IncludeFilters = @("*.*");
    }
    Push-Location $Root
    $files = Get-ChildItem -Recurse -Include $IncludeFilters -File | Test-EncodingBOM
    foreach($file in $files) {
        $content = Get-Content $file
        $content | Set-Content -NoNewline $file
    }
    Pop-Location
}
function Read-BomEncoding($Bytes, $Path) {
    if($null -eq $Bytes) {
        $Bytes = [System.IO.File]::ReadAllBytes($Path);
    }
    if($Bytes.Length -ge 4) {
        $bom = $Bytes[0..3];
        if ($bom[0] -eq 0x2b -And $bom[1] -eq 0x2f -And $bom[2] -eq 0x76) { 
            return  [System.Text.Encoding]::UTF7; 
        }
        if ($bom[0] -eq 0xef -And $bom[1] -eq 0xbb -And $bom[2] -eq 0xbf) { 
            return [System.Text.Encoding]::UTF8;
        }
        if ($bom[0] -eq 0xff -And $bom[1] -eq 0xfe) { 
            return [System.Text.Encoding]::Unicode; # UTF-16LE
        }
        if ($bom[0] -eq 0xfe -And $bom[1] -eq 0xff) { 
            return [System.Text.Encoding]::BigEndianUnicode; # UTF-16BE
        } 
        if ($bom[0] -eq 0 -And $bom[1] -eq 0 -And $bom[2] -eq 0xfe -And $bom[3] -eq 0xff) { 
            return [System.Text.Encoding]::UTF32; 
        }
    }
    return $null;
}

function Test-Utf8($Bytes, $Path) {
    if($null -eq $Bytes) {
        $Bytes = [System.IO.File]::ReadAllBytes($Path);
    }
    for($i = 0; $i -lt $Bytes.Length; $i += $cp_len) {
        $cp_start = $Bytes[$i];
        $cp_head = $cp_start -band 0xF0
        $cp_tail = $cp_start -band 0x0F
        if($cp_head -lt 0x80) {
            $cp_len = 1;
            continue;
        }
        elseif(($cp_head -eq 0xC0) -or ($cp_head -eq 0xD0)) {
            $cp_len = 2;
        }
        elseif($cp_head -eq 0xE0) {
            $cp_len = 3;
        }
        elseif($cp_head -eq 0xF0 -and $cp_tail -lt 0x8) {
            $cp_len = 4;
        }
        else {
            return $false
        }
    }
    return $true
}

function Resolve-Encoding($Bytes, $Path) {
    if($null -eq $Bytes) {
        $Bytes = [System.IO.File]::ReadAllBytes($Path);
    }
    Write-Debug "Deducing BOM encoding"
    $bom_enc = $(Read-BomEncoding -Bytes $Bytes);
    if($null -eq $bom_enc) {
        Write-Debug "No BOM encoding; checking UTF8 validity."
        if($(Test-Utf8 -Bytes $Bytes)) {
            Write-Debug "Valid UTF8 detected."
            return $script:Utf8NoBom
        }
        Write-Debug "Not valid UTF8; assuming ANSI"
        return $script:AnsiEncoding
    }
    Write-Debug "BOM encoding: $($bom_enc.BodyName)"
    return $bom_enc
}
function ConvertTo-UTF8($Root, $IncludeFilters) {
    if($null -eq $Root) {
        $Root = $global:WebDeployRoot
    }
    Push-Location $Root
    $files = Get-ProjectFiles -Root $Root -IncludeFilters $IncludeFilters 
    foreach($file in $files) {
        Write-Debug "Processing encoding for file: $($file.FullName)"
        $bytes = [System.IO.File]::ReadAllBytes($file);
        $src_enc = $(Resolve-Encoding -Bytes $bytes)
        Write-Debug "Deduced encoding: $($src_enc.BodyName)"
        $tgt_enc = $script:Utf8NoBom
        if($src_enc -eq $script:Utf8NoBom) {
            continue;
        }
        Write-Debug "Converting $($file.FullName): $($src_enc.BodyName)[$($src_enc.GetPreamble().Length)] --> $($tgt_enc.BodyName)[$($tgt_enc.GetPreamble().Length)]"
        $content = $src_enc.GetString($bytes);
        $out_bytes = $tgt_enc.GetBytes($content);
        [System.IO.File]::WriteAllBytes($file, $out_bytes);
    }
    Pop-Location
}

function Format-XmlFile {
    Param (
        [parameter(ValueFromPipeline)]
        [string]$Path,
        [parameter()]
        [int]$Indent = 4
    )
    [xml]$xml = Get-Content -Path $Path 
    $xml | Format-Xml -Indent $Indent | Set-Content -Path $Path
}

function Format-Xml {
    Param (
        [parameter(ValueFromPipeline)]
        [xml[]]$Xml,
        [parameter()]
        [int]$Indent = 4
    )
    $StringWriter = New-Object System.IO.StringWriter
    $XmlWriter = New-Object System.XMl.XmlTextWriter $StringWriter
    $XmlWriter.Formatting = "indented"
    $XmlWriter.Indentation = $Indent
    $Xml.WriteContentTo($XmlWriter)
    $XmlWriter.Flush()
    $StringWriter.Flush()
    return $StringWriter.ToString()
}


Export-ModuleMember -Function Update-MatchingFileContents, Update-MatchingLineData, Format-XmlFile, Format-Xml, ConvertTo-UTF8, Add-WhitespacePatterns, Search-MatchingLineData