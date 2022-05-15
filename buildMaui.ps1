[CmdletBinding()]
param(
)

$_platform = $null
switch ([System.OperatingSystem]::IsMacOS()) {
    $true {
        $_platform = "macOS"
        break
    }

    Default {
        $_platform = "Windows"
        break
    }
}
Write-Verbose "Target platform: $($_platform)"

$bootstrapInstallScript = Join-Path -Path $PSScriptRoot -ChildPath "copyBootstrapFiles.ps1"
$buildOutputPath = Join-Path -Path $PSScriptRoot -ChildPath "build\"

$slnFile = Join-Path -Path $PSScriptRoot -ChildPath "DeskHelper.Maui.Blazor.sln"
$srcPath = Join-Path -Path $PSScriptRoot -ChildPath "src\DeskHelper.Maui.Blazor\"

if (Test-Path -Path $buildOutputPath) {
    Remove-Item -Path $buildOutputPath -Force -Recurse
}

$null = New-Item -Path $buildOutputPath -ItemType "Directory"

& $bootstrapInstallScript

dotnet restore "$($slnFile)" --force --nologo
dotnet clean "$($slnFile)" --configuration "Release" -noLogo

switch ($_platform) {
    "macOS" {
        dotnet build "$($srcPath)" --framework "net6.0-maccatalyst" --configuration "Release" --nologo --output "$($buildOutputPath)" /p:"CreatePackage=false"

        $nonAppFiles = Get-ChildItem -Path $buildOutputPath | Where-Object { $PSItem.Name -ne "DeskHelper.Maui.Blazor.app" }
        foreach ($item in $nonAppFiles) {
            Remove-Item -Path $item.FullName -Recurse -Force
        }
        break
    }

    Default {
        dotnet publish "$($srcPath)" --framework "net6.0-windows10.0.19041.0" --configuration "Release" --nologo

        $appPackagesPath = Join-Path -Path $srcPath -ChildPath "bin\Release\net6.0-windows10.0.19041.0\win10-x64\AppPackages"
        $appPackagesItem = Get-ChildItem -Path $appPackagesPath | Sort-Object -Property "LastWriteTime" | Select-Object -Last 1

        Copy-Item -Path $appPackagesItem -Destination $buildOutputPath -Recurse
        break
    }
}