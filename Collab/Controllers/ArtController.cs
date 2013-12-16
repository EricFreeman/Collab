using System.Drawing;
using System.IO;
using System.Linq;
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
            var model = new IndexModel { ImageList = new Tile[10, 10], Width = 10, Height = 10 };

            var images = new DirectoryInfo(Server.MapPath("~/UploadedImages/Current/")).GetFiles().OnlyImages();

            images.Each(image =>
            {
                var loc = image.Name.Remove(image.Name.IndexOf('.')).Split('-');

                var x = int.Parse(loc[0]);
                var y = int.Parse(loc[1]);

                model.ImageList[x, y] = new Tile() {X = x, Y = y, ImagePath = image.Name};
            });

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

        //TODO: This is really ugly, please rewrite
        private FileInfo GetImage(int x, int y, bool checkPrevious = false)
        {
            var images = new DirectoryInfo(Server.MapPath("~/UploadedImages/Current/")).GetFiles();
            var previousDirect =
                new DirectoryInfo(Server.MapPath("~/UploadedImages/Previous")).GetDirectories()
                    .OrderByDescending(folder => folder.LastWriteTime).FirstOrDefault();
            var previous = previousDirect != null ? previousDirect.GetFiles() : null;

            var image = FindFile(images, x, y) ?? 
                (checkPrevious ? FindFile(previous, x, y) : null);

            return image;
        }

        private FileInfo FindFile(FileInfo[] files, int x, int y)
        {
            return files.FirstOrDefault(file =>
            {
                var fileName = Path.GetFileNameWithoutExtension(file.FullName);
                var tokens = fileName.Split('-');
                return tokens[0] == x.ToString() && tokens[1] == y.ToString();
            });
        }

        #endregion
    }
}
