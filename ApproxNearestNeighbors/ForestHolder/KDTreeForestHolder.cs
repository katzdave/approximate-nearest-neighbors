using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ApproxNearestNeighbors.General;
using ApproxNearestNeighbors.RandomKDTree;
using ApproxNearestNeighbors.Forest;
using ApproxNearestNeighbors.FileIO;
using ApproxNearestNeighbors.Brute;

namespace ApproxNearestNeighbors.ForestHolder
{
    class KDTreeForestHolder
    {
        public readonly int NumDim;

        private List<DimWeight> dimWeights;
        private Dictionary<Point, string> filenames;
        private PointSet dimset;
        private KDTree dimtree;
        private BruteForce bf;
        
        private DimWeight uniformdw;

        private Serializer serializer = new Serializer();

        public KDTreeForestHolder(PointSet ps, int depthDeter, int nRandom)
        {
            NumDim = ps.NumDim;
            dimWeights = new List<DimWeight>();
            filenames = new Dictionary<Point, string>();
            dimset = new PointSet(ps.NumDim);
            uniformdw = new DimWeight(ps.NumDim);

            for (int i = 1; i <= depthDeter; i++)
            {
                kSubset(new List<double>(), NumDim, i);
            }

            Random random = new Random();
            for (int i = 0; i < nRandom; i++)
            {
                var weights = new List<double>();

                for (int j = 0; j < NumDim; j++)
                {
                    weights.Add(random.NextDouble());
                }

                dimWeights.Add(new DimWeight(weights));
            }

            int h = 0;
            foreach (var dw in dimWeights)
            {
                Console.WriteLine(h++);
                var filename = serializer.Serialize(new KDTree(ps, dw));
                var point = new Point(dw.Pdf);
                filenames.Add(point, filename);
                dimset.AddPoint(point);
            }

            bf = new BruteForce(dimset);
            dimtree = new KDTree(dimset);

            var bestTrees = getKBestTreeLinear(new DimWeight(ps.NumDim), 4);
            var hello = 2;
        }

        private void kSubset(List<double> prev, int n, int k)
        {
            if (n == 0 || n < k || k == 0)
            {
                if (k == 0)
                {
                    for (int i = prev.Count; i < NumDim; i++)
                    {
                        prev.Add(0);
                    }
                    dimWeights.Add(new DimWeight(prev));
                }
                return;
            }
            var listCopy1 = new List<double>(prev);
            listCopy1.Add(0);
            kSubset(listCopy1, n - 1, k);
            var listCopy2 = new List<double>(prev);
            listCopy2.Add(1);
            kSubset(listCopy2, n - 1, k - 1);
        }

        private List<KDTree> getKBestTreeLinear(DimWeight dw, int k)
        {
            var result = new List<KDTree>();
            var queryp = new Point(dw.Pdf);

            var points = bf.GetKNN(queryp, k);
            foreach (var point in points.Points)
            {
                string filename;
                filenames.TryGetValue(point, out filename);
                result.Add((KDTree) serializer.Deserialize(filename));
            }

            return result;
        }
    }
}
