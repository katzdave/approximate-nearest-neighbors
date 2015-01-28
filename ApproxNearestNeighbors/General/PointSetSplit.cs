using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ApproxNearestNeighbors.General
{
    class PointSetSplit
    {
        public PointSet lower;
        public PointSet upper;
        public Point median;

        public PointSetSplit(int numdim)
        {
            lower = new PointSet(numdim);
            upper = new PointSet(numdim);
        }
    }
}
