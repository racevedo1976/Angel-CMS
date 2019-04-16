using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Angelo.Connect.Abstractions;
using Angelo.Connect.Models;

namespace Angelo.Connect.Models
{
    public class FileDocument : IDocument, IContentType
    {
        public FileDocument() : base()
        {
        }

        public string Title { get; set; }

        public string FileName { get; set; }

        public string FileType { get; set; }

        public Int64 ContentLength { get; set; }

        public string FileExtension
        {
            get {
                return System.IO.Path.GetExtension(FileName);
            }
        }

        public string FileSize
        {
            get {
                if (ContentLength >= 1073741824)
                {
                    return Math.Round(ContentLength / 1073741824D, 2) + " GB";
                }
                else if (ContentLength >= 1048576)
                {
                    return Math.Round(ContentLength / 1048576D, 2) + " MB";
                }
                return Math.Round(ContentLength / 1024D, 2) + " KB";
            }
        }

        public string DocumentId { get; set; } = KeyGen.NewGuid();

        public string Description { get; set; }

        public DateTime Created { get; set; } = DateTime.Now;

        public string CreatedDateString
        {
            get {
                return Created.ToString("MM/dd/yyyy h:mm:ss tt");
            }
        }
        public ICollection<DocumentTag> Tags { get; set; }


        // Taking this out due to logs being in another DbContext now (otherwise requires the same tabe defs duplicated in Connect and Log)
        //  public ICollection<DocumentEvent> Events { get; set; }
    }
}
