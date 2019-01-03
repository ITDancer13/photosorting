using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using PhotoSorting.Controller;
using PhotoSorting.Model;
using Image = System.Drawing.Image;

namespace PhotoSorting
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public ObservableCollection<ImageFile> TestCollection { get; } = new ObservableCollection<ImageFile>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;

            var imageReader = new DirectoryImageReader(dialog.SelectedPath);
            var imageFiles = await imageReader.GetImageFilesAsync();
            foreach (var imageFile in imageFiles)
                TestCollection.Add(imageFile);
        }
    }
}
