using System;
using System.Collections.Generic;
using System.Text;

namespace C4ImagingNetCore.Backend.Enums
{
    public enum LightSources : int
    {
      Unknown = 0,
      Daylight = 1,
      Fluorescent = 2,
      Tungsten = 3,
      Flash = 10,
      StandardLightA = 17,
      StandardLightB = 18,
      StandardLightC = 19,
      D55 = 20,
      D65 = 21,
      D75 = 22,
      Other = 255
    }
}
