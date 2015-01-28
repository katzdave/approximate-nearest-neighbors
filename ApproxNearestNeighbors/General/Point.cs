using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ApproxNearestNeighbors.General
{
    class Point
    {
        public readonly int NumDim;

        private List<double> values;
        public List<double> Values
        {
            get { return values; }
            set
            {
                if (value.Count == NumDim)
                    values = value;
            }
        }

        public Point(List<double> v)
        {
            values = v;
            NumDim = values.Count();
        }

        //public double ComputeDistance(Point other)
        //{
        //    if (other.NumDim != NumDim)
        //    {
        //        return -1;
        //    }

        //    double distSq = 0;
        //    for (int i = 0; i < NumDim; i++)
        //    {
        //        distSq += Math.Pow((other.Values[i] - Values[i]) / NumDim, 2);
        //    }

        //    return Math.Sqrt(distSq);
        //}

        public double ComputeDistance(Point other, DimWeight dw)
        {
            if (NumDim != other.NumDim || NumDim != dw.NumDim)
            {
                return -1;
            }

            double distSq = 0;
            for (int i = 0; i < NumDim; i++)
            {
                distSq += Math.Pow((other.Values[i] - Values[i]) * dw.Pdf[i], 2);
            }

            return Math.Sqrt(distSq);
        }
    }
}
