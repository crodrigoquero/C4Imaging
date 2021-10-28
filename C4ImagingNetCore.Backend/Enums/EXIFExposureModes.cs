using System;
using System.Collections.Generic;
using System.Text;

namespace C4ImagingNetCore.Backend.Enums
{
    public enum EXIFExposureModes : int
    {
      Unknown = 0,
      Average = 1,
      CenterWeightedAverage = 2,
      Spot = 3,
      MultiSpot = 4,
      MultiSegment = 5,
      Partial = 6,
      Other = 255
    }
}
