using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ApproxNearestNeighbors.General;

namespace ApproxNearestNeighbors.Brute
{
    class BruteForce
    {
        private PointSet pointset;

        public BruteForce(PointSet ps)
        {
            pointset = ps;
        }

        public PointSet GetKNN(Point p, int K)
        {
            return GetKNN(p, K, new DimWeight(p.NumDim));
        }

        public PointSet GetKNN(Point p, int K, DimWeight dw)
        {
            var pc = new PointCompare(dw, p);
            var heap = new MaxHeap<Point>(pc);

            for (int i = 0; i < pointset.Points.Count(); i++)
            {
                if (heap.Count < K)
                {
                    heap.Add(pointset.Points[i]);
                }
                else if (pc.Compare(heap.GetMin(), pointset.Points[i]) > 0)
                {
                    heap.ExtractDominating();
                    heap.Add(pointset.Points[i]);
                }
            }

            PointSet ps = new PointSet(p.NumDim);

            while (heap.Count > 0)
            {
                ps.AddPoint(heap.ExtractDominating());
            }

            return ps;
        }
    }
}
