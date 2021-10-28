using System;
using System.Collections.Generic;
using System.Text;

namespace C4ImagingNetCore.Backend.Enums
{
    public enum EXIFDataTypes : int
    {
      UnsignedByte = 1,
      AsciiString = 2,
      UnsignedShort = 3,
      UnsignedLong = 4,
      UnsignedRational = 5,
      SignedByte = 6,
      Undefined = 7,
      SignedShort = 8,
      SignedLong = 9,
      SignedRational = 10,
      SingleFloat = 11,
      DoubleFloat = 12
    }
}
