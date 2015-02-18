using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ApproxNearestNeighbors.General;
using ApproxNearestNeighbors.RandomKDTree;
using ApproxNearestNeighbors.Forest;

namespace ApproxNearestNeighbors.ForestHolder
{
    class KDTreeForestHolder
    {
        public readonly int NumDim;

        private List<DimWeight> dimWeights;
        private List<KDTree> kdTrees;

        public KDTreeForestHolder(PointSet ps, int depthDeter, int nRandom)
        {
            NumDim = ps.NumDim;
            dimWeights = new List<DimWeight>();
            kdTrees = new List<KDTree>();

            for (int i = 0; i < depthDeter; i++)
            {
                kSubset(new List<double>(), NumDim, depthDeter);
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
                kdTrees.Add(new KDTree(ps, dw));
            }
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
    }
}
