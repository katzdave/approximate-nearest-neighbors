﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ApproxNearestNeighbors.General
{
    class PointSet
    {
        public static int medianOf = 5;

        public readonly int NumDim;
        private List<Point> points;
        public List<Point> Points
        {
            get { return points; }
        }

        public int NPoints
        {
            get { return points.Count(); }
        }

        public PointSet(int numdim)
        {
            NumDim = numdim;
            points = new List<Point>();
        }

        public void AddPoint(Point p)
        {
            if (p.NumDim == NumDim)
                points.Add(p);
        }

        public void NormalizePoints()
        {

        }

        public PointSetSplit PartitionMedian(int dimNum)
        {
            if (dimNum >= NumDim)
            {
                throw new Exception("Dimension Number too High");
            }
            List<double> coords = new List<double>();
            if (points.Count() <= medianOf)
            {
                foreach (var p in points)
                {
                    coords.Add(p.Values[dimNum]);
                }
            }
            else
            {
                List<int> selected = new List<int>();
                Random random = new Random();
                int r;
                for (int i = 0; i < medianOf; i++)
                {
                    do
                    {
                        r = random.Next(0, points.Count());
                    } while (selected.Contains(r));
                    selected.Add(r);
                    coords.Add(points[r].Values[dimNum]);
                }
            }
            coords.Sort();
            double medVal;
            if (coords.Count() % 2 == 0)
            {
                medVal = (coords[coords.Count() / 2] + coords[coords.Count() / 2 - 1]) / 2;
            }
            medVal = coords[coords.Count() / 2];

            PointSetSplit pss = new PointSetSplit(NumDim);

            foreach (var p in points)
            {
                if (p.Values[dimNum] == medVal && pss.median == null)
                {
                    pss.median = p;
                }
                else if (p.Values[dimNum] < medVal)
                {
                    pss.lower.AddPoint(p);
                }
                else
                {
                    pss.upper.AddPoint(p);
                }
            }

            return pss;
        }
    }
}