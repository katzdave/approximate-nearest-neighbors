using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ApproxNearestNeighbors.General
{
    class DimWeight
    {
        public readonly int NumDim;
        private List<double> pdf;
        private List<double> cdf;

        public List<double> Pdf
        {
            get { return pdf; }
        }

        public DimWeight(List<double> weights)
        {
            NumDim = weights.Count();
            pdf = new List<double>();
            cdf = new List<double>();

            double wsum = 0;
            for (int i = 0; i < NumDim; i++)
            {
                wsum += weights[i];
            }

            double csum = 0;
            for (int i = 0; i < NumDim; i++)
            {
                double normw = weights[i] / wsum;
                csum += normw;
                pdf.Add(normw);
                cdf.Add(csum);
            }
        }

        public DimWeight(int numdim)
        {
            NumDim = numdim;
            pdf = new List<double>();
            cdf = new List<double>();

            double w = 1.0 / numdim;
            double csum = 0;
            for (int i = 0; i < NumDim; i++)
            {
                csum += w;
                pdf.Add(w);
                cdf.Add(csum);
            }
        }
    }
}
