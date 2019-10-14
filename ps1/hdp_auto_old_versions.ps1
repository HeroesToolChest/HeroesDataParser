# This script is for old versions
# Be sure to update the paths for variable 1 and 2

param([string]$full_version)

$major,$minor,$rev,$build = $full_version.Split("{.}")

# paths to update
$hots_path = "F:\heroes\heroes_${build}\mods_${build}"
$output_path = "C:\Users\koliva\Source\Repos\heroes-data\heroesdata\${full_version}"

$output_path_data = "${output_path}\data"
$output_path_gamestrings = "${output_path}\gamestrings"

dotnet heroes-data $hots_path --extract-data all --localization all --localized-text --json --output-directory $output_path

"Converting localized text to json..."
dotnet heroes-data localized-json "${output_path}\gamestrings-${build}"
"Converting done."

New-Item -Path $output_path_data -ItemType Directory
New-Item -Path $output_path_gamestrings -ItemType Directory

Copy-Item -Path "${output_path}\json\*.json" -Destination $output_path_data
Copy-Item -Path "${output_path}\gamestrings-${build}\localizedtextjson\*.json" -Destination $output_path_gamestrings

Remove-Item "${output_path}\json" -Recurse
Remove-Item "${output_path}\gamestrings-${build}" -Recurse

"Done."