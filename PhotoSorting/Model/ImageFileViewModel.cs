using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
// ReSharper disable MemberCanBePrivate.Global

namespace PhotoSorting.Model
{
    public class ImageFileViewModel : DependencyObject, INotifyPropertyChanged
    {
        private static readonly SolidColorBrush TransparentBorderBrush = new SolidColorBrush(Colors.Transparent);
        private static readonly SolidColorBrush YellowBorderBrush = new SolidColorBrush(Colors.Yellow);
        private static readonly SolidColorBrush GrayBorderBrush = new SolidColorBrush(Colors.Gray);
        private static readonly SolidColorBrush RedBorderBrush = new SolidColorBrush(Colors.Red);

        public bool HasJpegFile => !string.IsNullOrWhiteSpace(JpegPath);
        public Visibility HasJpegFileVisibility => HasJpegFile ? Visibility.Visible : Visibility.Hidden;

        public bool HasRawFile => !string.IsNullOrWhiteSpace(RawPath);
        public Visibility HasRawFileVisibility => HasRawFile ? Visibility.Visible : Visibility.Hidden;

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
                        return YellowBorderBrush;
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

        private Rotation _imageRotation = Rotation.Rotate0;

        private ICommand _rotateCommand;
        public ICommand RotateCommand
        {
            get
            {
                if (_rotateCommand != null)
                    return _rotateCommand;

                return _rotateCommand = new RelayCommand
                {
                    CanExecutePredicate = p => true,
                    ExecuteAction = p =>
                    {
                        switch (_imageRotation)
                        {
                            case Rotation.Rotate0:
                                SetRotation(Rotation.Rotate90);
                                break;
                            case Rotation.Rotate90:
                                SetRotation(Rotation.Rotate180);
                                break;
                            case Rotation.Rotate180:
                                SetRotation(Rotation.Rotate270);
                                break;
                            case Rotation.Rotate270:
                                SetRotation(Rotation.Rotate0);
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                    }
                };
            }
        }

        private ICommand _rotateAnticlockwiseCommand;
        public ICommand RotateAnticlockwiseCommand
        {
            get
            {
                if (_rotateAnticlockwiseCommand != null)
                    return _rotateAnticlockwiseCommand;

                return _rotateAnticlockwiseCommand = new RelayCommand
                {
                    CanExecutePredicate = p => true,
                    ExecuteAction = p =>
                    {
                        switch (_imageRotation)
                        {
                            case Rotation.Rotate0:
                                SetRotation(Rotation.Rotate270);
                                break;
                            case Rotation.Rotate90:
                                SetRotation(Rotation.Rotate0);
                                break;
                            case Rotation.Rotate180:
                                SetRotation(Rotation.Rotate90);
                                break;
                            case Rotation.Rotate270:
                                SetRotation(Rotation.Rotate180);
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                    }
                };
            }
        }
        private void SetRotation(Rotation rotation)
        {
            _previewBitmapImage = null;
            _jpegImage = null;
            _imageRotation = rotation;

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PreviewBitmapImage)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(JpegImage)));
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

        private BitmapImage _previewBitmapImage;

        public BitmapImage PreviewBitmapImage
        {
            get
            {
                if (_previewBitmapImage != null)
                    return _previewBitmapImage;

                _previewBitmapImage = new BitmapImage();
                _previewBitmapImage.BeginInit();
                _previewBitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                _previewBitmapImage.DecodePixelWidth = 500;
                _previewBitmapImage.Rotation = _imageRotation;
                _previewBitmapImage.UriSource = new Uri(JpegPath);
                _previewBitmapImage.EndInit();
                _previewBitmapImage.Freeze();

                return _previewBitmapImage;
            }
        }

        public void InitializePreviewImage()
        {
            if (JpegPath == null) return;

            // Workaround: Call any method to load...
            PreviewBitmapImage.CheckAccess();
        }

        private BitmapImage _jpegImage;
        public BitmapImage JpegImage
        {
            get
            {
                if (_jpegImage != null)
                    return _jpegImage;

                _jpegImage = new BitmapImage();
                _jpegImage.BeginInit();
                _jpegImage.DecodePixelWidth = 1920;
                _jpegImage.UriSource = new Uri(JpegPath);
                _jpegImage.Rotation = _imageRotation;
                _jpegImage.EndInit();

                return _jpegImage;
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;


    }
}