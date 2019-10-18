# This script is for newly released patches
# Be sure to update the paths for variables 1, 2, and 5

param([string]$full_version, [string]$heroes_directory, [bool]$ptr = $False)

$major,$minor,$rev,$build = $full_version.Split("{.}")

# paths to update
$output_path = "F:\heroes\heroes_${build}"
$heroes_data_path = "C:\Users\koliva\Source\Repos\heroes-data\heroesdata\${full_version}"
$heroes_data_path_data = "${heroes_data_path}\data"
$heroes_data_path_gamestrings = "${heroes_data_path}\gamestrings"
$heroes_images_path = "C:\Users\koliva\Source\Repos\heroes-images"

if ($ptr)
{
	$output_path = "${output_path}_ptr"
	$heroes_data_path = "${heroes_data_path}_ptr"
}

# extract all
dotnet heroes-data $heroes_directory --extract-data all --extract-images all --localization all --json --localized-text --output-directory $output_path

"Extracting mods data"
dotnet heroes-data extract $heroes_directory -o $output_path

$mods_path = "${output_path}\mods_${build}"

# full data
"Extracting full data"
dotnet heroes-data $mods_path -e all -o "${output_path}\data"

# converting localized text to json
"Converting localized text to json"
dotnet heroes-data localized-json "${output_path}\gamestrings-${build}"
"Converting done."

New-Item -Path $heroes_data_path_data -ItemType Directory
New-Item -Path $heroes_data_path_gamestrings -ItemType Directory

"Copying data"
Copy-Item -Path "${output_path}\json\*.json" -Destination $heroes_data_path_data
Copy-Item -Path "${output_path}\gamestrings-${build}\localizedtextjson\*.json" -Destination $heroes_data_path_gamestrings

"Copying images"
Copy-Item -Path "${output_path}\images\*" -Destination $heroes_images_path -Recurse -Force

"Done."