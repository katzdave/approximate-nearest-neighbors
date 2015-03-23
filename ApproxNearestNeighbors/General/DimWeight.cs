using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ApproxNearestNeighbors.General
{
    [Serializable]
    class DimWeight
    {
        public readonly int NumDim;
        private List<double> pdf;
        private List<double> cdf;

        private Random rand = new Random();

        public List<double> Pdf
        {
            get { return pdf; }
        }

        public DimWeight(List<double> weights)
        {
            NumDim = weights.Count();

            setPdfCdf(weights);
        }

        private void setPdfCdf(List<double> weights)
        {
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

        public DimWeight(int dim, Random random)
        {
            var weights = new List<double>();

            for (int j = 0; j < dim; j++)
            {
                weights.Add(random.NextDouble());
            }

            NumDim = dim;
            setPdfCdf(weights);
        }

        public DimWeight(int dim, double selper, Random random)
        {
            var weights = new List<double>();

            int include = random.Next(0, dim);

            for (int j = 0; j < dim; j++)
            {
                if (j == include || random.NextDouble() < selper)
                {
                    weights.Add(random.NextDouble());
                }
                else
                {
                    weights.Add(0);
                }
            }

            NumDim = dim;
            setPdfCdf(weights);
        }

        public int getRandomDim()
        {
            double val = rand.NextDouble();

            int l = 0;
            int r = NumDim - 1;
            int m;

            while (r > l)
            {
                m = (l + r) / 2;
                if (cdf[m] > val)
                {
                    r = m;
                }
                else if (cdf[m] < val)
                {
                    l = m + 1;
                }
            }

            return r;
        }

        public void eliminateBelowThresh(double thresh)
        {
            var newWeights = new List<double>();

            for (int i = 0; i < NumDim; i++)
            {
                if (pdf[i] < thresh)
                {
                    newWeights.Add(0);
                }
                else
                {
                    newWeights.Add(pdf[i]);
                }
            }

            setPdfCdf(newWeights);
        }

        public void keepTopK(int K)
        {
            var currDim = new List<int>();
            var currVal = new List<double>();

            for (int i = 0; i < NumDim; i++)
            {
                if (currDim.Count() < K)
                {
                    currDim.Add(i);
                    currVal.Add(pdf[i]);
                }
                else
                {
                    int minDim = 0;
                    double minVal = 2;
                    for (int j = 0; j < K; j++)
                    {
                        if (currVal[j] < minVal)
                        {
                            minDim = j;
                            minVal = currVal[j];
                        }
                    }

                    if (pdf[i] > minVal)
                    {
                        currDim.RemoveAt(minDim);
                        currVal.RemoveAt(minDim);
                        currDim.Add(i);
                        currVal.Add(pdf[i]);
                    }
                }
            }

            var newWeights = new List<double>();

            for (int i = 0; i < NumDim; i++)
            {
                if (currDim.Contains(i))
                {
                    newWeights.Add(pdf[i]);
                }
                else
                {
                    newWeights.Add(0);
                }
            }

            setPdfCdf(newWeights);
        }
    }
}
