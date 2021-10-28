using System;
using System.Collections.Generic;
using System.Text;

namespace C4ImagingNetCore.Backend.Enums
{
    public enum Flash : int
    {
      NotFired = 0,
      Fired = 1,
      FiredButNoStrobeReturned = 5,
      FiredAndStrobeReturned = 7
    }
}
