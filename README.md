# DeskHelper

A Windows (MAUI and WPF) application to quickly get info about a device by utilizing Blazor Hybrid.

> ⚠️ **Note**
>  
> This is a work-in-progress project and isn't guaranteed to be finished.

## 🗺️ Project layout

### `DeskHelper.Maui.Blazor`

A .NET MAUI WinUI3/macOS Catalyst application utilizing Blazor Hybrid.

### `DeskHelper.Wpf`

A WPF application utilizing Blazor Hybrid.

### `DeskHelper.Blazor`

The core Blazor pages/components that are used in the MAUI and WPF applications. This is separated from the MAUI and WPF projects, so that the same underlying Blazor code works for both of them.

### `DeskHelper.Lib`

A class library that contains models for getting information about the computer.

## 🏗️ Building

> ⚠️ **Note**
>  
> As of May 12th, 2022, [Visual Studio 2022 17.3 preview](https://visualstudio.microsoft.com/vs/preview/) with these two workloads installed:
>  
> * **.NET desktop development**
>   * _Needed for WPF_
> * **Mobile development with .NET**
>   * _Needed for MAUI_

**This section will be filled out later.**

## 🤝 License

This project is licensed under the [MIT License](./LICENSE).
