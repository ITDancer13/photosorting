using System;

namespace PhotoSorting.Entities
{
    public class Settings
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string DirectoryFilePath { get; set; }
    }
}
