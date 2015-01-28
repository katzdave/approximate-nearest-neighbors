using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ApproxNearestNeighbors.General;

namespace ApproxNearestNeighbors.RandomKDTree
{
    class KDTreeNode
    {
        private KDTreeNode parent;

        private bool isLeaf;
        private Point point;

        private int splitDim;
        private KDTreeNode leftChild;
        private KDTreeNode rightChild;

        public KDTreeNode(PointSet ps, KDTreeNode parent)
        {
            this.parent = parent;
            if (ps.Points.Count() > 1)
            {
                isLeaf = false;
                Random random = new Random();
                splitDim = random.Next(0, ps.NumDim);
                var pss = ps.PartitionMedian(splitDim);
                point = pss.median;
                leftChild = new KDTreeNode(pss.lower, this);
                rightChild = new KDTreeNode(pss.upper, this);
            }
            else if (ps.Points.Count() == 1)
            {
                isLeaf = true;
                point = ps.Points[0];
            }
            else
            {
                isLeaf = true;
            }
        }

        public PointSet GetANN(Point p, int K, int maxSearch)
        {
            return GetANN(p, K, maxSearch, new DimWeight(p.NumDim));
        }

        public PointSet GetANN(Point p, int K, int maxSearch, DimWeight dw)
        {
            var pc = new PointCompare(dw, p);
            var heap = new MaxHeap<Point>(pc);

            SearchDown(p, K, maxSearch, dw, new PointSet(p.NumDim), heap, pc);

            PointSet ps = new PointSet(p.NumDim);
            while (heap.Count > 0)
            {
                ps.AddPoint(heap.ExtractDominating());
            }

            return ps;
        }

        private void SearchDown(Point p, int K, int maxSearch, DimWeight dw, PointSet searched, MaxHeap<Point> heap, PointCompare pc)
        {
            if (this == null || this.point == null || searched.NPoints >= maxSearch)
                return;

            if (heap.Count < K)
            {
                heap.Add(point);
            }
            else if (pc.Compare(heap.GetMin(), point) > 0)
            {
                heap.ExtractDominating();
                heap.Add(point);
            }

            searched.AddPoint(point);

            if (!isLeaf)
            {
                bool leftSearched;
                if (p.Values[splitDim] < point.Values[splitDim])
                {
                    leftChild.SearchDown(p, K, maxSearch, dw, searched, heap, pc);
                    leftSearched = true;
                }
                else
                {
                    rightChild.SearchDown(p, K, maxSearch, dw, searched, heap, pc);
                    leftSearched = false;
                }

                //Hyperplane math here to determine if this next search should be done
                if (leftSearched)
                {
                    rightChild.SearchDown(p, K, maxSearch, dw, searched, heap, pc);
                }
                else
                {
                    leftChild.SearchDown(p, K, maxSearch, dw, searched, heap, pc);
                }
            }       
        }
    }
}
