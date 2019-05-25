using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoSorting.Entities
{
    public class Settings
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string DirectoryFilePath { get; set; }
    }
}
