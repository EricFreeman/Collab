using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Web.WebPages;
using Collab.Models;
using Collab.Services;
using Extensions;

namespace Collab.Controllers
{
    public class ArtController : Controller
    {
        #region Index

        /// <summary>
        /// Default page showing the current piece being worked on
        /// </summary>
        public ActionResult Index()
        {
            int width = 10 ,height = 10; //TODO: Hardcoded widths and heights are terrible please setup with config or something and make it so different art pieces can be different sizes

            var model = new IndexModel { ImageList = new Tile[width, height], Width = width, Height = height };

            model.ImageList = ArtBuilder(Server.MapPath("~/UploadedImages/Current/"), width, height);

            return View(model);
        }

        #endregion

        #region Upload

        /// <summary>
        /// Upload tile screen
        /// </summary>
        /// <param name="x">Location of tile in grid along x asix</param>
        /// <param name="y">Location of tile in grid along y axis</param>
        /// <returns></returns>
        [HttpGet]
        public PartialViewResult Upload(int x, int y)
        {
            //TODO: Make sure spot hasn't been taken

            var model = new UploadModel {X = x, Y = y};

            return PartialView(model);
        }

        /// <summary>
        /// Save tile or post error message back here
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Upload(UploadModel model)
        {
            if (!model.File.IsImage())
            {
                model.IsSuccessful = false;
                model.ErrorMessage = "Image type not recognized!";
            }
            else
            {
                var image = Image.FromStream(model.File.InputStream, true, true);

                if (GetImage(model.X, model.Y) != null)
                {
                    model.IsSuccessful = false;
                    model.ErrorMessage = "Tile already taken!";
                }
                else if (image.Width != 64 || image.Height != 64)
                {
                    if (!model.Resize)
                    {
                        model.IsSuccessful = false;
                        model.ErrorMessage = "Image dimensions not supported (64 x 64 required).  Would you like to resize?";
                    }
                    else
                    {
                        image = image.ResizeImage(64);
                    }
                }

                if (model.IsSuccessful.IsNotFalse())
                {
                    model.IsSuccessful = true;
                    image.Save(Server.MapPath("~/UploadedImages/Current/") + model.X + "-" + model.Y + ".png");
                    CompletionService.Run();
 
                    model.ImageUrl = GetImage(model.X, model.Y, true).FullName.RemoveBefore("\\UploadedImages\\");
                }
            }

            return PartialView(model);
        }

        #endregion

        #region Previous

        public ActionResult Previous()
        {
            var model = new PreviousModel();
            model.Collabs = new List<PreviousCollab>();

            var directories = new DirectoryInfo(Server.MapPath("~/UploadedImages/Previous")).GetDirectories().Reverse();

            directories.Each(d =>
            {
                var pc = new PreviousCollab {Id = d.Name, HasThumbnail = d.GetFiles("thumbnail.png").Any()};

                model.Collabs.Add(pc);
            });

            return View(model);
        }

        [HttpGet]
        public ActionResult PreviousImage(string id = "")
        {
            int width = 10, height = 10; //TODO: read these in from config once that's set up

            if (id.IsNullOrEmpty())
                id = GetDirectory("Previous").GetDirectories().Last().Name;

            var model = new PreviousImageModel();
            model.Id = id;
            model.Width = width;
            model.Height = height;

            model.ImageList = ArtBuilder(Server.MapPath("~/UploadedImages/Previous/{0}".ToFormat(id)),
                width, height);

            return View(model);
        }

        #endregion

        #region Helpers

        public DirectoryInfo GetDirectory(string folder)
        {
            return new DirectoryInfo(Server.MapPath("~/UploadedImages/{0}/".ToFormat(folder)));
        }

        private FileInfo GetImage(int x, int y)
        {
            var current = GetDirectory("Current");
            var images = current.GetFiles();
            return FindFile(images, x, y);
        }

        private FileInfo GetImage(int x, int y, bool checkPrevious)
        {
            var previousDirect = GetDirectory("Previous")
                .GetDirectories()
                .OrderByDescending(folder => folder.LastWriteTime)
                .FirstOrDefault();

            var previous = previousDirect != null
                ? previousDirect.GetFiles()
                : Enumerable.Empty<FileInfo>();

            return GetImage(x, y) ??
                   (checkPrevious ? FindFile(previous, x, y) : null);
        }

        private FileInfo FindFile(IEnumerable<FileInfo> files, int x, int y)
        {
            return files
                   .Where(FilterImages)
                   .FirstOrDefault(file => new Tile(file).Matches(x, y));
        }

        public Tile[,] ArtBuilder(string folderPath, int width, int height)
        {
            var images = new DirectoryInfo(folderPath).GetFiles().OnlyImages();

            return images
                .Where(FilterImages)
                .Select(x => new Tile(x))
                .Aggregate(new Tile[width, height], (acc, tile) =>
                {
                    acc[tile.Position.X, tile.Position.Y] = tile;

                    return acc;
                });
        }

        public bool FilterImages(FileInfo image)
        {
            return Regex.Match(Path.GetFileNameWithoutExtension(image.Name),
                @"^(\d+)-(\d+)", RegexOptions.None).Success;
        }

        #endregion
    }
}
