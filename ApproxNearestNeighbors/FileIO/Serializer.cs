using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ApproxNearestNeighbors.FileIO
{
    class Serializer
    {
        public Serializer()
        {
            string[] filenames = Directory.GetFiles(".", "*.bin");
            foreach (var file in filenames)
            {
                File.Delete(file);
            }
        }
    }
}
