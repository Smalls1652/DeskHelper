using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace DeskHelper.Wpf;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        ServiceCollection svcCollection = new();
        svcCollection.AddBlazorWebView();
        Resources.Add("services", svcCollection.BuildServiceProvider());

        InitializeComponent();
    }
}