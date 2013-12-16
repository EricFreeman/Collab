using System.IO;

namespace Extensions
{
    public static class DirectoryExtensions
    {
        public static void Copy(this DirectoryInfo d, string targetDir)
        {
            Directory.CreateDirectory(targetDir);

            d.GetFiles().Each(file => file.MoveTo(Path.Combine(targetDir, file.Name)));
            d.GetDirectories().Each(directory => directory.MoveTo(Path.Combine(targetDir, directory.Name)));
        }
    }
}