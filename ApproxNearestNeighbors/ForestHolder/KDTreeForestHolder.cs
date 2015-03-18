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
        public readonly bool ToDisk;

        private List<DimWeight> dimWeights;
        private Dictionary<Point, string> filenames;
        private Dictionary<Point, KDTree> pointmap;
        private PointSet dimset;
        private KDTree dimtree;
        private BruteForce bf;
        
        private DimWeight uniformdw;

        private Serializer serializer = new Serializer();

        public KDTreeForestHolder(PointSet ps, int depthDeter, int nRandom, bool toDisk, bool useRandom)
        {
            NumDim = ps.NumDim;
            ToDisk = toDisk;
            dimWeights = new List<DimWeight>();
            filenames = new Dictionary<Point, string>();
            pointmap = new Dictionary<Point, KDTree>();
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
                var point = new Point(dw.Pdf);
                if (ToDisk)
                {
                    var filename = serializer.Serialize(new KDTree(ps, dw, useRandom));
                    filenames.Add(point, filename);
                }
                else
                {
                    pointmap.Add(point, new KDTree(ps, dw, useRandom));
                }
                dimset.AddPoint(point);
            }

            bf = new BruteForce(dimset);
            dimtree = new KDTree(dimset, useRandom);

            var bestTrees = getKBestTreeLinear(new DimWeight(ps.NumDim), 4);
        }

        public PointSet GetANN(Point p, DimWeight dw, int k, int ktree, double ratio, int maxSearch)
        {
            int nSearchTrees = (int) Math.Round((maxSearch)*(1-ratio));
            if (nSearchTrees > dimWeights.Count())
            {
                nSearchTrees = dimWeights.Count();
            }
            int nSearchForest = maxSearch - nSearchTrees;

            var trees = getKBestTreeKD(dw, ktree, nSearchForest);
            var forest = new KDTreeForest(trees);
            return forest.GetANNWeighted(p, k, nSearchTrees, dw);
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
            var queryp = new Point(dw.Pdf);
            var points = bf.GetKNN(queryp, k);
            return pointsToTrees(points, queryp);
        }

        private List<KDTree> getKBestTreeKD(DimWeight dw, int k, int maxSearch)
        {
            var queryp = new Point(dw.Pdf);
            var points = dimtree.root.GetANN(queryp, k, maxSearch);
            return pointsToTrees(points, queryp);
        }

        private List<KDTree> pointsToTrees(PointSet points, Point queryp)
        {
            var result = new List<KDTree>();

            foreach (var point in points.Points)
            {
                KDTree tree;
                if (ToDisk)
                {
                    string filename;
                    filenames.TryGetValue(point, out filename);
                    tree = (KDTree)serializer.Deserialize(filename);
                }
                else
                {
                    pointmap.TryGetValue(point, out tree);
                }
                tree.SetQuality(queryp);
                result.Add(tree);
            }

            return result;
        }
    }
}
