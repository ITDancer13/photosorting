using System;
using PhotoSorting.Model;

namespace PhotoSorting.Entities
{
    public class Image
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Filepath without any extension
        /// </summary>
        public string Filepath { get; set; }

        /// <summary>
        /// Selection Mode of the Image
        /// </summary>
        public SelectionMode SelectionMode { get; set; }
    }
}
