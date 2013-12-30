using System.Web;

namespace Collab.Models
{
    public class UploadModel
    {
        public HttpPostedFileBase File { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        public bool? IsSuccessful { get; set; }
        public bool Resize { get; set; }
        public string ErrorMessage { get; set; }
        public string ImageUrl { get; set; }
    }
}