# all the packages are extracted to this directory. 
# this folder is typically under d:\Octopus\Work in a folder with DateTime signature
$workingDirectory = $OctopusParameters["env:OctopusCalamariWorkingDirectory"]

#list all contents
Get-ChildItem -Path $workingDirectory -Recurse

# Set environment variables
[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12
Write-Host Running on $OctopusParameters["Environment"] environment
Write-Host Setting environment variables

[Environment]::SetEnvironmentVariable("MigrateConnectionString",  $OctopusParameters["MigrationConnectionString"], "Process")

.\build.ps1 -t DbMigration
