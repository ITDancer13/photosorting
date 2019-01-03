using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PhotoSorting.Model
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly ObservableCollection<ImageFile> _imagesCollection = new ObservableCollection<ImageFile>();
        public IEnumerable<ImageFile> ImagesCollection => _imagesCollection;
        public string Directory { get; set; }

        public int SelectedImageFiles => _imagesCollection.Count(p => p.SelectionMode != SelectionMode.None);


        public MainViewModel()
        {
            _imagesCollection.CollectionChanged += ImagesCollection_CollectionChanged;
        }

        private void ImagesCollection_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedImageFiles)));
        }

        public void AddImageFile(ImageFile imageFile)
        {
            imageFile.PropertyChanged += ImageFile_PropertyChanged;
            _imagesCollection.Add(imageFile);
        }

        

        public void ClearImageFiles()
        {
            foreach (var imageFile in _imagesCollection)
                imageFile.PropertyChanged -= ImageFile_PropertyChanged;

            _imagesCollection.Clear();
        }

        private void ImageFile_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ImageFile.SelectionMode))
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedImageFiles)));
        }

        public event PropertyChangedEventHandler PropertyChanged;


    }
}
