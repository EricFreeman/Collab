using System.Drawing;
using System.IO;

namespace Collab.Models
{
    public class IndexModel
    {
        public Tile[,] ImageList { get; set; }
    }

    public class Tile
    {
        public Tile(FileInfo image)
        {
            ImagePath = image.Name;
            Position = GetPositionFromFile(image);
        }

        public static Position GetPositionFromFile(FileInfo file)
        {
            var loc = Path.GetFileNameWithoutExtension(file.Name).Split('-');
            return new Position(int.Parse(loc[0]), int.Parse(loc[1]));
        }

        public Position Position { get; set; }
        public string ImagePath { get; set; }

        public bool Matches(int x, int y)
        {
            return new Position(x, y).Equals(Position);
        }
    }

    public class Position
    {
        public Position(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; set; }
        public int Y { get; set; }

        protected bool Equals(Position other)
        {
            return X == other.X && Y == other.Y;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Position) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (X*397) ^ Y;
            }
        }
    }
}