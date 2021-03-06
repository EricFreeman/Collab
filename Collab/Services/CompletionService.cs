﻿using System;
using System.IO;
using System.Linq;
using Extensions;

namespace Collab.Services
{
    public static class CompletionService
    {
        private static readonly string RootPath = "~/UploadedImages/".ToMapPath();

        /// <summary>
        /// Verifies if the artwork is complete, and then saves it 
        /// in the Previous folder and clears out the current piece
        /// </summary>
        public static void Run()
        {
            if (IsComplete())
                CompleteArt();
        }

        private static bool IsComplete()
        {
            var config = ConfigService.Generate(RootPath + "Current/");
            var images = new DirectoryInfo(RootPath + "Current/").GetFiles().OnlyImages();
            return images.Count() == (config.Width * config.Height);
        }

        private static void CompleteArt()
        {
            var newDirectory = RootPath + "Previous\\{0}".ToFormat(DateTime.Now.ToString("yyyyMMddHHmmssfffffff"));
            Directory.CreateDirectory(newDirectory);
            File.Copy(RootPath + "Current/.config", newDirectory + "/.config");
            ConfigService.GenerateNewCurrent();

            var oldFiles = new DirectoryInfo(RootPath + "Current\\").GetFiles().OnlyImages();
            oldFiles.Each(file => file.MoveTo(Path.Combine(newDirectory, file.Name)));
        }
    }
}