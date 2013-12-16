using System;
using System.IO;
using System.Linq;
using System.Web;
using Extensions;

namespace Collab.Services
{
    public static class CompletionService
    {
        static readonly string RootPath = HttpContext.Current.Server.MapPath("~/UploadedImages/");

        /// <summary>
        /// Verifies if the artwork is complete, and then saves it 
        /// in the Previous folder and clears out the current piece
        /// </summary>
        public static void Run()
        {
            if (IsComplete())
                CompleteArt();
        }

        // TODO: make a config file for each piece that specifies width x height to make this method not hardcoded 
        private static bool IsComplete()
        {
            var images = new DirectoryInfo(RootPath + "Current/").GetFiles().OnlyImages();
            return images.Count() == 100;
        }

        private static void CompleteArt()
        {
            var newDirectory = RootPath + "Previous\\{0}".ToFormat(DateTime.Now.ToString("HHmmssfffffff"));
            Directory.CreateDirectory(newDirectory);

            var oldFiles = new DirectoryInfo(RootPath + "Current\\").GetFiles().OnlyImages();
            oldFiles.Each(file => file.MoveTo(Path.Combine(newDirectory, file.Name)));
        }
    }
}