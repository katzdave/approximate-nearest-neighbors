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

        public double Quality;

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

        public void SetQuality(Point querydw)
        {
            Point p = new Point(splitWeight.Pdf);
            Quality = p.ComputeDistance(querydw, new DimWeight(p.NumDim));
        }
    }
}
