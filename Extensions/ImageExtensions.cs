using System;
using System.Linq;
using System.Web;
using System.Drawing;

namespace Extensions
{
    public static class ImageExtensions
    {
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

            string[] formats = { ".jpg", ".png", ".gif", ".jpeg" };

            return formats.Any(item => file.FileName.EndsWith(item, StringComparison.OrdinalIgnoreCase));
        }
    }
}
