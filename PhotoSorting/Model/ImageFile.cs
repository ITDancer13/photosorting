using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace PhotoSorting.Model
{
    public class ImageFile : INotifyPropertyChanged
    {
        public bool HasJpegFile => !string.IsNullOrWhiteSpace(JpegPath);
        public Visibility HasJpegFileVisibility => HasJpegFile ? Visibility.Visible : Visibility.Hidden;

        public bool HasRawFile => !string.IsNullOrWhiteSpace(RawPath);
        public Visibility HasRawFileVisibility => HasRawFile ? Visibility.Visible : Visibility.Hidden;

        public bool IsSelected { get; private set; }
        public bool IsFocused { get; set; }

        public string JpegPath { get; }
        public string RawPath { get; }


        public BitmapImage PreviewBitmapImage { get; private set; }

        private ICommand _selectCommand;
        public ICommand SelectCommand
        {
            get
            {
                if (_selectCommand != null)
                    return _selectCommand;

                return _selectCommand = new RelayCommand
                {
                    CanExecutePredicate = p => true,
                    ExecuteAction = p => { IsSelected = !IsSelected; }
                };
            }
        }

        public ImageFile(string jpegPath = null, string rawPath = null)
        {
            JpegPath = jpegPath;
            RawPath = rawPath;



        }

        public void LoadPreviewBitmapImageInNonUiThread()
        {
            if (JpegPath == null) return;

            PreviewBitmapImage = new BitmapImage();
            PreviewBitmapImage.BeginInit();
            PreviewBitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            PreviewBitmapImage.DecodePixelWidth = 500;
            PreviewBitmapImage.UriSource = new Uri(JpegPath);
            PreviewBitmapImage.EndInit();
            PreviewBitmapImage.Freeze();
        }

        public event PropertyChangedEventHandler PropertyChanged;


    }

    public class RelayCommand : ICommand
    {
        public Predicate<object> CanExecutePredicate { private get; set; }
        public Action<object> ExecuteAction { private get; set; }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object parameter)
        {
            return CanExecutePredicate?.Invoke(parameter) ?? false;
        }

        public void Execute(object parameter)
        {
            ExecuteAction?.Invoke(parameter);
        }
    }
}