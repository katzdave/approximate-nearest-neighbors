using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace ApproxNearestNeighbors.FileIO
{
    class Serializer
    {
        IFormatter formatter = new BinaryFormatter();

        public Serializer()
        {
            string[] filenames = Directory.GetFiles(".", "*.bin");
            foreach (var file in filenames)
            {
                File.Delete(file);
            }
        }

        public void Serialize(string filename, object obj)
        {
            Stream stream = new FileStream(filename + ".bin", FileMode.Create, FileAccess.Write, FileShare.None);
            formatter.Serialize(stream, obj);
            stream.Close();
        }

        public string Serialize(object obj)
        {
            var filename = Guid.NewGuid().ToString();
            Serialize(filename, obj);
            return filename;
        }

        public object Deserialize(string filename)
        {
            Stream streamDes = new FileStream(filename + ".bin", FileMode.Open, FileAccess.Read, FileShare.Read);
            var myObject = formatter.Deserialize(streamDes);
            streamDes.Close();
            return myObject;
        }
    }
}
