using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ApproxNearestNeighbors.General;
using ApproxNearestNeighbors.RandomKDTree;
using ApproxNearestNeighbors.Brute;

namespace ApproxNearestNeighbors
{
    class Program
    {
        static void Main(string[] args)
        {
            int dim = 3;
            int npoint = 10000;
            int testcases = 1;
            int K = 5;
            int maxSearch = 500;

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

            var node = new KDTreeNode(ps, null);

            var bf = new BruteForce(ps);

            for (int i = 0; i < testcases; i++)
            {
                List<double> d = new List<double>();
                for (int j = 0; j < dim; j++)
                {
                    d.Add(random.NextDouble());
                }
                Point p = new Point(d);

                var set = bf.GetKNN(p, K);
                var set2 = node.GetANN(p, K, maxSearch);

                for (int e = 0; e < K; e++)
                {
                    Console.WriteLine(set.Points[e].ComputeDistance(p, new DimWeight(p.NumDim)));
                    Console.WriteLine(set2.Points[e].ComputeDistance(p, new DimWeight(p.NumDim)));
                    Console.WriteLine();
                }
            }
        }
    }
}
