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
        public int id;

        public KDTree(PointSet ps, bool useRandom)
        {
            splitWeight = new DimWeight(ps.NumDim);
            root = new KDTreeNode(ps, null, splitWeight, useRandom);
        }

        public KDTree(PointSet ps, DimWeight dw, bool useRandom)
        {
            splitWeight = dw;
            root = new KDTreeNode(ps, null, splitWeight, useRandom);

        }

        public void SetQuality(Point querydw)
        {
            Point p = new Point(splitWeight.Pdf);
            Quality = p.ComputeDistance(querydw, new DimWeight(p.NumDim));
        }
    }
}
