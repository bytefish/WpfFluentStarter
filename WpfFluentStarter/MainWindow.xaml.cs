// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using WpfFluentStarter.ViewModels;
using System.ComponentModel;
using System.Windows;

namespace WpfFluentStarter;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// The code-behind is kept minimal, handling only View-specific tasks like printing.
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    // Event handler for Window.Closing (triggered by X button, Alt+F4, or Close())
    private void Window_Closing(object sender, CancelEventArgs e)
    {
        // Access the ViewModel
        if (DataContext is MainWindowViewModel vm)
        {
            // Ask the ViewModel if we can close
            bool canClose = vm.CanClose();

            // If ViewModel says no (user clicked Cancel), cancel the event
            if (!canClose)
            {
                e.Cancel = true;
            }
        }
    }

    private void MinimizeButton_Click(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }

    private void MaximizeRestoreButton_Click(object sender, RoutedEventArgs e)
    {
        if (WindowState == WindowState.Normal)
        {
            WindowState = WindowState.Maximized;
        }
        else
        {
            WindowState = WindowState.Normal;
        }
    }
}