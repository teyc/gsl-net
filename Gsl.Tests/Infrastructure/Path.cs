using System.IO.Abstractions;

namespace Gsl.Tests
{
    public static class Path
    {
        public static string DataFile(string relativePath)
        {
            return $"../../../{relativePath}";
        }

        public static string ReadToEnd(this IFileInfo fileInfo)
        {
            using var stream = fileInfo.OpenText();
            return stream.ReadToEnd();
        }
    }
}
