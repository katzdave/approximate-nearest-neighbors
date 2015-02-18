using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ApproxNearestNeighbors.General;

namespace ApproxNearestNeighbors.RandomKDTree
{
    [Serializable]
    class KDTree
    {
        public KDTreeNode root;
        public readonly DimWeight splitWeight;

        public KDTree(PointSet ps)
        {
            splitWeight = new DimWeight(ps.NumDim);
            root = new KDTreeNode(ps, null, splitWeight);
        }

        public KDTree(PointSet ps, DimWeight dw)
        {
            splitWeight = dw;
            root = new KDTreeNode(ps, null, splitWeight);
        }
    }
}
