param($installPath, $toolsPath, $package, $project)

# Add reference.
$project.Object.References | Where-Object { $_.Name -eq 'FluentMigrator.Runner' } | ForEach-Object { $_.Remove() }
$project.Object.References.Add("$installPath\..\FluentMigrator.Tools.1.0.6.0\tools\AnyCPU\40\FluentMigrator.Runner.dll")