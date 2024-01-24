using module ./JSON.psm1
using namespace System.Collections
using namespace System.Collections.Specialized
using namespace System.IO
<#
.SYNOPSIS
  A convention for formatting variable identifiers in source code
  using a combination of letter cases, hyphens, underscores, periods,
  and whitespace.
.LINK
  https://en.wikipedia.org/wiki/Naming_convention_(programming)
#>
enum CaseType {
  None
  Upper
  Lower
  Title
  # The TailTitle case type applies Lower casing to the first element in an array, and
  # then applies Title casing to the remaining elements in the tail of the array. For
  # now this is only used for camelCase.
  TailTitle
}

class NamingConvention {
  [String] $Name
  [String] $Sample
  [String] $Pattern
  [CaseType] $CaseType
  [String] $Delimiter
  [String] ToString() {
      return $this.Name
  }
  static [OrderedDictionary] $Conventions = [ordered]@{
    Cobol = $(New-Object NamingConvention -Property @{
      Sample = 'THIS-IS-COBOL-CASE';
      Pattern = '^(?<part>[A-Z]+)(?:\-(?<part>[A-Z]+))*$';
      CaseType = [CaseType]::Upper
      Delimiter = '-'
    });
    Domain = $(New-Object NamingConvention -Property @{
      Sample = 'this.is.domain.case';
      Pattern = '^(?<part>[a-z][a-z0-9]*)(?:\.(?<part>[a-z][a-z0-9]*))*$';
      CaseType = [CaseType]::Lower
      Delimiter = '.'
    });
    DotNet = $(New-Object NamingConvention -Property @{
      Sample = 'This.Is.Dot.Net.Case';
      Pattern = '^(?<part>[A-Z][A-Za-z0-9]*)(?:\.(?<part>[A-Z][A-Za-z0-9]*))*$';
      CaseType = [CaseType]::Title
      Delimiter = '.'
    });
    Pascal = $(New-Object NamingConvention -Property @{
      Sample = 'ThisIsPascalCase';
      Pattern = '^(?<part>[A-Z][a-z0-9]*)+$';
      CaseType = [CaseType]::Title
      Delimiter = ''
    });
    Camel = $(New-Object NamingConvention -Property @{
      Sample = 'thisIsCamelCase';
      Pattern = '^(?<part>[a-z][a-z0-9]*)(?<part>[A-Z][a-z0-9]*)*$';
      CaseType = [CaseType]::TailTitle
      Delimiter = ''
    });
    Snake = $(New-Object NamingConvention -Property @{
      Sample = 'this_is_snake_case';
      Pattern = '^(?<part>[a-z][a-z0-9]*)(?:_(?<part>[a-z0-9]+))*$';
      CaseType = [CaseType]::Lower
      Delimiter = '_'
    });
    Macro = $(New-Object NamingConvention -Property @{
      Sample = 'THIS_IS_MACRO_CASE';
      Pattern = '^(?<part>[A-Z][A-Z0-9]+)(?:_(?<part>[A-Z][A-Z0-9]+))*$';
      CaseType = [CaseType]::Upper
      Delimiter = '_'
    });
    PascalSnake = $(New-Object NamingConvention -Property @{
      Sample = 'This_Is_Pascal_Snake_Case';
      Pattern = '^(?<part>[A-Z][A-Za-z0-9]*)(?:_(?<part>[A-Z][A-Za-z0-9]+))*$';
      CaseType = [CaseType]::Title
      Delimiter = '_'
    });
    Kebab = $(New-Object NamingConvention -Property @{
      Sample = 'this-is-kebab-case';
      Pattern = '^(?<part>[a-z][a-z0-9]*)(?:\-(?<part>[a-z0-9]+))*$';
      CaseType = [CaseType]::Lower
      Delimiter = '-'
    });
    Train = $(New-Object NamingConvention -Property @{
      Sample = 'This-Is-Train-Case';
      Pattern = '^(?<part>[A-Z][A-Za-z0-9]*)(?:\-(?<part>[A-Z][A-Za-z0-9]+))*$';
      CaseType = [CaseType]::Title
      Delimiter = '-'
    });
  }
  static NamingConvention() {
    foreach($key in [NamingConvention]::Conventions.Keys) {
      $nc = [NamingConvention]::Conventions[$key]
      $nc.Name = $key
    }
    # Aliases for simple (single-part) casing
    [NamingConvention]::Conventions['title'] = [NamingConvention]::Conventions['dotnet']
    [NamingConvention]::Conventions['upper'] = [NamingConvention]::Conventions['cobol']
    [NamingConvention]::Conventions['lower'] = [NamingConvention]::Conventions['domain']
  }

