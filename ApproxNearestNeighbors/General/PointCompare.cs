using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ApproxNearestNeighbors.General
{
    class PointCompare : Comparer<Point>
    {
        public readonly DimWeight Dw;
        public readonly Point P;

        public PointCompare(DimWeight dw, Point p)
        {
            Dw = dw;
            P = p;
        }

        public override int Compare(Point p1, Point p2)
        {
            double d1, d2;
            d1 = p1.ComputeDistance(P, Dw);
            d2 = p2.ComputeDistance(P, Dw);

            if (d1 > d2)
                return 1;
            if (d1 < d2)
                return -1;
            return 0;
        }
    }
}
