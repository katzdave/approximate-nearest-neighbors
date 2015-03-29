using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ApproxNearestNeighbors.General
{
    class PointSet
    {
        public static int medianOf = 5;
        Random random = new Random();
        public readonly int NumDim;
        private List<Point> points;
        public List<Point> Points
        {
            get { return points; }
        }

        private int currpid;

        public int NPoints
        {
            get { return points.Count(); }
        }

        public PointSet(int numdim)
        {
            NumDim = numdim;
            currpid = 1;
            points = new List<Point>();
        }

        public PointSet(string filename, int startcol, char delim)
        {
			var rows = System.IO.File.ReadAllLines(filename);
			currpid = 1;
            points = new List<Point>();
			foreach(var row in rows)
			{
				var vals = row.Split(' ');
				List<double> dvals = new List<double>();
				for(int i = startcol; i < vals.Length; i++){
					dvals.Add(Double.Parse(vals[i]));
				}
				NumDim = dvals.Count();
				AddPoint(new Point(dvals));
			}
        }

        public void AddPoint(Point p)
        {
            if (p.NumDim == NumDim)
            {
                p.setId(currpid++);
                points.Add(p);
            }
        }

        public void AddPointInternal(Point p)
        {
            if (p.NumDim == NumDim)
            {
                points.Add(p);
            }
        }

        public void NormalizePoints()
        {
            for (int i = 0; i < NumDim; i++)
            {
                var min = Double.MaxValue;
                var max = Double.MinValue;
                foreach (var point in points)
                {
                    if (point.Values[i] < min)
                    {
                        min = point.Values[i];
                    }
                    if (point.Values[i] > max)
                    {
                        max = point.Values[i];
                    }
                }
                foreach (var point in points)
                {
                    point.Values[i] = (point.Values[i] - min) / (max - min); 
                }
            }
        }

        public double GetMeanDistance(Point p, DimWeight dw)
        {
            double sum = 0;
            foreach (var myp in points)
            {
                sum += myp.ComputeDistance(p, dw);
            }

            return sum / points.Count();
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
                    pss.lower.AddPointInternal(p);
                }
                else if (p.Values[dimNum] > medVal)
                {
                    pss.upper.AddPointInternal(p);
                }
                else if (random.Next(0, 2) == 0)
                {
                    pss.lower.AddPointInternal(p);
                }
                else
                {
                    pss.upper.AddPointInternal(p);
                }
            }

            return pss;
        }

        public int GetLongestDimension(DimWeight dw)
        {
            //return GetHighestVarianceDimension(dw);

            int longestDim = 0;
            double longestDimLen = Double.MinValue;
            for (int i = 0; i < dw.NumDim; i++)
            {
                double mincur = Double.MaxValue;
                double maxcur = Double.MinValue;

                foreach (var point in points)
                {
                    if (point.Values[i] > maxcur)
                        maxcur = point.Values[i];
                    if (point.Values[i] < mincur)
                        mincur = point.Values[i];
                }
                if (((maxcur - mincur) * dw.Pdf[i]) > longestDimLen)
                {
                    longestDim = i;
                    longestDimLen = ((maxcur - mincur) * dw.Pdf[i]);
                }
            }
            return longestDim;
        }

        public int GetHighestVarianceDimension(DimWeight dw)
        {
            int longestDim = 0;
            double longestDimLen = Double.MinValue;
            for (int i = 0; i < dw.NumDim; i++)
            {
                List<double> d = new List<double>();
                foreach (var point in points)
                {
                    d.Add(point.Values[i]);
                }
                var std = stdev(d)*dw.Pdf[i];
                if (std > longestDimLen)
                {
                    longestDim = i;
                    longestDimLen = std;
                }
            }
            return longestDim;
        }

        private static double stdev(List<double> values)
        {
            double ret = 0;
            if (values.Count() > 0)
            {
                //Compute the Average      
                double avg = values.Average();
                //Perform the Sum of (value-avg)_2_2      
                double sum = values.Sum(d => Math.Pow(d - avg, 2));
                //Put it all together      
                ret = Math.Sqrt((sum) / (values.Count() - 1));
            }
            return ret;
        }
    }
}
