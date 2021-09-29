using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using Azure.Storage.Blobs;

namespace app.Models
{
    public class File
    {
        [Key]
        public Guid Id { get; set; }
        public DateTime Created { get; set; }
        public string Uri { get; set; }

        public string FileName { get; set; }

        public string BlobName 
        {
            get{
                return Id.ToString();
            }
        }

        public string FileType
        {
            get
            {
                return Path.GetExtension(FileName);
            }
        }

        public static readonly List<string> ImageExtensions = new() { ".JPG", ".JPE", ".BMP", ".GIF", ".PNG" };
        public static readonly List<string> VideoExtensions = new() { ".WEBM", ".MPG", ".MP2", ".MPEG", ".MPE", ".MPV", ".OGG", ".MP4", ".M4P", ".M4V", ".AVI", ".WMV", ".MOV", ".QT", ".FLV", ".SWF", ".AVCHD" };
        public bool IsVideo
        {
            get
            {
                return VideoExtensions.Contains(FileType.ToUpperInvariant());
            }
        }
        public bool IsImage
        {
            get
            {
                return ImageExtensions.Contains(FileType.ToUpperInvariant());
            }
        }

        public bool IsValidFileType
        {
            get
            {
                return IsVideo || IsImage;
            }
        }
    };
}