  static [Boolean] Contains([String] $Name) {
    return [NamingConvention]::Conventions.Contains($Name)
  }
  static [NamingConvention[]] GetConvention([String] $Name) {
    return [NamingConvention]::Conventions[$Name]
  }
  hidden static [NamingConvention[]] GetConventions() {
    return [NamingConvention]::Conventions.Values
  }
  static [NamingConvention] InferConvention([String] $Identifier) {
    $Identifier = $Identifier.Trim()
    if($Identifier -match '[^\w\-\.]') {
      throw "Invalid characters found in identifier: '$Identifier'"
    }
    foreach($nc in [NamingConvention]::GetConventions()) {
      if($Identifier -cmatch $nc.Pattern) {
        return $nc;
      }
    }
    return $null
  }
  [String] ApplyConvention([String] $Identifier) {
    $idnc = [NamingConvention]::InferConvention($Identifier)
    $parts = $idnc.SplitIdentifier($Identifier)
    $parts = $this.TransformIdentifierParts($parts)
    $converted = $this.JoinIdentifier($Parts)
    return $converted
  }
  hidden [String[]] TransformIdentifierParts([String[]] $Parts) {
    if(0 -eq $Parts.Length) {
      return $Parts
    }
    $Parts = $Parts | ConvertTo-Case $this.CaseType
    if($this -eq [NamingConvention]::Camel) {
      $Parts = @($Parts[0].ToLower();) + $Parts[1..$Parts.Length]
    }
    return $Parts
  }
  hidden [String[]] SplitIdentifier([String] $Identifier) {
    $id_matches = [Regex]::Matches($Identifier, $this.Pattern)
    if($id_matches.Count -eq 0) {
      throw "Could not parse identifier as $($this.Name): '$Identifier'"
    }
    $parts = $id_matches | % { $_.Groups['part'].Captures } | % { $_.Value }
    return $parts
  }
  hidden [String] JoinIdentifier([String[]] $Parts) {
    return $Parts -join $this.Delimiter
  }
}

function ConvertTo-TitleCase {
  param(
    [Parameter(Mandatory,Position=0)]
    [String] $Value
  )
  $titled = $Value.Substring(0, 1).ToUpper() + $Value.Substring(1)
  return $titled
}

function ConvertTo-Case {
  param(
    [Parameter(Mandatory,ValueFromPipeline)]
    [String] $Value,
    [Parameter(Mandatory,Position=0)]
    [CaseType] $Case
  )
  begin {
    $index = 0
  }
  process {
    switch($Case) {
      Upper { $Value.ToUpper() }
      Lower { $Value.ToLower() }
      # Title case isn't well-supported by default. The closest function available is
      # the TextInfo.ToTitleCase method, but this breaks on strings in all caps
      # (ostensibly to avoid title-casing acronyms). As a workaround, we can first
      # convert the string to all lower-case as a workaround so that ToTitleCase can
      # produce the expected output.
      Title { ConvertTo-TitleCase $Value }
      TailTitle {
        if($index -eq 0) {
          $Value.ToLower()
        } else {
          ConvertTo-TitleCase $Value
        }
      }
    }
    $index++
  }
}

