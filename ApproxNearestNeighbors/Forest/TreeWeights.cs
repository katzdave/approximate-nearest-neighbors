using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ApproxNearestNeighbors.General;
using ApproxNearestNeighbors.RandomKDTree;

namespace ApproxNearestNeighbors.Forest
{
    class TreeWeights
    {
        private Random random;
        public List<KDTree> Trees;
        private List<double> pdf;
        private List<double> cdf;

        private const double epsilon = 1e-10;

        public TreeWeights(List<KDTree> inTrees)
        {
            Trees = inTrees;
            random = new Random();

            foreach (var tree in inTrees)
            {
                tree.Quality = 1 / (epsilon + tree.Quality);
            }

            setPdfCdf();
            pruneQuality(1 / (Trees.Count() * 2));
            setPdfCdf();
        }

        private void pruneQuality(double thresh)
        {
            var indices = new List<int>();
            for (int i = 0; i < Trees.Count(); i++)
            {
                if (pdf[i] < thresh)
                {
                    indices.Add(i);
                }
            }

            for (int i = (indices.Count() - 1); i >= 0; i--)
            {
                Trees.RemoveAt(indices[i]);
            }

            setPdfCdf();
        }

        private void setPdfCdf()
        {
            pdf = new List<double>();
            cdf = new List<double>();

            double wsum = 0;
            for (int i = 0; i < Trees.Count(); i++)
            {
                wsum += Trees[i].Quality;
            }

            double csum = 0;
            for (int i = 0; i < Trees.Count(); i++)
            {
                double normw = Trees[i].Quality / wsum;
                csum += normw;
                pdf.Add(normw);
                cdf.Add(csum);
            }
        }

        public void RemoveTree(int id)
        {
            int indx = -1;
            for (int i = 0; i < Trees.Count(); i++)
            {
                if (Trees[i].id == id)
                {
                    indx = i;
                    break;
                }
            }
            Trees.RemoveAt(indx);
            setPdfCdf();
        }

        public int GetRandomId()
        {
            double val = random.NextDouble();

            int l = 0;
            int r = Trees.Count() - 1;
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

            return Trees[r].id;
        }
    }
}
