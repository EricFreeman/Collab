using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Collab.Models;

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

            var images = new DirectoryInfo(Server.MapPath("~/Content/Images/Current/")).GetFiles();

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
            if (!IsImage(model.File))
            {
                model.IsSuccessful = false;
                model.ErrorMessage = "Image type not recognized!";
            }
            else
            {
                using (System.Drawing.Image image = System.Drawing.Image.FromStream(model.File.InputStream, true, true))
                {
                    if (GetImage(model.X, model.Y) != null)
                    {
                        model.IsSuccessful = false;
                        model.ErrorMessage = "Tile already taken!";
                    }
                    else if (image.Width != 64 || image.Height != 64)
                    {
                        model.IsSuccessful = false;
                        model.ErrorMessage = "Image dimensions not supported (64 x 64 required)";
                    }
                    else
                    {
                        model.File.SaveAs(Server.MapPath("~/Content/Images/Current/") + model.X + "-" + model.Y + ".png");
                        model.IsSuccessful = true;
                        model.ImageUrl = GetImage(model.X, model.Y).Name;
                    }
                }
            }

            return PartialView(model);
        }

        private FileInfo GetImage(int x, int y)
        {
            var images = new DirectoryInfo(Server.MapPath("~/Content/Images/Current/")).GetFiles();

            return images.FirstOrDefault(file =>
            {
                var fileName = Path.GetFileNameWithoutExtension(file.FullName);
                var tokens = fileName.Split('-');
                return tokens[0] == x.ToString() && tokens[1] == y.ToString();
            });
        }

        private bool IsImage(HttpPostedFileBase file)
        {
            if (file == null) return false;
            if (file.ContentType.Contains("image")) return true;

            string[] formats = new string[] { ".jpg", ".png", ".gif", ".jpeg" }; // add more if u like...

            // linq from Henrik Stenbæk
            return formats.Any(item => file.FileName.EndsWith(item, StringComparison.OrdinalIgnoreCase));
        }
    }
}
