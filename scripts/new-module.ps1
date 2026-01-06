#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Creates a new module folder structure in the MainApi project.

.DESCRIPTION
    This script creates a new module folder with the standard structure including:
    - ModuleDiExtension.cs with properly named DI registration method
    - Contracts folder
    - Controllers folder
    - Services folder

.PARAMETER ModuleName
    The name of the module to create (e.g., "Billing", "Reporting")

.EXAMPLE
    ./scripts/new-module.ps1 Billing
    Creates a new Billing module in src/Chronos.MainApi/Billing/

.EXAMPLE
    pwsh ./scripts/new-module.ps1 Reporting
    Creates a new Reporting module in src/Chronos.MainApi/Reporting/
#>

param(
    [Parameter(Mandatory = $true, Position = 0)]
    [string]$ModuleName
)

# Validate module name
if ([string]::IsNullOrWhiteSpace($ModuleName)) {
    Write-Error "Module name cannot be empty"
    exit 1
}

# Convert first character to uppercase for proper naming
$ModuleName = $ModuleName.Substring(0, 1).ToUpper() + $ModuleName.Substring(1)

# Define paths
$scriptDir = Split-Path -Parent $PSCommandPath
$rootDir = Split-Path -Parent $scriptDir
$mainApiDir = Join-Path (Join-Path $rootDir "src") "Chronos.MainApi"
$moduleDir = Join-Path $mainApiDir $ModuleName

# Check if module already exists
if (Test-Path $moduleDir) {
    Write-Error "Module '$ModuleName' already exists at: $moduleDir"
    exit 1
}

# Create module directory structure
Write-Host "Creating module directory structure for '$ModuleName'..." -ForegroundColor Green
New-Item -ItemType Directory -Path $moduleDir -Force | Out-Null
New-Item -ItemType Directory -Path (Join-Path $moduleDir "Contracts") -Force | Out-Null
New-Item -ItemType Directory -Path (Join-Path $moduleDir "Controllers") -Force | Out-Null
New-Item -ItemType Directory -Path (Join-Path $moduleDir "Services") -Force | Out-Null

# Create ModuleDiExtension.cs
$diExtensionContent = @"
namespace Chronos.MainApi.$ModuleName;

public static class ModuleDiExtension
{
    public static void Add${ModuleName}Module(this IServiceCollection services, IConfiguration configuration)
    {
        // define your DI here
    }
}
"@

$diExtensionPath = Join-Path $moduleDir "ModuleDiExtension.cs"
Set-Content -Path $diExtensionPath -Value $diExtensionContent -Encoding UTF8

Write-Host "Module '$ModuleName' created successfully!" -ForegroundColor Green
Write-Host "  Location: $moduleDir" -ForegroundColor Cyan
Write-Host ""
Write-Host "Created structure:" -ForegroundColor Yellow
Write-Host "  - $ModuleName/" -ForegroundColor White
Write-Host "    - ModuleDiExtension.cs" -ForegroundColor White
Write-Host "    - Contracts/" -ForegroundColor White
Write-Host "    - Controllers/" -ForegroundColor White
Write-Host "    - Services/" -ForegroundColor White
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Yellow
Write-Host "  1. Add your services, controllers, and contracts" -ForegroundColor White
Write-Host "  2. Register services in ModuleDiExtension.cs" -ForegroundColor White
$functionName = "Add" + $ModuleName + "Module"
Write-Host "  3. Call services.$functionName(configuration) in Program.cs" -ForegroundColor White