using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using static C4ImagingNetCore.Helpers.Maths;
using C4ImagingNetCore.Helpers;
using System.Net;
using Newtonsoft.Json;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;
using C4ImagingNetCore.Backend.Enums;

namespace C4ImagingNetCore.Backend
{
    public class ImageAnaliser
    {
        public static List<ImageCategorizationResult> GetImageCountryTaken(string imagePath, string apiKey)
        {
            ImageCategorizationResult imgCategoryzationResult = new ImageCategorizationResult();
            List<ImageCategorizationResult> imgCategoryzationResults = new List<ImageCategorizationResult>();
            ImageGeoCoordinates coords = new ImageGeoCoordinates();

            imgCategoryzationResult.FilePath = imagePath;

            try
            {
                coords = GetImageGeoCoordinates(imagePath);

                imgCategoryzationResult.Latitude = coords.Latitude;
                imgCategoryzationResult.Longitude = coords.Longitude;

                imgCategoryzationResult.ImageCategory = GetDataFromGoogle(imgCategoryzationResult.Latitude, imgCategoryzationResult.Longitude, apiKey);
            }
            catch (Exception ex) 
            {
                imgCategoryzationResult.ImageCategory = "Unknow Country";
            }
            finally
            {
                imgCategoryzationResult.DateAndTime = DateTime.Now;
                imgCategoryzationResults.Add(imgCategoryzationResult);             
            }

            return imgCategoryzationResults;

        }
        public static List<ImageCategorizationResult> GetImageMonthNameTaken(string imagePath)
        {
            ImageCategorizationResult imgCategoryzationResult = new ImageCategorizationResult();
            List<ImageCategorizationResult> imgCategoryzationResults = new List<ImageCategorizationResult>();
            Regex r = new Regex(":");

            imgCategoryzationResult.FilePath = imagePath;
            imgCategoryzationResult.ImageCategory = "Unknow Month";

            try
            {
                PropertyItem propItem = GetImageProperty(imagePath, EXIFTags.DateTime);

                if (propItem == null)
                {
                    imgCategoryzationResults.Add(imgCategoryzationResult);
                    return imgCategoryzationResults;
                }

                DateTime dateTaken = DateTime.Parse(r.Replace(Encoding.UTF8.GetString(propItem.Value), "-", 2));
            
                imgCategoryzationResult.ImageCategory = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(dateTaken.Month);
                //imgCategoryzationResult.DateAndTime = GetImageDate(imagePath);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            imgCategoryzationResults.Add(imgCategoryzationResult);
            return imgCategoryzationResults;
        }
        public static List<ImageCategorizationResult> GetImageSeasonTaken(string imagePath)
        {
            ImageCategorizationResult imgCategoryzationResult = new ImageCategorizationResult();
            List<ImageCategorizationResult> imgCategoryzationResults = new List<ImageCategorizationResult>();

            imgCategoryzationResult.FilePath = imagePath;
            int season = 0;

            try
            {
                season = getSeason(GetImageDate(imagePath)); // get season number
            }
            catch
            {
                season = 5;
            }

            // get the season name from its season number
            switch (season)
            {
                case 0:
                    imgCategoryzationResult.ImageCategory = "Spring";
                    break;
                case 1:
                    imgCategoryzationResult.ImageCategory = "Summer";
                    break;
                case 2:
                    imgCategoryzationResult.ImageCategory = "Winter";
                    break;
                case 3:
                    imgCategoryzationResult.ImageCategory = "Autumn";
                    break;
                default:
                    imgCategoryzationResult.ImageCategory = "Unknow Sason";
                    break;
            };

            imgCategoryzationResults.Add(imgCategoryzationResult);

            return imgCategoryzationResults;
        }
        public static List<ImageCategorizationResult> GetImageYearTaken(string imagePath)
        {
            ImageCategorizationResult imgCategoryzationResult = new ImageCategorizationResult();
            List<ImageCategorizationResult> imgCategoryzationResults = new List<ImageCategorizationResult>();

            imgCategoryzationResult.FilePath = imagePath;
            try
            {
                imgCategoryzationResult.ImageCategory = GetImageDate(imagePath).Year.ToString();
            }
            catch
            {
                imgCategoryzationResult.ImageCategory = "Unknow Year";
            }
             //imgCategoryzationResult.DateAndTime = GetImageDate(imagePath);

            imgCategoryzationResults.Add(imgCategoryzationResult);

            return imgCategoryzationResults;
        }
        public static List<ImageCategorizationResult> GetImageAspectRatio(string imagePath)
        {
            string veredict = String.Empty;

            ImageCategorizationResult imgCategoryzationResult = new ImageCategorizationResult();
            ImageCategorizationResult imgCategoryzationResultChild = new ImageCategorizationResult();

            List<ImageCategorizationResult> imgCategoryzationResults = new List<ImageCategorizationResult>();

            imgCategoryzationResult.FilePath = imagePath;
            imgCategoryzationResult.LogId = 1000001;

            try
            {
                // the 2nd parameter of below method is optinal (trace log on/off) 
                imgCategoryzationResultChild = CalculateImageAspectRatio(imagePath);

                imgCategoryzationResult.ImageCategory = imgCategoryzationResultChild.ImageCategory;
                imgCategoryzationResult.LogLevel = LogLevels.INFO;

            }
            catch  // from C4Imaging predefined exceptions
            {
                imgCategoryzationResult.LogLevel = LogLevels.WARNING;
            }


            imgCategoryzationResult.DateAndTime = DateTime.Now;
            imgCategoryzationResults.Add(imgCategoryzationResult);

            return imgCategoryzationResults;

        }
        public static List<ImageCategorizationResult> GetImageAuthor(string imagePath)
        {
            ImageCategorizationResult imgCategoryzationResult = new ImageCategorizationResult();
            List<ImageCategorizationResult> imgCategoryzationResults = new List<ImageCategorizationResult>();

            imgCategoryzationResult.FilePath = imagePath;
            imgCategoryzationResult.ImageCategory = "Unknow Author";

            try
            {
                PropertyItem propItem = GetImageProperty(imagePath, EXIFTags.Copyright);

                if (propItem == null)
                {
                    imgCategoryzationResults.Add(imgCategoryzationResult);
                    return imgCategoryzationResults;
                }

                imgCategoryzationResult.ImageCategory = Encoding.UTF8.GetString(propItem.Value).ToString().Replace("\0", "").Trim();

            }
            catch (Exception ex)
            {
                throw ex;
            }

            if (imgCategoryzationResult.ImageCategory.Length >= 100) // we must assume that copyright info doesn't contain an author / Company name
            {
                imgCategoryzationResult.ImageCategory = "Unknow Author"; // so, lets assume as unknow
            }

            imgCategoryzationResults.Add(imgCategoryzationResult);
            return imgCategoryzationResults;
        }
        public static List<ImageCategorizationResult> GetImageResolution(string imagePath)
        {

            ImageCategorizationResult imgCategoryzationResult = new ImageCategorizationResult();
            List<ImageCategorizationResult> imgCategoryzationResults = new List<ImageCategorizationResult>();

            imgCategoryzationResult.FilePath = imagePath;
            imgCategoryzationResult.ImageCategory = "Unknow Resolution";

            try
            {
                PropertyItem propItem3 = GetImageProperty(imagePath, EXIFTags.ResolutionUnit);
                string resolutionUnitDescription = "dpi";
 
                ushort resolutionXUnit = BitConverter.ToUInt16(propItem3.Value);
                if (resolutionXUnit == 3) resolutionUnitDescription = "cm";

                string resolution = GetImageResolutionProperty(imagePath) + " " + resolutionUnitDescription;
                imgCategoryzationResult.ImageCategory = resolution;

            }
            catch (Exception ex)
            {
                throw ex;
            }

            imgCategoryzationResults.Add(imgCategoryzationResult);
            return imgCategoryzationResults;
        }
        public static List<ImageCategorizationResult> GetImageCameraModel(string imagePath)
        {
            ImageCategorizationResult imgCategoryzationResult = new ImageCategorizationResult();
            List<ImageCategorizationResult> imgCategoryzationResults = new List<ImageCategorizationResult>();

            imgCategoryzationResult.FilePath = imagePath;
            imgCategoryzationResult.ImageCategory = "Unknow Camera Model";

            try
            {
                PropertyItem propItem1 = GetImageProperty(imagePath, EXIFTags.EquipMake);
                PropertyItem propItem2 = GetImageProperty(imagePath, EXIFTags.EquipModel);

                if (propItem2 == null || propItem1 == null)
                {
                    imgCategoryzationResults.Add(imgCategoryzationResult);
                    return imgCategoryzationResults;
                }

                imgCategoryzationResult.ImageCategory = Encoding.UTF8.GetString(propItem1.Value).ToString().Replace("\0", "").Trim() +
                    " - " + Encoding.UTF8.GetString(propItem2.Value).ToString().Replace("\0", "").Trim();

            }
            catch (Exception ex)
            {
                throw ex;
            }

            if (imgCategoryzationResult.ImageCategory.Length >= 100) // we must assume that copyright info doesn't contain an author / Company name
            {
                imgCategoryzationResult.ImageCategory = "Unknow Author"; // so, lets assume as unknow
            }

            imgCategoryzationResults.Add(imgCategoryzationResult);
            return imgCategoryzationResults;
        }


        #region EXIF functions
        private static float? GetImageLatitude(Image targetImg)
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
        private static float? GetImageLongitude(Image targetImg)
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
        private static DateTime GetImageDate(string imagePath)
        {
            ImageGeoCoordinates geoCoords = new ImageGeoCoordinates();
            Regex r = new Regex(":");

            using (Stream stream = File.Open(imagePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (Image image = Image.FromStream(stream))
            {
                try
                {
                    PropertyItem propItem = image.GetPropertyItem(36867);
                    string dateTaken = r.Replace(Encoding.UTF8.GetString(propItem.Value), "-", 2);
                    return DateTime.Parse(dateTaken);
                }
                catch
                {
                    throw new Exception("Date & Time info not found.");
                }

            }

        }
        private static PropertyItem GetImageProperty(string imagePath, EXIFTags tag)
        {
            using (Stream stream = File.Open(imagePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (Image image = Image.FromStream(stream))
            {

                try
                {
                    PropertyItem propItem = image.PropertyItems.Where(x => x.Id == (int)tag).FirstOrDefault();

                    if (propItem == null)
                    {
                        return null;
                    }

                    return propItem;
                }
                catch
                {
                    throw new Exception(tag.ToString() + " info not found.");
                }


            }
        }
        private static float GetImageResolutionProperty(string imagePath)
        {
            ImageGeoCoordinates geoCoords = new ImageGeoCoordinates();
            Regex r = new Regex(":");

            using (Stream stream = File.Open(imagePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (Image image = Image.FromStream(stream))
            {
                try
                {
                       return image.VerticalResolution;
                }
                catch
                {
                    throw new Exception("Resolution info not found.");
                }

            }

        }


        #endregion

        #region Helper Functions

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
        private static int getSeason(DateTime date)
        {
            float value = (float)date.Month + date.Day / 100f;  // <month>.<day(2 digit)>    
            if (value < 3.21 || value >= 12.22) return 3;   // Winter
            if (value < 6.21) return 0; // Spring
            if (value < 9.23) return 1; // Summer
            return 2;   // Autumn
        }
        private static string GetDataFromGoogle(float latitude, float longitude, string ApiKey)
        {

            using (var webclient = new WebClient())
            {
                string baseUrl = @"https://maps.googleapis.com/maps/api/geocode/json?latlng=" + latitude + "," + longitude + "&key=" + ApiKey;
                string json = webclient.DownloadString(baseUrl);

                GoogleGeoCodeResponse jsonResult = JsonConvert.DeserializeObject<GoogleGeoCodeResponse>(json);

                if (jsonResult.status == "OK")
                {
                    for (int i = 0; i < jsonResult.results.Length; i++)
                    {
                        string country = jsonResult.results[i].address_components[1].types[0];
                        if (country == "country")
                        {
                            return jsonResult.results[i].address_components[1].short_name;
                        }
                    }

                    return "unknow location";
                }
                else
                {
                    throw new Exception("Unable to access to Google Geocode Api");
                }

            }


        }
        private static ImageCategorizationResult CalculateImageAspectRatio(string imagePath)
        {

            ImageCategorizationResult imgCategoryzationResult = new ImageCategorizationResult();
            imgCategoryzationResult.FilePath = imagePath;
            imgCategoryzationResult.LogId = 2000001; // we are in another logging level (more detail)

            // ImageAnaliser imgAnalyser = new ImageAnaliser();
            Size currentImageSize = GetImageSize(imagePath);

            ImgAspectRatio CurrentImageAspectRatio = new ImgAspectRatio(); // helper entity to hold the current image aspect ratio
            CurrentImageAspectRatio.x = currentImageSize.Width / GCD(currentImageSize.Width, currentImageSize.Height);
            CurrentImageAspectRatio.y = currentImageSize.Height / GCD(currentImageSize.Width, currentImageSize.Height);

            double CurrentImageSizeQuotient = CurrentImageAspectRatio.x / CurrentImageAspectRatio.y;

            AspectRatioRepository spectRatioRepository = new AspectRatioRepository();
            List<ImgAspectRatio> Imglist = new List<ImgAspectRatio>();

            string veredict = "";

            var imagesRatioRepository = spectRatioRepository.All.ToList();
            foreach (AspectRatio item in imagesRatioRepository.OrderByDescending(x => x.Quotient))
            {

                if (CurrentImageSizeQuotient <= item.Quotient)
                {
                    veredict = item.Description;
                }

            }

            imgCategoryzationResult.LogLevel = LogLevels.INFO;
            imgCategoryzationResult.FilePath = imagePath;
            imgCategoryzationResult.ImageCategory = veredict;
            imgCategoryzationResult.DateAndTime = DateTime.Now;

            return imgCategoryzationResult;

        }
        private static ImageGeoCoordinates GetImageGeoCoordinates(string imageFileLocation)
        {

            ImageGeoCoordinates geoCoords = new ImageGeoCoordinates();

            using (Stream stream = File.Open(imageFileLocation, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (Image image = Image.FromStream(stream))
            {
                try
                {
                    PropertyItem propItem = image.GetPropertyItem(2);

                    geoCoords.Latitude = (float)GetImageLatitude(image);
                    geoCoords.Longitude = (float)GetImageLongitude(image);

                    return geoCoords;

                }
                catch
                {
                    throw new Exception("Geo-location info not found.");
                }

            }

        }
        private static Size GetImageSize(string imageFileLocation)
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

        #endregion

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
