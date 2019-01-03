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
        public MainViewModel MainViewModel { get; } = new MainViewModel();

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void SelectDir_OnClick(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;

            MainViewModel.Directory= dialog.SelectedPath;

            MainViewModel.ClearImageFiles();

            var dlg = await this.ShowProgressAsync("Loading files", "Please wait...");
            dlg.SetIndeterminate();

            var imageReader = new DirectoryImageReader(dialog.SelectedPath);
            var imageFiles = await imageReader.GetImageFilesAsync();
            foreach (var imageFile in imageFiles)
                MainViewModel.AddImageFile(imageFile);

            await dlg.CloseAsync();
        }
    }
}
