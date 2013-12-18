namespace Collab.Models
{
    public class PreviousImageModel
    {
        public Tile[,] ImageList { get; set; }
        public string Id { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }
}