using System;
using System.Collections.Generic;
using System.Text;

namespace C4ImagingNetCore.Backend
{
    public class AspectRatioRepository : IAspectRatioRepository
    {
        static readonly List<AspectRatio> AllRatios = new List<AspectRatio>();

        static AspectRatioRepository()
        {
            AllRatios.Add(new AspectRatio("Standard (4x3)", 4, 3));
            AllRatios.Add(new AspectRatio("Widescreen (16x9)", 16, 9));
            AllRatios.Add(new AspectRatio("IMAX", 1.43, 1));
        }

        public IEnumerable<AspectRatio> All
        {
            get { return AllRatios; }
        }

    }
}
