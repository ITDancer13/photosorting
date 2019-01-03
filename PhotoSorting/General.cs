using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using PhotoSorting.Model;

namespace PhotoSorting
{
    public static class General
    {
        public static ObservableCollection<ImageFile> ImagesCollection { get; } = new ObservableCollection<ImageFile>();
    }
}
