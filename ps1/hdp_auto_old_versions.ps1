# This script is for old versions
# Be sure to update the PATHS

param(
	[Parameter(Mandatory=$true)][string]$full_version,
	[bool]$ptr = $False)
	
$major,$minor,$rev,$build = $full_version.Split(".")

# PATHS ##########################
### PATH TO UPDATE ###
$hots_path = "F:\heroes\heroes_${build}\mods_${build}"

### PATH TO UPDATE ###
$output_path = "C:\Users\koliva\Source\Repos\heroes-data\heroesdata\${full_version}"

##################################

# if ptr update the paths to _ptr
if ($ptr)
{
	$hots_path = "F:\heroes\heroes_${build}_ptr\mods_${build}"
	$output_path = "${output_path}_ptr"
}

$hdp_json_file = "${output_path}\.hdp.json"

# Powershell version check
if ($PSVersionTable.PSVersion.Major -lt 6)
{
	"Must use PowerShell version 6 or higher"
	exit
}

$output_path_data = "${output_path}\data"
$output_path_gamestrings = "${output_path}\gamestrings"

# validate directories
if (!(Test-Path $hots_path -PathType Container))
{
	"hots_path is invalid"
	exit
}
if (!(Test-Path $output_path -PathType Container))
{
	"output_path is invalid"
	exit
}
if (!(Test-Path $hdp_json_file -PathType Leaf))
{
	"hdp_json_file does not exist"
	exit
}

# ensure output directories exist
New-Item -ItemType Directory -Force -Path $output_path_data | Out-Null
New-Item -ItemType Directory -Force -Path $output_path_gamestrings | Out-Null

# get version of hdp
$version = [string](dotnet heroes-data --version)
$x = $version -match "Heroes Data Parser (?<content>.*)"
$v_num = $matches['content'].trim("(").trim(")")

# extract
dotnet heroes-data $hots_path --extract-data all --localization all --localized-text --json --output-directory $output_path

"Converting localized text to json..."
dotnet heroes-data localized-json "${output_path}\gamestrings-${build}"
"Converting done."

"Copying..."
Copy-Item -Path "${output_path}\json\*.json" -Destination $output_path_data
Copy-Item -Path "${output_path}\gamestrings-${build}\localizedtextjson\*.json" -Destination $output_path_gamestrings

"Removing old..."
Remove-Item "${output_path}\json" -Recurse
Remove-Item "${output_path}\gamestrings-${build}" -Recurse

"Update hdp version"
$hdp_json = get-content $hdp_json_file
$hdp_property = $hdp_json | select-string -pattern "hdp"
$hdp_json.replace($hdp_property, "  `"hdp`": `"${v_num}`"") | out-file $hdp_json_file

"Done."