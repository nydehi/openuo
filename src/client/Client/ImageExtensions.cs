using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;

namespace Client
{
    public static class ImageExtensions
    {
        public static Stream ToStream(this Image image)
        {
            MemoryStream stream = new MemoryStream();
            image.Save(stream, image.RawFormat);
            return stream;
        }
    }
}
