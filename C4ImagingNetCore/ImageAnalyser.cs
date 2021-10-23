using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

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

        public ImageGeoCoordinates GetImageGeoCoordinates(string imageFileLocation)
        {

            ImageGeoCoordinates geoCoords = new ImageGeoCoordinates();


                using (Stream stream = File.Open(imageFileLocation, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (Image image = Image.FromStream(stream))
                {
                    try
                    {
                        PropertyItem propItem = image.GetPropertyItem(2);

                        geoCoords.Latitude = (float)GetLatitude(image);
                        geoCoords.Longitude = (float)GetLongitude(image);

                        return geoCoords;

                    } catch (Exception ex)
                    {
                        throw new Exception("Geo-location info not found.");
                    }

                }


            

        }
        
        
        
        private static float? GetLatitude(Image targetImg)
        {
            try
            {
                //Property Item 0x0001 - PropertyTagGpsLatitudeRef
                PropertyItem propItemRef = targetImg.GetPropertyItem(1);
                //Property Item 0x0002 - PropertyTagGpsLatitude
                PropertyItem propItemLat = targetImg.GetPropertyItem(2);
                return ExifGpsToFloat(propItemRef, propItemLat);
            }
            catch (ArgumentException)
            {
                return null;
            }
        }
        private static float? GetLongitude(Image targetImg)
        {
            try
            {
                ///Property Item 0x0003 - PropertyTagGpsLongitudeRef
                PropertyItem propItemRef = targetImg.GetPropertyItem(3);
                //Property Item 0x0004 - PropertyTagGpsLongitude
                PropertyItem propItemLong = targetImg.GetPropertyItem(4);
                return ExifGpsToFloat(propItemRef, propItemLong);
            }
            catch (ArgumentException)
            {
                return null;
            }
        }
        private static float ExifGpsToFloat(PropertyItem propItemRef, PropertyItem propItem)
        {
            uint degreesNumerator = BitConverter.ToUInt32(propItem.Value, 0);
            uint degreesDenominator = BitConverter.ToUInt32(propItem.Value, 4);
            float degrees = degreesNumerator / (float)degreesDenominator;

            uint minutesNumerator = BitConverter.ToUInt32(propItem.Value, 8);
            uint minutesDenominator = BitConverter.ToUInt32(propItem.Value, 12);
            float minutes = minutesNumerator / (float)minutesDenominator;

            uint secondsNumerator = BitConverter.ToUInt32(propItem.Value, 16);
            uint secondsDenominator = BitConverter.ToUInt32(propItem.Value, 20);
            float seconds = secondsNumerator / (float)secondsDenominator;

            float coorditate = degrees + (minutes / 60f) + (seconds / 3600f);
            string gpsRef = System.Text.Encoding.ASCII.GetString(new byte[1] { propItemRef.Value[0] }); //N, S, E, or W
            if (gpsRef == "S" || gpsRef == "W")
                coorditate = 0 - coorditate;
            return coorditate;
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
