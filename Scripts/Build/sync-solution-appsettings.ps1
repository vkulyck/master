param(
  [string]$SolutionDirectory,
  [string]$TargetDirectory,
  [string]$ProjectDirectory
)

# Setup environment
$SolutionDataDirectory = Join-Path -Path $SolutionDirectory -ChildPath 'SolutionData'
$SolutionSettingsDirectory = Join-Path -Path $SolutionDataDirectory -ChildPath 'Config'
$TargetSettingsDirectory = Join-Path -Path $TargetDirectory -ChildPath 'Settings'
$ProjectSettingsDirectory = Join-Path -Path $ProjectDirectory -ChildPath 'Settings'

# Create copy targets
New-Item -ItemType Directory -Force -Path $ProjectSettingsDirectory -ErrorAction Ignore | Out-Null
New-Item -ItemType Directory -Force -Path $TargetSettingsDirectory -ErrorAction Ignore | Out-Null

# Perform copy actions
Copy-Item -Path $SolutionSettingsDirectory\* -Destination $ProjectSettingsDirectory -Recurse -Exclude '.gitignore' -ErrorAction SilentlyContinue | Out-Null
Copy-Item -Path $SolutionSettingsDirectory\* -Destination $TargetSettingsDirectory -Recurse -Exclude '.gitignore'  -ErrorAction SilentlyContinue | Out-Null