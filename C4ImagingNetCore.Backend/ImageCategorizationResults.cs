using System;
using System.Collections.Generic;
using System.Text;

namespace C4ImagingNetCore.Backend
{
    public class ImageCategorizationResults
    {
        //trying to use the cheapest way to keep the list in memory!
        public IList<ImageCategorizationResult> List { get; set; }

    }
}
