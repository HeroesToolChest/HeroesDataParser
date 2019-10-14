# This script is for comparing two json data directory contents for changes

param([int]$previous_build, [int]$current_build)

# Paths
$output_path_previous = "F:\heroes\heroes_${previous_build}\data\json"
$output_path_current = "F:\heroes\heroes_${current_build}\data\json"

dotnet heroes-data quick-compare $output_path_previous $output_path_current