using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PhotoSorting.Model
{
    public enum SelectionMode { None, Raw, Jpeg, RawAndJpeg }

    public class ImageFileViewModel : DependencyObject, INotifyPropertyChanged
    {
        private static readonly SolidColorBrush TransparentBorderBrush = new SolidColorBrush(Colors.Transparent);
        private static readonly SolidColorBrush GreenBorderBrush = new SolidColorBrush(Colors.YellowGreen);
        private static readonly SolidColorBrush GrayBorderBrush = new SolidColorBrush(Colors.Gray);
        private static readonly SolidColorBrush RedBorderBrush = new SolidColorBrush(Colors.Red);

        public bool HasJpegFile => !string.IsNullOrWhiteSpace(JpegPath);
        public Visibility HasJpegFileVisibility => HasJpegFile ? Visibility.Visible : Visibility.Hidden;

        public bool HasRawFile => !string.IsNullOrWhiteSpace(RawPath);
        public Visibility HasRawFileVisibility => HasRawFile ? Visibility.Visible : Visibility.Hidden;

        public bool IsFocused { get; set; }
        public SelectionMode SelectionMode { get; private set; }

        public bool IsSelected => SelectionMode != SelectionMode.None;
        public Visibility SelectionModeTextVisibility => IsSelected ? Visibility.Visible : Visibility.Hidden;
        public string SelectionModeText
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

        private FileInfo JpegFileInfo { get; }
        private FileInfo RawFileInfo { get; }

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
                        IsFocused = true;
                        switch (SelectionMode)
                        {
                            case SelectionMode.None:
                                SelectionMode = HasRawFile ? SelectionMode.Raw : SelectionMode.Jpeg;
                                break;
                            case SelectionMode.Raw:
                                SelectionMode = HasJpegFile ? SelectionMode.RawAndJpeg : SelectionMode.None;
                                break;
                            case SelectionMode.RawAndJpeg:
                                SelectionMode = SelectionMode.Jpeg;
                                break;
                            case SelectionMode.Jpeg:
                                SelectionMode = SelectionMode.None;
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                };
            }
        }

        private ICommand _gotFocusCommand;
        public ICommand GotFocusCommand
        {
            get
            {
                if (_gotFocusCommand != null)
                    return _gotFocusCommand;

                return _gotFocusCommand = new RelayCommand
                {
                    CanExecutePredicate = p => true,
                    ExecuteAction = p =>
                    {
                        IsFocused = true;
                    }
                };
            }
        }

        private ICommand _lostFocusCommand;
        public ICommand LostFocusCommand
        {
            get
            {
                if (_lostFocusCommand != null)
                    return _lostFocusCommand;

                return _lostFocusCommand = new RelayCommand
                {
                    CanExecutePredicate = p => true,
                    ExecuteAction = p =>
                    {
                        IsFocused = false;
                    }
                };
            }
        }

        public int SelectedFilesCount
        {
            get
            {
                switch (SelectionMode)
                {
                    case SelectionMode.None:
                        return 0;
                    case SelectionMode.Raw:
                        return 1;
                    case SelectionMode.Jpeg:
                        return 1;
                    case SelectionMode.RawAndJpeg:
                        return 2;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public long SelectedFilesSize
        {
            get
            {
                switch (SelectionMode)
                {
                    case SelectionMode.None:
                        return 0;
                    case SelectionMode.Raw:
                        return RawFileInfo.Length;
                    case SelectionMode.Jpeg:
                        return JpegFileInfo.Length;
                    case SelectionMode.RawAndJpeg:
                        return RawFileInfo.Length + JpegFileInfo.Length;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public ImageFileViewModel(string jpegPath = null, string rawPath = null)
        {
            JpegPath = jpegPath;
            RawPath = rawPath;

            if (JpegPath != null)
                JpegFileInfo = new FileInfo(JpegPath);

            if (RawPath != null)
                RawFileInfo = new FileInfo(RawPath);

        }

        public void LoadInfos()
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

        public BitmapImage JpegImage
        {
            get
            {
                var img = new BitmapImage();
                img.BeginInit();
                img.DecodePixelWidth = 1920;
                img.UriSource = new Uri(JpegPath);
                img.EndInit();

                return img;
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;


    }
}