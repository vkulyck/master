using namespace System.Collections
using namespace System.Collections.Generic
using namespace System.Collections.Specialized

function ConvertTo-Hashtable {
    [CmdletBinding()]
    [OutputType([OrderedDictionary])]
    param (
        [Parameter(ValueFromPipeline)]
        $InputObject
    )
    process {
        ## Return null if the input is null. This can happen when calling the function
        ## recursively and a property is null
        if ($null -eq $InputObject) {
            return $null
        }
        ## Check if the input is an array or collection. If so, we also need to convert
        ## those types into hash tables as well. This function will convert all child
        ## objects into hash tables (if applicable)
        if ($InputObject -is [IEnumerable] -and $InputObject -isnot [string]) {
            $collection = @(
                foreach ($object in $InputObject) {
                    ConvertTo-Hashtable -InputObject $object
                }
            )
            ## Return the array but don't enumerate it because the object may be pretty complex
            Write-Output -InputObject $collection -NoEnumerate
        } elseif ($InputObject -is [PSObject]) { ## If the object has properties that need enumeration
            ## Convert it to its own hash table and return it
            $hash = [ordered]@{}
            foreach ($property in $InputObject.PSObject.Properties) {
                $hash[$property.Name] = ConvertTo-Hashtable -InputObject $property.Value
            }
            $hash
        } else {
            ## If the object isn't an array, collection, or other object, it's already a hash table
            ## So just return it.
            $InputObject
        }
    }
}

function Convert-JsonToHashtable([Parameter(Mandatory, ValueFromPipeline)][String] $json) {
    $json | ConvertFrom-Json | ConvertTo-Hashtable
}

function Copy-DeepDictionary {
    [CmdletBinding()]
    [OutputType([OrderedDictionary])]
    param (
        [Parameter(ValueFromPipeline,Position=0)]
        $Dict
    )
    process {
        ConvertTo-Json -InputObject $Dict -Depth 100 | Convert-JsonToHashtable
    }
}

function Format-Json([Parameter(Mandatory, ValueFromPipeline)][String] $json) {
    $indent = 0;
    ($json -Split "`n" | % {
        if ($_ -match '[\}\]]\s*,?\s*$') {
            # This line ends with ] or }, decrement the indentation level
            $indent--
        }
        $line = ('    ' * $indent) + $($_.TrimStart() -replace '":  (["{[])','": $1' -replace ':  ',': ')
        if ($_ -match '[\{\[]\s*$') {
            # This line ends with [ or {, increment the indentation level
            $indent++
        }
        $line
    }) -Join "`n"
}

# https://www.powershellgallery.com/packages/PSSharedGoods/0.0.31/Content/Public%5CObjects%5CMerge-Objects.ps1
function Merge-Objects {
    [CmdletBinding()]
    param (
        [OrderedDictionary] $Original,
        [OrderedDictionary] $Update,
        [Parameter(HelpMessage="By default, collisions are resolved by recursively merging the colliding key-values. " +
                                "This option enables 'Terminal Mode', where recursive merges are avoided by simply " +
                                "taking the Update object's key value at each collision."
        )]
        [Alias('t', 'NoRecurse')]
        [Switch]$Terminal = $false
    )
    $Merged = [ordered]@{}
    $OriginalOuterKeys = $Original.Keys | Where-Object { $Update.Keys -NotContains $_ }
    foreach($key in $OriginalOuterKeys) {
        $Merged[$key] = $Original[$key]
    }
    $UpdateOuterKeys = $Update.Keys | Where-Object { $Original.Keys -NotContains $_ }
    foreach ($key in $UpdateOuterKeys) {
        $Merged[$key] = $Update[$key]
    }
    $InnerKeys = $Original.Keys | Where-Object { $Update.Keys -Contains $_ }
    foreach($key in $InnerKeys) {
        $OriginalValue = $original[$key]
        $UpdateValue = $Update[$key]
        if($null -eq $OriginalValue) {
            # If the Original value is null, take the Update value
            $Merged[$key] = $UpdateValue
        } elseif($null -eq $UpdateValue) {
            # If the Update value is null, take the Original value
            $Merged[$key] = $OriginalValue
        } elseif($OriginalValue -is [OrderedDictionary] -And $UpdateValue -is [OrderedDictionary]) {
            # If both values are child objects, recurse
            if($Terminal) {
                $Merged[$key] = $UpdateValue
            } else {
                $Merged[$key] = $(Merge-Objects -Original $OriginalValue -Update $UpdateValue)
            }
        } else {
            # If no recursion is possible then just replace the original value with the update value
            $Merged[$key] = $UpdateValue
        }
    }
    return $Merged
}

function Test-HashtableConvertible {
    param (
        [Parameter(ValueFromPipeline=$true, Mandatory=$true, Position=0)]
        $Object
    )
    process {
        if($Object -is [Dictionary[[String],[String]]]) {
            return $true
        }
        if($Object -is [OrderedDictionary]) {
            return $true
        }
        if($Object -is [Hashtable]) {
            return $true
        }
        return $false
    }
}

