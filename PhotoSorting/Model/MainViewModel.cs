using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using PhotoSorting.Controller;

namespace PhotoSorting.Model
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly MetroWindow _metroWindow;
        private readonly ObservableCollection<ImageFile> _imagesCollection = new ObservableCollection<ImageFile>();
        public IEnumerable<ImageFile> ImagesCollection => _imagesCollection;
        public string Directory { get; set; }

        public int SelectedImageFiles => _imagesCollection.Count(p => p.SelectionMode != SelectionMode.None);

        public MainViewModel(MetroWindow metroWindow)
        {
            _metroWindow = metroWindow;
            _imagesCollection.CollectionChanged += ImagesCollection_CollectionChanged;
        }

        private void ImagesCollection_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedImageFiles)));
        }

        private void AddImageFile(ImageFile imageFile)
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
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;

            Directory = dialog.SelectedPath;

            ClearImageFiles();

            var dlg = await _metroWindow.ShowProgressAsync("Loading files", "Please wait...");
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
                return _selectDirectoryCommand = new RelayCommand
                {
                    CanExecutePredicate = p => true,
                    ExecuteAction = p => { SelectDirectory(); }
                };
            }
        }

        private void ImageFile_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ImageFile.SelectionMode))
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedImageFiles)));
        }

        public event PropertyChangedEventHandler PropertyChanged;


    }
}
