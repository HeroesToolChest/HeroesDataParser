# This script is for comparing two json data directory contents for changes

param(
	[Parameter(Mandatory=$true)][int]$previous_build,
	[Parameter(Mandatory=$true)][int]$current_build)

# Paths
$output_path_previous = "F:\heroes\heroes_${previous_build}\data\json"
$output_path_current = "F:\heroes\heroes_${current_build}\data\json"

dotnet heroes-data quick-compare $output_path_previous $output_path_current