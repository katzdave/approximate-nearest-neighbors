using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ApproxNearestNeighbors.General;

namespace ApproxNearestNeighbors.RandomKDTree
{
    class KDTreeForest
    {
        public readonly int NTrees;
        private List<KDTree> trees;

        public KDTreeForest(int ntrees, PointSet ps)
        {
            NTrees = ntrees;
            for (int i = 0; i < ntrees; i++)
            {
                trees.Add(new KDTree(ps));
            }
        }
    }
}
