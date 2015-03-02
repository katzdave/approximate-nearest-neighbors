using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ApproxNearestNeighbors.General;
using ApproxNearestNeighbors.RandomKDTree;
using ApproxNearestNeighbors.Forest;
using ApproxNearestNeighbors.ForestHolder;
using ApproxNearestNeighbors.Brute;
using ApproxNearestNeighbors.FileIO;
using System.Runtime.Serialization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace ApproxNearestNeighbors
{
    class Program
    {
        static void Main(string[] args)
        {
            int dim = 10;
            int npoint = 100000;
            int testcases = 100;
            int K = 5;
            int maxSearch = 400;
            int ntrees = 3;

            Random random = new Random();
            PointSet ps = new PointSet(dim);

            for (int i = 0; i < npoint; i++)
            {
                List<double> d = new List<double>();
                for(int j=0; j<dim; j++)
                {
                    d.Add(random.NextDouble());
                }
                ps.AddPoint(new Point(d));
            }

            Serializer s = new Serializer();
            var forestHolder = new KDTreeForestHolder(ps, 1, 10, false);

            var weights = new List<double>();

            for (int j = 0; j < dim; j++)
            {
                weights.Add(random.NextDouble());
            }

            var dw = new DimWeight(dim);
            var querydw = new DimWeight(weights);

            var dw5 = new DimWeight(weights);
            dw5.eliminateBelowThresh(.1);

            var dw6 = new DimWeight(weights);
            dw6.keepTopK(7);

            var tree = new KDTree(ps);
            var tree4 = new KDTree(ps, querydw);
            var tree5 = new KDTree(ps, dw5);
            var tree6 = new KDTree(ps, dw6);

            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream("MyFile.bin", FileMode.Create, FileAccess.Write, FileShare.None);
            formatter.Serialize(stream, tree);
            stream.Close();

            Stream streamDes = new FileStream("MyFile.bin", FileMode.Open, FileAccess.Read, FileShare.Read);
            KDTree treeDes = (KDTree)formatter.Deserialize(streamDes);
            streamDes.Close();

            var bf = new BruteForce(ps);

            var forest = new KDTreeForest(ntrees, ps);

            double mse1 = 0;
            double mse2 = 0;
            double mse3 = 0;
            double mse4 = 0;
            double mse5 = 0;
            double mse6 = 0;

            for (int i = 0; i < testcases; i++)
            {
                List<double> d = new List<double>();
                for (int j = 0; j < dim; j++)
                {
                    d.Add(random.NextDouble());
                }
                Point p = new Point(d);

                var res = treeDes.root.GetANN(p, K, maxSearch, querydw);

                var set = bf.GetKNN(p, K, querydw);
                var set2 = tree.root.GetANN(p, K, maxSearch, querydw);
                var set3 = forest.GetANN(p, K, maxSearch, querydw);
                var set4 = tree4.root.GetANN(p, K, maxSearch, querydw);
                var set5 = tree5.root.GetANN(p, K, maxSearch, querydw);
                var set6 = tree6.root.GetANN(p, K, maxSearch, querydw);

                for (int e = 0; e < K; e++)
                {
                    mse1 += set.Points[e].ComputeDistance(p, querydw);
                    mse2 += set2.Points[e].ComputeDistance(p, querydw);
                    mse3 += set3.Points[e].ComputeDistance(p, querydw);
                    mse4 += set4.Points[e].ComputeDistance(p, querydw);
                    mse5 += set5.Points[e].ComputeDistance(p, querydw);
                    mse6 += set6.Points[e].ComputeDistance(p, querydw);
                }
            }

            Console.WriteLine(mse1 + " " + mse2 + " " + mse3 + " " + mse4 + " " + mse5 + " " + mse6);
        }
    }
}
