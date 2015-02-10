using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ApproxNearestNeighbors.General;
using ApproxNearestNeighbors.RandomKDTree;
using ApproxNearestNeighbors.Forest;
using ApproxNearestNeighbors.Brute;

namespace ApproxNearestNeighbors
{
    class Program
    {
        static void Main(string[] args)
        {
            int dim = 10;
            int npoint = 100000;
            int testcases = 10;
            int K = 5;
            int maxSearch = 100;
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

            var tree = new KDTree(ps);

            var bf = new BruteForce(ps);

            var forest = new KDTreeForest(ntrees, ps);

            double mse1 = 0;
            double mse2 = 0;
            double mse3 = 0;

            for (int i = 0; i < testcases; i++)
            {
                List<double> d = new List<double>();
                for (int j = 0; j < dim; j++)
                {
                    d.Add(random.NextDouble());
                }
                Point p = new Point(d);

                var set = bf.GetKNN(p, K);
                var set2 = tree.root.GetANN(p, K, maxSearch);
                var set3 = forest.GetANN(p, K, maxSearch, new DimWeight(p.NumDim));

                for (int e = 0; e < K; e++)
                {
                    mse1 += set.Points[e].ComputeDistance(p, new DimWeight(p.NumDim));
                    mse2 += set2.Points[e].ComputeDistance(p, new DimWeight(p.NumDim));
                    mse3 += set3.Points[e].ComputeDistance(p, new DimWeight(p.NumDim));
                }
            }

            Console.WriteLine(mse1 + " " + mse2 + " " + mse3);
        }
    }
}
