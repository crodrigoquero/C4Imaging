using System;

namespace C4ImagingNetCore.Helpers
{

    public static class Maths
    {
        /// <summary>
        /// Web need to find Greatest Common Divisor, then divide width (a) and then heigh (b) by it
        /// later on the main funcion in order to get the image aspect ratio
        /// </summary>
        /// <param name="a">Image width</param>
        /// <param name="b">Image height</param>
        /// <returns></returns>
        public static int GCD(int a, int b)
        {
            int Remainder;

            while (b != 0)
            {
                Remainder = a % b;
                a = b;
                b = Remainder;
            }

            return a;
        }
    }

}
