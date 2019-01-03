using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PhotoSorting.Model
{
    public enum SelectionMode { None, Raw, Jpeg, RawAndJpeg }

    public class ImageFile : INotifyPropertyChanged
    {
        private static readonly SolidColorBrush TransparentBorderBrush = new SolidColorBrush(Colors.Transparent);
        private static readonly SolidColorBrush GreenBorderBrush = new SolidColorBrush(Colors.YellowGreen);
        private static readonly SolidColorBrush GrayBorderBrush = new SolidColorBrush(Colors.Gray);
        private static readonly SolidColorBrush RedBorderBrush = new SolidColorBrush(Colors.Red);

        public bool HasJpegFile => !string.IsNullOrWhiteSpace(JpegPath);
        public Visibility HasJpegFileVisibility => HasJpegFile ? Visibility.Visible : Visibility.Hidden;

        public bool HasRawFile => !string.IsNullOrWhiteSpace(RawPath);
        public Visibility HasRawFileVisibility => HasRawFile ? Visibility.Visible : Visibility.Hidden;

        public SelectionMode SelectionMode { get; private set; }

        public bool IsSelected => SelectionMode != SelectionMode.None;
        public Visibility SelectionModeTextVisibility => IsSelected ? Visibility.Visible : Visibility.Hidden;
        public  string SelectionModeText 
        {
            get
            {
                switch (SelectionMode)
                {
                    case SelectionMode.None:
                        return string.Empty;
                    case SelectionMode.Raw:
                        return "raw selected";
                    case SelectionMode.Jpeg:
                        return "jpeg selected";
                    case SelectionMode.RawAndJpeg:
                        return "raw & jpeg selected";
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
        public SolidColorBrush BorderBrush
        {
            get
            {
                switch (SelectionMode)
                {
                    case SelectionMode.None:
                        return TransparentBorderBrush;
                    case SelectionMode.Raw:
                        return GreenBorderBrush;
                    case SelectionMode.RawAndJpeg:
                        return RedBorderBrush;
                    case SelectionMode.Jpeg:
                        return GrayBorderBrush;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

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
                    ExecuteAction = p =>
                    {
                        switch (SelectionMode)
                        {
                            case SelectionMode.None:
                                SelectionMode = HasRawFile ? SelectionMode.Raw : SelectionMode.Jpeg;
                                break;
                            case SelectionMode.Raw:
                                SelectionMode = HasJpegFile ? SelectionMode.Jpeg : SelectionMode.None;
                                break;
                            case SelectionMode.Jpeg:
                                SelectionMode = HasRawFile ? SelectionMode.RawAndJpeg : SelectionMode.None;
                                break;
                            case SelectionMode.RawAndJpeg:
                                SelectionMode = SelectionMode.None;
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
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
}