<#
.SYNOPSIS
Format the supplied Template string using the provided Data table to map each variable reference
to its assigned value. Each reference may specify an optional format specifier as follows:
{variable_name:format_specifier}, formatted according to the optional ref:fmt argument.
#>
function Format-TemplateString {
  [CmdletBinding()]
  param(
    [Parameter(Mandatory,ValueFromPipeline)]
    [String] $Template,
    [Parameter(Mandatory,Position=0)]
    [Hashtable] $Data
  )
  begin {
    $ph_pattern = '(?<!{ *)(?<ref>{ *(?<key>[A-Za-z]\w*)(?:[:;](?<format>\w+))? *})(?! *})'
  }
  process {
    $tmpl_lines = $Template -split "`r?`n"
    $fmt_lines = @()
    foreach($line in $tmpl_lines) {
      while($line -cmatch $ph_pattern) {
        $value_ref = [Regex]::Escape($Matches['ref'])
        $value_key = $Matches['key']
        if(-not $Data.Contains($value_key)) {
          break
        }
        $value_format = $Matches['format']
        $value = $Data[$value_key]
        if($null -ne $value_format) {
          $is_string = ($value -is [String])
          $is_convention = [NamingConvention]::Contains($value_format)
          if($is_string -and $is_convention) {
            $nc = [NamingConvention]::GetConvention($value_format)
            $value = $nc.ApplyConvention($value)
          } else {
            $value = "{0:$value_format}" -f $value
          }
        }
        $line = $line -replace $value_ref,$value
      }
      $line = $line -replace '\{ *\{','{' -replace '\} *\}','}'
      $fmt_lines += $line
    }
    $formatted = $fmt_lines -join "`n"
    Write-Output $formatted
  }
}

<#
.SYNOPSIS
Format the supplied Template file using the provided Data table to map each variable reference
to its assigned value. Each reference may specify an optional format specifier as follows:
{variable_name:format_specifier}, formatted according to the optional ref:fmt argument.
If the specified file does not exist then a FileNotFoundException is thrown.
#>
function Format-TemplateFile {
  [CmdletBinding()]
  param(
    [Parameter(Mandatory,ValueFromPipeline)]
    [String] $Path,
    [Parameter(Mandatory,Position=0)]
    [Hashtable] $Data
  )
  process {
    if(-not $(Test-Path -Path $Path -PathType Leaf)) {
      Write-Error -Exception ([System.IO.FileNotFoundException]::new("File does not exist: $Path")) -ErrorAction Stop
    }
    $content = Get-Content -Path $Path -Raw
    $formatted = $content | Format-TemplateString -Data $Data
    Write-Output $formatted
  }
}

<#
.SYNOPSIS
Format the supplied Template file using the provided Data table to map each variable reference
to its assigned value. Each reference may specify an optional format specifier as follows:
{variable_name:format_specifier}, formatted according to the optional ref:fmt argument.
If the supplied Content argument refers to a valid file path, then the function call is
delegated to the Template-FormatFile function where the matching file's contents are treated
as a template string. Otherwise, the Content itself is interpreted as the template string and
formatted directly by delegating to the Format-TemplateString function.
#>
function Format-TemplateData {
  [CmdletBinding()]
  param(
    [Parameter(Mandatory,ValueFromPipeline)]
    [String] $Content,
    [Parameter(Mandatory,Position=0)]
    [Hashtable] $Data
  )
  process {
    if(Test-Path -Path $Content -PathType Leaf) {
      Write-Information "Formatting file at target path: $Content"
      Format-TemplateFile -Path $Content -Data $Data
    } else {
      Write-Information "Formatting content as a template string."
      Format-TemplateString -Template $Content -Data $Data
    }
  }
}

