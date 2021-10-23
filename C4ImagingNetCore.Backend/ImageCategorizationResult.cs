using C4ImagingNetCore.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace C4ImagingNetCore.Backend
{
    public class ImageCategorizationResult
    {
        public string FilePath { get; set; }
        public string ImageCategory { get; set; }
        public int LogId { get; set; } // holds the log detail level (as a subcategory)
        public LogLevels LogLevel { get; set; } // Info, warning, critical, etc.
        public DateTime DateAndTime { get; set; }

        public float Latitude { get; set; }
        public float Longitude { get; set; }

    }
}
