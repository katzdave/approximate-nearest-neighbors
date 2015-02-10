using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ApproxNearestNeighbors.General
{
    class PointSetHash
    {
        private HashSet<int> pids = new HashSet<int>();

        public int NPoints
        {
            get { return pids.Count(); }
        }

        public void AddPoint(Point p)
        {
            pids.Add(p.Id);
        }

        public bool CheckContains(Point p)
        {
            return pids.Contains(p.Id);
        }
    }
}
