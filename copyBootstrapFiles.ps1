[CmdletBinding()]
param()

$scriptRoot = $PSScriptRoot

$writeInfoSplat = @{
    "InformationAction" = "Continue";
}

$nodeModulesPath = Join-Path -Path $scriptRoot -ChildPath "node_modules\"
if (Test-Path -Path $nodeModulesPath) {
    Remove-Item -Path $nodeModulesPath -Recurse -Force
}

Start-Process -FilePath "npm" -ArgumentList @("install") -Wait -NoNewWindow -WorkingDirectory $scriptRoot

foreach ($srcPathItem in @("src\DeskHelper.Wpf", "src\DeskHelper.Maui.Blazor")) {
    $bootstrapCssPath = Join-Path -Path $scriptRoot -ChildPath "node_modules\bootstrap\dist\css\bootstrap.min.css"
    $bootstrapCssMapPath = Join-Path -Path $scriptRoot -ChildPath "node_modules\bootstrap\dist\css\bootstrap.min.css.map"
    $bootstrapOutPath = Join-Path -Path $scriptRoot -ChildPath "$($srcPathItem)\wwwroot\css\bootstrap\"

    $bootstrapIconsCssPath = Join-Path -Path $scriptRoot -ChildPath "node_modules\bootstrap-icons\font\bootstrap-icons.css"
    $bootstrapIconsFontDirPath = Join-Path -Path $scriptRoot -ChildPath "node_modules\bootstrap-icons\font\fonts\"
    $bootstrapIconsOutPath = Join-Path -Path $scriptRoot -ChildPath "$($srcPathItem)\wwwroot\css\bootstrap-icons\"

    if (!(Test-Path -Path $bootstrapOutPath)) {
        $null = New-Item -Path $bootstrapOutPath -ItemType "Directory"
    }

    foreach ($fileItem in (Get-ChildItem -Path $bootstrapOutPath)) {
        Write-Information @writeInfoSplat -MessageData "`t| Removing '$($fileItem.Name)'"
        Remove-Item -Path $fileItem.FullName -Force
    }

    Write-Information @writeInfoSplat -MessageData "`t| bootstrap.min.css-> $($bootstrapOutPath)"
    Copy-Item -Path $bootstrapCssPath -Destination $bootstrapOutPath -ErrorAction "Stop"

    Write-Information @writeInfoSplat -MessageData "`t| bootstrap.min.css.map-> $($bootstrapOutPath)"
    Copy-Item -Path $bootstrapCssMapPath -Destination $bootstrapOutPath -ErrorAction "Stop"

    if (!(Test-Path -Path $bootstrapIconsOutPath)) {
        $null = New-Item -Path $bootstrapIconsOutPath -ItemType "Directory"
    }

    foreach ($fileItem in (Get-ChildItem -Path $bootstrapIconsOutPath)) {
        Write-Information @writeInfoSplat -MessageData "`t| Removing '$($fileItem.Name)'"
        Remove-Item -Path $fileItem.FullName -Force -Recurse
    }

    Write-Information @writeInfoSplat -MessageData "`t| bootstrap-icons.css-> $($bootstrapIconsOutPath)"
    Copy-Item -Path $bootstrapIconsCssPath -Destination $bootstrapIconsOutPath -ErrorAction "Stop"

    Write-Information @writeInfoSplat -MessageData "`t| fonts\-> $($bootstrapIconsOutPath)"
    Copy-Item -Path $bootstrapIconsFontDirPath -Destination $bootstrapIconsOutPath -Recurse -ErrorAction "Stop"
}