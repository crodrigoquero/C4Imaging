using System;
using System.Collections.Generic;
using System.Text;

namespace C4ImagingNetCore.Backend
{
    public interface IAspectRatioRepository
    {
        IEnumerable<AspectRatio> All { get; }
    }
}
