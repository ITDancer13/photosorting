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
        private static readonly string[] RawExtensions = { ".cr2", ".nef" };

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
                var imageFile = new ImageFileViewModel(groupedFile.FirstOrDefault(p => JpegExtensions.Contains(Path.GetExtension(p)?.ToLower())),
                    groupedFile.FirstOrDefault(p => RawExtensions.Contains(Path.GetExtension(p)?.ToLower())));

                _imageFiles.Add(imageFile);
            }

            return Task.Run(() =>
            {
                _imageFiles.AsParallel().ForAll(p => p.LoadInfos());
                return _imageFiles;
            });
        }
    }
}
