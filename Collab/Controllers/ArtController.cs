using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Collab.Models;
using Extensions;

namespace Collab.Controllers
{
    public class ArtController : Controller
    {
        /// <summary>
        /// Default page showing the current piece being worked on
        /// </summary>
        public ActionResult Index()
        {
            var model = new IndexModel { ImageList = new Tile[10, 10], Width = 10, Height = 10 };

            var images = new DirectoryInfo(Server.MapPath("~/UploadedImages/Current/")).GetFiles();

            foreach (var image in images)
            {
                var loc = image.Name.Remove(image.Name.IndexOf('.')).Split('-');

                var x = int.Parse(loc[0]);
                var y = int.Parse(loc[1]);

                model.ImageList[x, y] = new Tile() {X = x, Y = y, ImagePath = image.Name};
            }

            return View(model);
        }

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
                    model.ImageUrl = GetImage(model.X, model.Y).Name;
                }
            }

            return PartialView(model);
        }

        private FileInfo GetImage(int x, int y)
        {
            var images = new DirectoryInfo(Server.MapPath("~/UploadedImages/Current/")).GetFiles();

            return images.FirstOrDefault(file =>
            {
                var fileName = Path.GetFileNameWithoutExtension(file.FullName);
                var tokens = fileName.Split('-');
                return tokens[0] == x.ToString() && tokens[1] == y.ToString();
            });
        }
    }
}