<#
.SYNOPSIS
Format all template files found in the provided input directory using the Format-TemplateFile
function. Output files are generated at the same relative path in the output directory.
#>
function Format-TemplateDirectory {
  [CmdletBinding()]
  param(
    [Parameter(Mandatory,ValueFromPipeline)]
    [Alias('InDir')]
    [String] $InputDirectory,
    [Parameter(Mandatory,Position=0)]
    [Hashtable] $Data,
    [Parameter(Mandatory,Position=1)]
    [Alias('OutDir')]
    [String] $OutputDirectory,
    [Alias('CR')]
    [Switch] $UseCarriageReturn = $false
  )
  begin {
    function Get-TemplateOutputPath {
      param(
        [Parameter(Mandatory)]
        [FileInfo] $File,
        [Parameter(Mandatory)]
        [Hashtable] $Data,
        [Parameter(Mandatory)]
        [String] $OutputDirectory
      )
      $relative_directory = Resolve-Path -Path $File.DirectoryName -Relative
      if($File.Extension -eq '.template') {
        $filename = $File.Name | Format-TemplateString $Data
        $filename = [Path]::GetFileNameWithoutExtension($filename)
      } else {
        $filename = $File.Name
      }
      $output_path = [Path]::Combine($OutputDirectory,$relative_directory,$filename)
      $output_path = [Path]::GetFullPath($output_path)
      $output_directory = Split-Path -Path $output_path -Parent
      if(-not $(Test-Path -Path $output_directory -PathType Container)) {
        New-Item -Path $output_directory -ItemType Directory | Out-Null
      }
      return $output_path
    }
  }
  process {
    if(-not $(Test-Path -Path $OutputDirectory -PathType Container)) {
      Write-Error -Exception ([System.IO.FileNotFoundException]::new("Directory does not exist: $OutputDirectory")) -ErrorAction Stop
      return
    }
    Push-Location $InputDirectory
    $files = Get-ChildItem -Path '.' -Recurse -File
    foreach($file in $files) {
      $output_path = Get-TemplateOutputPath -File $file -OutputDirectory $OutputDirectory -Data $Data
      if($file.Extension -eq '.template') {
        $content = $file | Format-TemplateFile -Data $Data
      } else {
        $content = $file | Get-Content -Raw
      }
      $content = $content -replace "`r?`n",($UseCarriageReturn ? "`r`n" : "`n")
      Set-Content -Value $content -Path $output_path -NoNewline
    }
    Pop-Location
  }
}

<#
.SYNOPSIS
Enumerate all possible data vectors generated by recursively combining possible array value
in the supplied Data table.
#>
function Get-DataTuples {
  param(
    [Parameter(Mandatory,ValueFromPipeline,Position=0)]
    [Alias('Data')]
    [IDictionary] $DataSpace
  )
  begin {
    Set-Alias -Name "Copy-Tuple" -Value Copy-DeepDictionary -Scope 1
  }
  process {
    if($DataSpace -is [IOrderedDictionary]) {
      $dims = @($DataSpace.Keys)
    } else {
      $dims = @($DataSpace.Keys | Sort)
    }
    $data_tuples = @();
    foreach($dim in $dims) {
      $data_range = $DataSpace[$dim]
      if($data_range -isnot [Array]) {
        continue
      }
      $DataSpace[$dim] = $null
      $sub_tuples = @(Get-DataTuples -DataSpace $DataSpace)
      foreach($sub_tuple in $sub_tuples) {
        foreach($data_value in $data_range) {
          $data_tuple = $(Copy-Tuple $sub_tuple)
          $data_tuple[$dim] = $data_value
          $data_tuples += $data_tuple
        }
      }
    }
    if($data_tuples.Length -eq 0) {
      $data_tuple = $(Copy-Tuple $DataSpace)
      $data_tuples += $data_tuple
    }
    return $data_tuples
  }
}

<#
.SYNOPSIS
Convert nested dictionary data to a flattened representation by concatenating key sequences
and assigning key-value pairs to the root dictionary instance.
#>
function Convert-NestedTuple {
  param(
    [Parameter(Mandatory,ValueFromPipeline)]
    [IDictionary] $Tuple
  )
  process {
    $keys = @($Tuple.Keys)
    foreach($key in $keys) {
      $value = $Tuple[$key]
      if($value -is [IDictionary]) {
        $flat_value = $(Convert-NestedTuple -Tuple $value)
        foreach($subkey in $flat_value.Keys) {
          $subvalue = $flat_value[$subkey]
          $flat_key = "$($key)_$($subkey)"
          $Tuple[$flat_key] = $subvalue`
        }
        $Tuple.Remove($key)
      }
    }
    return $tuple
  }
}

Set-Alias -Name "-fmt" -Value Format-TemplateData
Export-ModuleMember -Alias * -Function `
  Format-TemplateString, Format-TemplateFile, `
  Format-TemplateData, Format-TemplateDirectory, `
  Get-DataTuples, Convert-NestedTuple
