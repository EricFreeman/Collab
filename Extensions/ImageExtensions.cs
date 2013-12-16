using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Drawing;

namespace Extensions
{
    public static class ImageExtensions
    {
        private static readonly string[] Formats = { ".jpg", ".png", ".gif", ".jpeg" };
        
        public static Image ResizeImage(this Image image, int newSize)
        {
            return new Bitmap(image, new Size(newSize, newSize));
        }

        public static Image ResizeImage(this Image image, int newWidth, int newHeight)
        {
            return new Bitmap(image, new Size(newWidth, newHeight));
        }

        public static bool IsImage(this HttpPostedFileBase file)
        {
            if (file == null) return false;
            if (file.ContentType.Contains("image")) return true;

            return Formats.Any(item => file.FileName.EndsWith(item, StringComparison.OrdinalIgnoreCase));
        }

        public static FileInfo[] OnlyImages(this FileInfo[] files)
        {
            return files.Where(x => Formats.Contains(x.Extension)).ToArray();
        }
    }
}
