using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using static C4ImagingNetCore.ImageAnalyser;
using static C4ImagingNetCore.Helpers.Maths;
using C4ImagingNetCore.Helpers;
using System.Net;
using Newtonsoft.Json;

namespace C4ImagingNetCore.Backend
{
    public class ImageAnaliser
    {
        public static List<ImageCategorizationResult> GetImageCountryTaken(string imagePath, string apiKey)
        {
            ImageAnalyser imgAnalyser = new ImageAnalyser();

            ImageCategorizationResult imgCategoryzationResult = new ImageCategorizationResult();
            List<ImageCategorizationResult> imgCategoryzationResults = new List<ImageCategorizationResult>();
            
            ImageGeoCoordinates coords = new ImageGeoCoordinates();

            imgCategoryzationResult.FilePath = imagePath;
            imgCategoryzationResult.LogId = 1000001;
            imgCategoryzationResult.LogLevel = LogLevels.INFO;

            try
            {
                coords = imgAnalyser.GetImageGeoCoordinates(imagePath);

                imgCategoryzationResult.Latitude = coords.Latitude;
                imgCategoryzationResult.Longitude = coords.Longitude;

                imgCategoryzationResult.ImageCategory = GetDataFromGoogle(imgCategoryzationResult.Latitude, imgCategoryzationResult.Longitude, apiKey);
            }
            catch (Exception ex) 
            {
                imgCategoryzationResult.LogLevel = LogLevels.WARNING;
                throw new Exception(imgCategoryzationResult.LogLevel.ToString() + ": " + ex.Message);
            }
            finally
            {
                imgCategoryzationResult.DateAndTime = DateTime.Now;
                imgCategoryzationResults.Add(imgCategoryzationResult);             
            }

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

        private static ImageCategorizationResult CalculateImageAspectRatio(string imagePath)
        {

            ImageCategorizationResult imgCategoryzationResult = new ImageCategorizationResult();
            imgCategoryzationResult.FilePath = imagePath;
            imgCategoryzationResult.LogId = 2000001; // we are in another logging level (more detail)

            ImageAnalyser imgAnalyser = new ImageAnalyser();
            Size currentImageSize = imgAnalyser.GetImageSize(imagePath);

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

                    return "unknow";
                }
                else
                {
                    return jsonResult.status;
                }

            }


        }
    }
}
