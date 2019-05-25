using System.Windows;
using PhotoSorting.Model;

namespace PhotoSorting
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public static MainWindow Instance { get; private set; }

        private bool _loaded;

        public MainWindow()
        {
            InitializeComponent();
            Instance = this;

        }

        private async void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (_loaded)
                return;

            _loaded = true;

            var mainViewModel = (MainViewModel) FindResource("MainViewModel");
            await mainViewModel.InitializeAsync();
        }
    }
}
