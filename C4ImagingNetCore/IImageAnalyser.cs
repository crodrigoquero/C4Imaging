using System.Drawing;

namespace C4ImagingNetCore
{
    public interface IImageAnalyser
    {
        Size GetImageSize(string imageFileLocation);
    }
}
