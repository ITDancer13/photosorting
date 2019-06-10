using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.EntityFrameworkCore;
using PhotoSorting.Controller;
using PhotoSorting.Entities;

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

        public async Task InitializeAsync()
        {
            using (var dbContext = new DatabaseContext())
            {
                var settings = await dbContext.Settings.FirstAsync();
                await LoadDirectory(settings.DirectoryFilePath);
            }
        }

        private void ImagesCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
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

        private async Task SelectDirectory()
        {
            var dialog = new FolderBrowserDialog { SelectedPath = Directory };
            if (dialog.ShowDialog() != DialogResult.OK)
                return;

            using (var dbContext = new DatabaseContext())
            {
                dbContext.Settings.First().DirectoryFilePath = dialog.SelectedPath;
                await dbContext.SaveChangesAsync();
            }

            await LoadDirectory(dialog.SelectedPath);
        }

        private async Task LoadDirectory(string path)
        {
            if (string.IsNullOrEmpty(path) || !System.IO.Directory.Exists(path))
                return;

            Directory = path;

            ClearImageFiles();

            var dlg = await MainWindow.Instance.ShowProgressAsync("Loading files", "Please wait...");
            dlg.SetIndeterminate();

            var imageReader = new DirectoryImageReader(path);
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

        private ICommand _moveSelectedFiles;
        public ICommand MoveSelectedFiles
        {
            get
            {
                if (_moveSelectedFiles != null)
                    return _moveSelectedFiles;

                return _moveSelectedFiles = new RelayCommand
                {
                    CanExecutePredicate = p => true,
                    ExecuteAction = p =>
                    {
                        var dialog = new FolderBrowserDialog { SelectedPath = Directory, Description = @"Select target folder" };
                        if (dialog.ShowDialog() != DialogResult.OK)
                            return;

                        var jpegs = ImagesCollection.Where(i => i.SelectionMode == SelectionMode.Jpeg || i.SelectionMode == SelectionMode.RawAndJpeg).Select(i => i.JpegPath);
                        var raws = ImagesCollection.Where(i => i.SelectionMode == SelectionMode.Raw || i.SelectionMode == SelectionMode.RawAndJpeg).Select(i => i.RawPath);
                        var files = jpegs.Concat(raws).OrderBy(f => f).ToList();

                        foreach (var file in files)
                        {
                            var filename = Path.GetFileName(file);
                            if(string.IsNullOrEmpty(filename))
                                continue;

                            var dest = System.IO.Path.Combine(dialog.SelectedPath, filename);
                            File.Move(file, dest);
                        }

                        LoadDirectory(Directory);
                    }
                };
            }
        }



        private void ImageFile_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // ReSharper disable once InvertIf
            if (e.PropertyName == nameof(ImageFileViewModel.SelectionMode))
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedFilesCount)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedFilesSizeMb)));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
