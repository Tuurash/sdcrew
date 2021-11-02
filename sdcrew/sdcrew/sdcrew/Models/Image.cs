using System;
using System.Collections.Generic;
using System.Text;

namespace sdcrew.Models
{
    class Image
    {
        public string containerId { get; set; }
        public string fileId { get; set; }
        public string fileName { get; set; }
        public string fileExtension { get; set; }
        public string contentType { get; set; }
        public int sizeBytes { get; set; }
        public string fileUri { get; set; }
        public string sasToken { get; set; }
        public string storageAccount { get; set; }
        public bool exists { get; set; }
    }
}
