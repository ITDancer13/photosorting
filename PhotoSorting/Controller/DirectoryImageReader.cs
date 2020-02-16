using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using PhotoSorting.Model;

namespace PhotoSorting.Controller
{
    public class DirectoryImageReader
    {
        private readonly string _path;
        private static readonly string[] JpegExtensions = { ".jpg", ".jpeg" };
        private static readonly string[] RawExtensions = { ".cr2", ".nef", "cr3" };

        private List<ImageFileViewModel> _imageFiles;

        public DirectoryImageReader(string path)
        {
            _path = path;
        }

        public Task<List<ImageFileViewModel>> GetImageFilesAsync()
        {
            if (_imageFiles != null)
                return Task.FromResult(_imageFiles);

            _imageFiles = new List<ImageFileViewModel>();
            var files = Directory.GetFiles(_path)
                .Where(p =>
                {
                    var extension = Path.GetExtension(p).ToLower();
                    return JpegExtensions.Contains(extension) || RawExtensions.Contains(extension);
                })
                .OrderBy(p => p);

            var groupedFiles = files.GroupBy(Path.GetFileNameWithoutExtension);

            foreach (var groupedFile in groupedFiles)
            {
                var jpegFilepath = groupedFile.FirstOrDefault(p => JpegExtensions.Contains(Path.GetExtension(p)?.ToLower()));
                var rawFilepath = groupedFile.FirstOrDefault(p => RawExtensions.Contains(Path.GetExtension(p)?.ToLower()));

                var imageFile = new ImageFileViewModel(jpegFilepath, rawFilepath);

                _imageFiles.Add(imageFile);
            }

            return Task.Run(() =>
            {
                _imageFiles.AsParallel().ForAll(p => p.InitializePreviewImage());
                return _imageFiles;
            });
        }
    }
}
