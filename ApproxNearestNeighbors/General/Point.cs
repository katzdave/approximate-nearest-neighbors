﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ApproxNearestNeighbors.General
{
    [Serializable]
    class Point
    {
        public readonly int NumDim;

        private int id;
        public int Id
        {
            get { return id; }
        }

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

        public Point(int dim, Random random)
        {
            NumDim = dim;

            List<double> d = new List<double>();
            for (int j = 0; j < dim; j++)
            {
                d.Add(random.NextDouble());
            }
            values = d;
        }

        public Point(int dim, double value)
        {
            NumDim = dim;

            List<double> d = new List<double>();
            for (int j = 0; j < dim; j++)
            {
                d.Add(value);
            }
            values = d;
        }

        public void setId(int pid)
        {
            id = pid;
        }

        public double ComputeDistance(Point other, DimWeight dw)
        {
            if (NumDim != other.NumDim || NumDim != dw.NumDim)
            {
                return -1;
            }

            double distSq = 0;
            for (int i = 0; i < NumDim; i++)
            {
                distSq += Math.Pow((other.Values[i] - Values[i]) * dw.Pdf[i] * NumDim, 2);
            }

            return Math.Sqrt(distSq);
        }
    }
}
