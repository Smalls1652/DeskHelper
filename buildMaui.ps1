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

$slnFile = Join-Path -Path $PSScriptRoot -ChildPath "DeskHelper.Maui.Blazor.sln"
$srcPath = Join-Path -Path $PSScriptRoot -ChildPath "src\DeskHelper.Maui.Blazor\"

& $bootstrapInstallScript

dotnet restore "$($slnFile)" --force --nologo
dotnet clean "$($slnFile)" --configuration "Release" -noLogo

switch ($_platform) {
    "macOS" {
        dotnet publish "$($srcPath)" --framework "net6.0-maccatalyst" --configuration "Release" --nologo
        break
    }

    Default {
        dotnet publish "$($srcPath)" --framework "net6.0-windows10.0.19041.0" --configuration "Release" --nologo
        break
    }
}