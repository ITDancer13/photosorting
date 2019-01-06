using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Input;
using MahApps.Metro.Controls.Dialogs;
using PhotoSorting.Controller;

namespace PhotoSorting.Model
{
    public class MainViewModel : INotifyPropertyChanged
    {
        // private readonly MetroWindow _metroWindow;
        private readonly ObservableCollection<ImageFileViewModel> _imagesCollection = new ObservableCollection<ImageFileViewModel>();
        public IEnumerable<ImageFileViewModel> ImagesCollection => _imagesCollection;
        public string Directory { get; set; }

        public int SelectedFilesCount => _imagesCollection.Sum(p => p.SelectedFilesCount);
        public string SelectedFilesSizeMb => (_imagesCollection.Sum(p => p.SelectedFilesSize) / 1024.0 / 1024.0).ToString("f2");

        public MainViewModel()
        {
            _imagesCollection.CollectionChanged += ImagesCollection_CollectionChanged;
        }

        private void ImagesCollection_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ImageFileViewModel.SelectionMode)));
        }

        private void AddImageFile(ImageFileViewModel imageFile)
        {
            imageFile.PropertyChanged += ImageFile_PropertyChanged;
            _imagesCollection.Add(imageFile);
        }


        private void ClearImageFiles()
        {
            foreach (var imageFile in _imagesCollection)
                imageFile.PropertyChanged -= ImageFile_PropertyChanged;

            _imagesCollection.Clear();
        }

        private async void SelectDirectory()
        {
            var dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() != DialogResult.OK) return;

            Directory = dialog.SelectedPath;

            ClearImageFiles();

            var dlg = await MainWindow.Instance.ShowProgressAsync("Loading files", "Please wait...");
            dlg.SetIndeterminate();

            var imageReader = new DirectoryImageReader(dialog.SelectedPath);
            var imageFiles = await imageReader.GetImageFilesAsync();
            foreach (var imageFile in imageFiles)
                AddImageFile(imageFile);

            await dlg.CloseAsync();
        }

        private ICommand _selectDirectoryCommand;
        public ICommand SelectDirectoryCommand
        {
            get
            {
                if (_selectDirectoryCommand != null)
                    return _selectDirectoryCommand;

                return _selectDirectoryCommand = new RelayCommand
                {
                    CanExecutePredicate = p => true,
                    ExecuteAction = p => { SelectDirectory(); }
                };
            }
        }

        public ImageFileViewModel SelectedImage { get; set; }

        private void ImageFile_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ImageFileViewModel.SelectionMode))
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedFilesCount)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedFilesSizeMb)));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
