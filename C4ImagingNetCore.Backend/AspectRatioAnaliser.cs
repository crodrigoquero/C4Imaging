using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using static C4ImagingNetCore.ImageAnalyser;
using static C4ImagingNetCore.Helpers.Maths;
using C4ImagingNetCore.Helpers;

namespace C4ImagingNetCore.Backend
{
    public class AspectRatioAnaliser
    {

        public static ImageCategorizationResults GetImageAspectRatio(string imagePath)
        {
            string veredict = String.Empty;

            ImageCategorizationResult imgCategoryzationResult = new ImageCategorizationResult();
            ImageCategorizationResult imgCategoryzationResultChild = new ImageCategorizationResult();

            ImageCategorizationResults imgCategoryzationResults = new ImageCategorizationResults();

            imgCategoryzationResults.List = new List<ImageCategorizationResult>();

            imgCategoryzationResult.FilePath = imagePath;
            imgCategoryzationResult.LogId = 1000001;

            try
            {
                // the 2nd parameter of below method is optinal (trace log on/off) 
                imgCategoryzationResultChild = CalculateImageAspectRatio(imagePath);

                imgCategoryzationResult.ImageCategory = imgCategoryzationResultChild.ImageCategory;
                imgCategoryzationResult.LogLevel = LogLevels.INFO;
                imgCategoryzationResult.HasException = false;

            }
            catch (ImageProcessorException ex) // from C4Imaging predefined exceptions
            {
                imgCategoryzationResult.HasException = true;
                imgCategoryzationResult.LogLevel = LogLevels.WARNING;
                imgCategoryzationResult.ExceptionDescription = ex.Message;
            }


            imgCategoryzationResult.DateAndTime = DateTime.Now;

            imgCategoryzationResults.List.Add(imgCategoryzationResult);
            //imgCategoryzationResults.List.Add(imgCategoryzationResultChild);

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
            imgCategoryzationResult.HasException = false;
            imgCategoryzationResult.DateAndTime = DateTime.Now;

            return imgCategoryzationResult;

        }

    }
}
