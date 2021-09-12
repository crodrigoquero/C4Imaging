using System;
using System.Collections.Generic;
using System.Text;

namespace C4ImagingNetCore
{
    public class AspectRatio
    {
        public AspectRatio(string description, double width, double height)
        {
            Width = width;
            Height = height;
            Description = description;
        }


        public double Width { get; protected set; }

        public double Height { get; protected set; }

        public string Description { get; protected set; }


        /// <summary>
        /// The quotient value of this ratio, calculated by dividing the width by the height.
        /// </summary>
        public double Quotient
        {
            get { return Width / Height; }
        }


        public override string ToString()
        {
            return Description;
        }


        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            var other = (AspectRatio)obj;
            return (Width == other.Width) && (Height == other.Height);
        }

        public override int GetHashCode()
        {
            return (Width / Height).GetHashCode();
        }


    }

}
