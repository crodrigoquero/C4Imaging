using System;
using System.Collections.Generic;
using System.Text;

namespace C4ImagingNetCore
{
    public interface IAspectRatioRepository
    {
        IEnumerable<AspectRatio> All { get; }
    }
}
