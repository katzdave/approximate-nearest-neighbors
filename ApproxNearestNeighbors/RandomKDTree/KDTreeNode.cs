using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using ApproxNearestNeighbors.General;

namespace ApproxNearestNeighbors.RandomKDTree
{
    [Serializable]
    class KDTreeNode
    {
        public KDTreeNode parent;

        private bool isLeaf;
        private Point point;

        private int splitDim;
        private KDTreeNode leftChild;
        private KDTreeNode rightChild;

        //For dummy node only
        public KDTreeNode(int dim, KDTreeNode child)
        {
            isLeaf = false;
            leftChild = child;
            splitDim = 0;
            point = new Point(dim, 1000);
        }

        public KDTreeNode(PointSet ps, KDTreeNode parent, DimWeight dwsplit, bool useRandom)
        {
            this.parent = parent;
            if (ps.Points.Count() > 1)
            {
                isLeaf = false;
                if(useRandom)
                    splitDim = dwsplit.getRandomDim();
                else
                    splitDim = ps.GetLongestDimension(dwsplit);
                var pss = ps.PartitionMedian(splitDim);
                point = pss.median;
                leftChild = new KDTreeNode(pss.lower, this, dwsplit, useRandom);
                rightChild = new KDTreeNode(pss.upper, this, dwsplit, useRandom);
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
            var searched = new PointSet(p.NumDim);

            SearchDown(p, K, maxSearch, dw, searched, heap, pc);

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

            if (isLeaf)
            {
                CheckPoint(K, heap, pc);
                searched.AddPoint(point);
            }
            else
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

                // Check this point
                if (searched.NPoints < maxSearch)
                {
                    CheckPoint(K, heap, pc);
                    searched.AddPoint(point);
                }

                // Check if a better point possibly exists in the other subtree
                var pval = new List<Double>(p.Values);
                pval[splitDim] = point.Values[splitDim];
                Point planecheck = new Point(pval);

                if (pc.Compare(heap.GetMin(), planecheck) >= 0)
                {
                    if (leftSearched && rightChild != null)
                    {
                        rightChild.SearchDown(p, K, maxSearch, dw, searched, heap, pc);
                    }
                    else if(!leftSearched && leftChild != null)
                    {
                        leftChild.SearchDown(p, K, maxSearch, dw, searched, heap, pc);
                    }
                }
            }       
        }

        public void SearchDownThreaded(Point p, int K, int maxSearch, DimWeight dw, PointSetHash searched,
            MaxHeap<Point> heap, PointCompare pc, List<Boolean> b, List<Mutex> m, int id)
        {
            if (this == null || this.point == null || searched.NPoints >= maxSearch)
                return;

            if (isLeaf)
            {
                while (b[id]) ;
                m[id].WaitOne();
                b[id] = true;

                if (!searched.CheckContains(point) && searched.NPoints < maxSearch)
                {
                    CheckPoint(K, heap, pc);
                    searched.AddPoint(point);
                }

                m[id].ReleaseMutex();
            }
            else
            {
                bool leftSearched;
                if (p.Values[splitDim] < point.Values[splitDim])
                {
                    leftChild.SearchDownThreaded(p, K, maxSearch, dw, searched, heap, pc, b, m, id);
                    leftSearched = true;
                }
                else
                {
                    rightChild.SearchDownThreaded(p, K, maxSearch, dw, searched, heap, pc, b, m, id);
                    leftSearched = false;
                }

                // Check this point
                while (b[id]) ;
                m[id].WaitOne();
                b[id] = true;

                bool exceeded = maxSearch <= searched.NPoints;

                if (!searched.CheckContains(point) && searched.NPoints < maxSearch)
                {
                    CheckPoint(K, heap, pc);
                    searched.AddPoint(point);
                }

                // Check if a better point possibly exists in the other subtree
                var pval = new List<Double>(p.Values);
                pval[splitDim] = point.Values[splitDim];
                Point planecheck = new Point(pval);
                bool expandOther = pc.Compare(heap.GetMin(), planecheck) >= 0;

                m[id].ReleaseMutex();

                if (expandOther)
                {
                    if (leftSearched && rightChild != null)
                    {
                        rightChild.SearchDownThreaded(p, K, maxSearch, dw, searched, heap, pc, b, m, id);
                    }
                    else if (!leftSearched && leftChild != null)
                    {
                        leftChild.SearchDownThreaded(p, K, maxSearch, dw, searched, heap, pc, b, m, id);
                    }
                }
            }
        }

        private void CheckPoint(int K, MaxHeap<Point> heap, PointCompare pc)
        {
            if (heap.Count < K)
            {
                heap.Add(point);
            }
            else if (pc.Compare(heap.GetMin(), point) > 0)
            {
                heap.ExtractDominating();
                heap.Add(point);
            }
        }
    }
}
