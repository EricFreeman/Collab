using System.Drawing;

namespace Collab.Models
{
    public class IndexModel
    {
        public Tile[,] ImageList { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }

    public class Tile
    {
        public int X { get; set; }
        public int Y { get; set; }
        public string ImagePath { get; set; }
    }
}