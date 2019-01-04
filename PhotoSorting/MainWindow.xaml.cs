using System.Collections.ObjectModel;
using System.Windows;
using MahApps.Metro.Controls.Dialogs;
using PhotoSorting.Controller;
using PhotoSorting.Model;

namespace PhotoSorting
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainViewModel MainViewModel { get; } 

        public MainWindow()
        {
            MainViewModel = new MainViewModel(this);
            InitializeComponent();
        }
    }
}
