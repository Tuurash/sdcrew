using System;
using System.Collections.Generic;
using System.Text;

namespace sdcrew.Models
{
    public class Document
    {
        public int documentId { get; set; }
        public string fileId { get; set; }
        public string fileName { get; set; }
        public int parentAssociationType { get; set; }
        public int imageParentId { get; set; }
    }

}
