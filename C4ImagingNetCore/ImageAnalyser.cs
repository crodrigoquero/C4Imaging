using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace C4ImagingNetCore
{

    /// <summary>
    /// Extracts information from an image file.
    /// </summary>
    public class ImageAnalyser : IImageAnalyser
    {

        /// <summary>
        /// Returns the dimensions of the image in the specified image file, in a Size structure.
        /// </summary>
        /// <param name="imageFileLocation">The location of the image file.</param>
        /// <returns>The size of the image.</returns>
        public Size GetImageSize(string imageFileLocation)
        {
            if (String.IsNullOrWhiteSpace(imageFileLocation)) throw new ImageProcessorException("No image has been specified.");

            try
            {
                using (Stream stream = File.Open(imageFileLocation, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (Image image = Image.FromStream(stream))
                {
                    return new Size(image.Width, image.Height);
                }
            }
            catch (ArgumentException)
            {
                throw new InvalidImageFileException(imageFileLocation);
            }
            catch (IOException ex)
            {
                throw new FileAccessException(imageFileLocation, ex);
            }
        }


        #region Exceptions

        public class ImageProcessorException : Exception
        {
            public ImageProcessorException(string message) : base(message) { }
            public ImageProcessorException(string message, Exception innerException) : base(message, innerException) { }
        }


        public class FileAccessException : ImageProcessorException
        {
            public FileAccessException(string fileLocation, Exception innerException) :
              base(String.Format("The image file at \"{0}\" could not be accessed.", fileLocation), innerException)
            { }
        }


        public class InvalidImageFileException : ImageProcessorException
        {
            public InvalidImageFileException(string fileLocation)
              : base(String.Format("The file at \"{0}\" is not a valid image.", fileLocation)) { }
        }

        #endregion

    }


}