function Compare-Hashtables {
    [CmdletBinding()]
    param (
        $Original,
        $Update,
        [Parameter(HelpMessage="By default, collisions are resolved by recursively merging the colliding key-values. " +
                                "This option enables 'Terminal Mode', where recursive merges are avoided by simply " +
                                "taking the Update object's key value at each collision."
        )]
        [Alias('t', 'NoRecurse')]
        [Switch]$Terminal = $false,
        [Parameter(HelpMessage="The CurrentPath is the sequence of keys that were accessed within each ancestor object " +
                                "to arrive at the current key-value node. This value will always be an empty string " +
                                "at the root invocation, and period-delimited sequence of keys for all recursive " +
                                "invocations."
        )]
        [String]$CurrentPath = ''
    )
    if(-Not $(Test-HashtableConvertible $Original)) {
        Write-Error "Parameter 'Original' must be Hashtable-Convertible."
        exit
    }
    if(-Not $(Test-HashtableConvertible $Update)) {
        Write-Error "Parameter 'Update' must be Hashtable-Convertible."
        exit
    }
    if($Original -isnot [OrderedDictionary]) {
        $Original = [Hashtable]$Original
    }
    if($Update -isnot [OrderedDictionary]) {
        $Update = [Hashtable]$Update
    }
    if([String]::IsNullOrWhiteSpace($CurrentPath)) {
        $CurrentPath = "<Root>"
        $AtRootInvocation = $true
    } else {
        $AtRootInvocation = $false
    }
    $OriginalOuterKeys = $Original.Keys | Where-Object { $Update.Keys -NotContains $_ }
    if($OriginalOuterKeys.Length -gt 0) {
        Write-Host "`t$CurrentPath`: [Missing Keys: $OriginalOuterKeys]"
    }
    $UpdateOuterKeys = $Update.Keys | Where-Object { $Original.Keys -NotContains $_ }
    if($UpdateOuterKeys.Length -gt 0) {
        Write-Host "`t$CurrentPath`: [Additional Keys: $UpdateOuterKeys]"
    }
    foreach($key in $Original.Keys) {
        if($Update.Keys -notcontains $key) {
            continue
        }
        if($AtRootInvocation) {
            $ChildPath = "$key"
        } else {
            $ChildPath = "$CurrentPath.$key"
        }
        $OriginalValue = $original[$key]
        $UpdateValue = $Update[$key]
        if($(Test-HashtableConvertible $OriginalValue) -And $(Test-HashtableConvertible $UpdateValue)) {
            # If both values are child objects, recurse
            if($Terminal) {
                if($OriginalValue -ne $UpdateValue) {
                    Write-Host "`t$ChildPath`: [Reference Diff]"
                }
            } else {
                Compare-Hashtables -Original $OriginalValue -Update $UpdateValue -CurrentPath $ChildPath
            }
        } else {
            if($OriginalValue -ne $UpdateValue) {
                Write-Host "`t$ChildPath`: [Original]'$OriginalValue' != [Update]'$UpdateValue'"
            }
        }
    }
    return $Merged
}

function Set-JsonPathValue {
    param(
        [String]$Path,
        [OrderedDictionary]$Value,
        [OrderedDictionary]$Object
    )
    $pathProperties = $Path.Split(".")
    [Array]::Reverse($pathProperties);
    foreach($prop in $pathProperties) {
        $Value = [ordered]@{ $prop = $Value; }
    }
    $merged = $(Merge-Objects $Object $Value)
    return $merged;
}

function Write-JsonConfig {
    param(
        [Parameter(Mandatory=$true, ValueFromPipeline=$true)]
        $ConfigData,
        [Parameter(Mandatory=$true, Position=0)]
        [String]$ConfigPath
    )
    process {
        $ConfigData | ConvertTo-Json -Depth 100 | Format-Json | Set-Content $ConfigPath
    }
}


function Read-JsonConfig {
    param(
        [Parameter(Mandatory=$true, Position=0)]
        [String]$ConfigPath
    )
    $ConfigData = $(Get-Content $ConfigPath | ConvertFrom-Json | ConvertTo-Hashtable)
    Write-Output $ConfigData
}

function Format-RedisJson {
    param(
        [Parameter(Mandatory,ValueFromPipeline)]
        [String]$ConfigJson
    )
    process {
        $($ConfigJson | Format-Json) -replace '"','\"' -replace "`r`n","`n"
    }
}

function ConvertTo-RedisJson {
    param(
        [Parameter(Mandatory=$true, ValueFromPipeline=$true)]
        $ConfigData
    )
    process {
        $ConfigData | ConvertTo-Json -Depth 100 | Format-Json | Format-RedisJson
    }
}

Export-ModuleMember -Function *
