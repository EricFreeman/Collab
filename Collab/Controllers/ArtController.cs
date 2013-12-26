using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
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
            var model = new IndexModel { ImageList = ArtBuilder("~/UploadedImages/Current/") };

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
                    image.Save("~/UploadedImages/Current/".ToMapPath() + model.X + "-" + model.Y + ".png");
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

            var directories = new DirectoryInfo("~/UploadedImages/Previous".ToMapPath()).GetDirectories().Reverse();

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
            if (id.IsNullOrEmpty())
                id = GetDirectory("Previous").GetDirectories().Last().Name;

            var model = new CollabModel
            {
                Id = id,
                ImageList = ArtBuilder("~/UploadedImages/Previous/{0}".ToFormat(id))
            };

            return View("Collab", model);
        }

        #endregion

        #region Helpers

        public DirectoryInfo GetDirectory(string folder)
        {
            return new DirectoryInfo("~/UploadedImages/{0}/".ToMapPath().ToFormat(folder));
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

        public Tile[,] ArtBuilder(string path)
        {
            var folderPath = path.ToMapPath();
            var images = new DirectoryInfo(folderPath).GetFiles().OnlyImages();
            var config = ConfigService.Generate(folderPath);

            return images
                .Where(FilterImages)
                .Select(x => new Tile(x))
                .Aggregate(new Tile[config.Width, config.Height], (acc, tile) =>
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
