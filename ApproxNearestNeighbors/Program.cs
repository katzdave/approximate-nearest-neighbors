using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ApproxNearestNeighbors.General;
using ApproxNearestNeighbors.RandomKDTree;
using ApproxNearestNeighbors.Forest;
using ApproxNearestNeighbors.ForestHolder;
using ApproxNearestNeighbors.Brute;
using ApproxNearestNeighbors.FileIO;
using System.Runtime.Serialization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace ApproxNearestNeighbors
{
    class Program
    {
        static void Main(string[] args)
        {
            //DRV_Matching_Test();
            //Test_Tree_maxSearch();
            //Full_Sys_Test();
            //Full_Sys_Test_ExtremeDRV();
            //Full_Sys_Test_Dimensions();
            //Full_Sys_Test_Alternative();
            //Full_Sys_Test_Trees();
            //Full_Sys_Test_K();
            //Full_Sys_Test_Size();
            //Full_Sys_Test_Nrand();
            Full_Sys_Test_Alternative_Extreme();
        }

        public static void DRV_Matching_Test()
        {
            int dim = 8;
            int npoint = 100000;
            int testcases = 10;
            int dws = 10;
            int K = 20;
            int maxSearch = 500;
            int ntrees = 3;

            Random random = new Random();
            PointSet ps = new PointSet(dim);

            for (int i = 0; i < npoint; i++)
            {
                ps.AddPoint(new Point(dim, random));
            }

            //Indexes
            var bruteForce = new BruteForce(ps);

            var standardTree_r = new KDTree(ps, true);
            var standardTree = new KDTree(ps, false);

            var standardForest_r = new KDTreeForest(ntrees, ps, true);
            var standardForest = new KDTreeForest(ntrees, ps, false);

            //Result holders
            var bruteforceDist = new List<double>();

            var standardDist = new List<double>();
            var queryDist = new List<double>();

            var standardDist_r = new List<double>();
            var queryDist_r = new List<double>();

            var standardDist_f = new List<double>();
            var queryDist_f = new List<double>();

            var standardDist_r_f = new List<double>();
            var queryDist_r_f = new List<double>();

            for (int i = 0; i < dws; i++)
            {
                Console.WriteLine(i);
                var querydw = new DimWeight(dim, random);

                var queryTree = new KDTree(ps, querydw, false);
                var queryTree_r = new KDTree(ps, querydw, true);

                var queryForest = new KDTreeForest(ntrees, ps, querydw, false);
                var queryForest_r = new KDTreeForest(ntrees, ps, querydw, true);

                for (int j = 0; j < testcases; j++)
                {
                    var p = new Point(dim, random);
                    //bruteforceDist.Add(bruteForce.GetKNN(p, K, querydw).GetMeanDistance(p, querydw));
                    var bfd = bruteForce.GetKNN(p, K, querydw).GetMeanDistance(p, querydw);

                    standardDist.Add(standardTree.root.GetANN(p, K, maxSearch, querydw).GetMeanDistance(p, querydw) / bfd - 1);
                    queryDist.Add(queryTree.root.GetANN(p, K, maxSearch, querydw).GetMeanDistance(p, querydw) / bfd - 1);

                    standardDist_r.Add(standardTree_r.root.GetANN(p, K, maxSearch, querydw).GetMeanDistance(p, querydw) / bfd - 1);
                    queryDist_r.Add(queryTree_r.root.GetANN(p, K, maxSearch, querydw).GetMeanDistance(p, querydw) / bfd - 1);

                    //Forests

                    standardDist_f.Add(standardForest.GetANN(p, K, maxSearch, querydw).GetMeanDistance(p, querydw) / bfd - 1);
                    queryDist_f.Add(queryForest.GetANN(p, K, maxSearch, querydw).GetMeanDistance(p, querydw) / bfd - 1);

                    standardDist_r_f.Add(standardForest_r.GetANN(p, K, maxSearch, querydw).GetMeanDistance(p, querydw) / bfd - 1);
                    queryDist_r_f.Add(queryForest_r.GetANN(p, K, maxSearch, querydw).GetMeanDistance(p, querydw) / bfd - 1);
                }
            }

            //Console.WriteLine(bruteforceDist.Average());
            Console.WriteLine(standardDist.Average() + " " + queryDist.Average());
            Console.WriteLine(standardDist_r.Average() + " " + queryDist_r.Average());
            Console.WriteLine(standardDist_f.Average() + " " + queryDist_f.Average());
            Console.WriteLine(standardDist_r_f.Average() + " " + queryDist_r_f.Average());

            //Console.WriteLine(standardDist.Max() + " " + queryDist.Max());
            //Console.WriteLine(standardDist_r.Max() + " " + queryDist_r.Max());
            //Console.WriteLine(standardDist_f.Max() + " " + queryDist_f.Max());
            //Console.WriteLine(standardDist_r_f.Max() + " " + queryDist_r_f.Max());
        }

        public static void Test_Tree_maxSearch()
        {
            int dim = 8;
            int npoint = 100000;
            int testcases = 10;
            int dws = 10;
            int K = 20;
            int maxSearch = 200;
            int ntrees = 3;

            Random random = new Random();
            PointSet ps = new PointSet(dim);

            for (int i = 0; i < npoint; i++)
            {
                ps.AddPoint(new Point(dim, random));
            }

            //Indexes
            var bruteForce = new BruteForce(ps);
            var standardTree = new KDTree(ps, false);

            //Result holders
            var bruteforceDist = new List<double>();
            var standardDist = new List<List<double>>();
            var queryDist = new List<List<double>>();

            //Final Result Holder
            var finStd = new List<double>();
            var finQue = new List<double>();

            for (int i = 0; i < dws; i++)
            {
                Console.WriteLine(i);
                var querydw = new DimWeight(dim, random);
                var queryTree = new KDTree(ps, querydw, false);

                standardDist.Clear();
                queryDist.Clear();

                for (int j = 0; j < testcases; j++)
                {
                    standardDist.Add(new List<double>());
                    queryDist.Add(new List<double>());

                    var p = new Point(dim, random);
                    //bruteforceDist.Add(bruteForce.GetKNN(p, K, querydw).GetMeanDistance(p, querydw));
                    var bfd = bruteForce.GetKNN(p, K, querydw).GetMeanDistance(p, querydw);

                    for (int k = maxSearch; k < 8000; k=k+200)
                    {
                        if (j == 0 && i == 0)
                        {
                            finStd.Add(0);
                            finQue.Add(0);
                        }
                        standardDist[j].Add(standardTree.root.GetANN(p, K, k, querydw).GetMeanDistance(p, querydw)/bfd - 1);
                        queryDist[j].Add(queryTree.root.GetANN(p, K, k, querydw).GetMeanDistance(p, querydw)/bfd - 1);
                    }
                }

                for (int k = 0; k < queryDist[0].Count(); k++)
                {
                    double sumStd = 0;
                    double sumQue = 0;
                    for(int j = 0; j < testcases; j++)
                    {
                        sumStd += standardDist[j][k];
                        sumQue += queryDist[j][k];
                    }

                    sumStd /= testcases;
                    sumQue /= testcases;

                    finStd[k] += sumStd / dws;
                    finQue[k] += sumQue / dws;
                }
            }

            FileStream fs = new FileStream("Results2.csv", FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            for (int i = 0; i < finStd.Count(); i++)
            {
                sw.WriteLine(finStd[i] + "," + finQue[i]);
            }
        }

        public static void Full_Sys_Test()
        {
            int dim = 8;
            int npoint = 100000;
            int testcases = 10;
            int dws = 50;
            int K = 20;
            int maxSearch = 500;
            int ntrees = 3;

            int nrand = 100;
            int ddeter = 2;
            double ratio = .4;
            double prune = 1.5;

            Random random = new Random();
            PointSet ps = new PointSet(dim);

            for (int i = 0; i < npoint; i++)
            {
                ps.AddPoint(new Point(dim, random));
            }

            //Indexes
            var bruteForce = new BruteForce(ps);

            var standardTree = new KDTree(ps, false);
            var standardForest = new KDTreeForestHolder(ps, ddeter, nrand, false, false);

            var standardTree_r = new KDTree(ps, true);
            var standardForest_r = new KDTreeForestHolder(ps, ddeter, nrand, false, true);

            //Result holders
            var standardDist = new List<double>();
            var queryDist = new List<double>();
            var standardDist_f = new List<double>();

            var standardDist_r = new List<double>();
            var queryDist_r = new List<double>();
            var standardDist_f_r = new List<double>();

            for (int i = 0; i < dws; i++)
            {
                Console.WriteLine(i);
                var querydw = new DimWeight(dim, random);

                var queryTree = new KDTree(ps, querydw, false);
                var queryTree_r = new KDTree(ps, querydw, true);

                for (int j = 0; j < testcases; j++)
                {
                    var p = new Point(dim, random);
                    var bfd = bruteForce.GetKNN(p, K, querydw).GetMeanDistance(p, querydw);

                    standardDist.Add(standardTree.root.GetANN(p, K, maxSearch, querydw).GetMeanDistance(p, querydw) / bfd - 1);
                    queryDist.Add(queryTree.root.GetANN(p, K, maxSearch, querydw).GetMeanDistance(p, querydw) / bfd - 1);
                    standardDist_f.Add(standardForest.GetANN(p, querydw, K, ntrees, ratio, prune, maxSearch).GetMeanDistance(p, querydw) / bfd - 1);

                    standardDist_r.Add(standardTree_r.root.GetANN(p, K, maxSearch, querydw).GetMeanDistance(p, querydw) / bfd - 1);
                    queryDist_r.Add(queryTree_r.root.GetANN(p, K, maxSearch, querydw).GetMeanDistance(p, querydw) / bfd - 1);
                    standardDist_f_r.Add(standardForest_r.GetANN(p, querydw, K, ntrees, ratio, prune, maxSearch).GetMeanDistance(p, querydw) / bfd - 1);

                }
            }

            Console.WriteLine(standardDist.Average() + " " + standardDist_f.Average() + " " + queryDist.Average());
            Console.WriteLine(standardDist_r.Average() + " " + standardDist_f_r.Average() + " " + queryDist_r.Average());

            Console.WriteLine(stdev(standardDist) + " " + stdev(standardDist_f) + " " + stdev(queryDist));
            Console.WriteLine(stdev(standardDist_r) + " " + stdev(standardDist_f_r) + " " + stdev(queryDist_r));
        }

        public static void Full_Sys_Test_ExtremeDRV()
        {
            int dim = 8;
            int npoint = 100000;
            int testcases = 10;
            int dws = 50;
            int K = 20;
            int maxSearch = 500;
            int ntrees = 3;

            int nrand = 50;
            int ddeter = 3;
            double ratio = .4;
            double prune = 1.5;
            double selper = .125;

            Random random = new Random();
            PointSet ps = new PointSet(dim);

            for (int i = 0; i < npoint; i++)
            {
                ps.AddPoint(new Point(dim, random));
            }

            ////garbage
            //var p2 = new Point(dim, random);
            //var querydw2 = new DimWeight(dim, 0, random);
            //var queryTree2 = new KDTree(ps, querydw2, false);
            //var tmp = queryTree2.root.GetANN(p2, K, maxSearch, querydw2);

            //Indexes
            var bruteForce = new BruteForce(ps);

            var standardTree = new KDTree(ps, false);
            var standardForest = new KDTreeForestHolder(ps, ddeter, nrand, false, false);

            var standardTree_r = new KDTree(ps, true);
            var standardForest_r = new KDTreeForestHolder(ps, ddeter, nrand, false, true);

            //Result holders
            var standardDist = new List<double>();
            var queryDist = new List<double>();
            var standardDist_f = new List<double>();

            var standardDist_r = new List<double>();
            var queryDist_r = new List<double>();
            var standardDist_f_r = new List<double>();

            for (int i = 0; i < dws; i++)
            {
                Console.WriteLine(i);
                var querydw = new DimWeight(dim, selper, random);

                var queryTree = new KDTree(ps, querydw, false);
                var queryTree_r = new KDTree(ps, querydw, true);

                for (int j = 0; j < testcases; j++)
                {
                    var p = new Point(dim, random);
                    var bfd = bruteForce.GetKNN(p, K, querydw).GetMeanDistance(p, querydw);

                    standardDist.Add(standardTree.root.GetANN(p, K, maxSearch, querydw).GetMeanDistance(p, querydw) / bfd - 1);
                    queryDist.Add(queryTree.root.GetANN(p, K, maxSearch, querydw).GetMeanDistance(p, querydw) / bfd - 1);
                    standardDist_f.Add(standardForest.GetANN(p, querydw, K, ntrees, ratio, prune, maxSearch).GetMeanDistance(p, querydw) / bfd - 1);

                    standardDist_r.Add(standardTree_r.root.GetANN(p, K, maxSearch, querydw).GetMeanDistance(p, querydw) / bfd - 1);
                    queryDist_r.Add(queryTree_r.root.GetANN(p, K, maxSearch, querydw).GetMeanDistance(p, querydw) / bfd - 1);
                    standardDist_f_r.Add(standardForest_r.GetANN(p, querydw, K, ntrees, ratio, prune, maxSearch).GetMeanDistance(p, querydw) / bfd - 1);

                }
            }

            Console.WriteLine(standardDist.Average() + " " + standardDist_f.Average() + " " + queryDist.Average());
            Console.WriteLine(standardDist_r.Average() + " " + standardDist_f_r.Average() + " " + queryDist_r.Average());

            Console.WriteLine(stdev(standardDist) + " " + stdev(standardDist_f) + " " + stdev(queryDist));
            Console.WriteLine(stdev(standardDist_r) + " " + stdev(standardDist_f_r) + " " + stdev(queryDist_r));
        }

        public static void Full_Sys_Test_Dimensions()
        {
            List<int> dims = new List<int>() { 2, 4, 8, 16, 32 };
            int npoint = 100000;
            int testcases = 20;
            int dws = 80;
            int K = 20;
            int maxSearch = 500;
            int ntrees = 3;

            int nrand = 100;
            int ddeter = 1;
            double ratio = .4;
            double prune = 1.5;
            
            var lines = new List<string>();
            string filename = "resultsdim.csv";            

            Random random = new Random();

            foreach (var dim in dims)
            {
                PointSet ps = new PointSet(dim);

                for (int i = 0; i < npoint; i++)
                {
                    ps.AddPoint(new Point(dim, random));
                }

                //Indexes
                var bruteForce = new BruteForce(ps);

                var standardTree = new KDTree(ps, false);
                var standardForest = new KDTreeForestHolder(ps, ddeter, nrand, false, false);

                //Result holders
                var standardDist = new List<double>();
                var queryDist = new List<double>();
                var standardDist_f = new List<double>();

                for (int i = 0; i < dws; i++)
                {
                    Console.WriteLine(dim + " " + i);
                    var querydw = new DimWeight(dim, random);

                    var queryTree = new KDTree(ps, querydw, false);

                    for (int j = 0; j < testcases; j++)
                    {
                        var p = new Point(dim, random);
                        var bfd = bruteForce.GetKNN(p, K, querydw).GetMeanDistance(p, querydw);

                        standardDist.Add(standardTree.root.GetANN(p, K, maxSearch, querydw).GetMeanDistance(p, querydw) / bfd - 1);
                        queryDist.Add(queryTree.root.GetANN(p, K, maxSearch, querydw).GetMeanDistance(p, querydw) / bfd - 1);
                        standardDist_f.Add(standardForest.GetANN(p, querydw, K, ntrees, ratio, prune, maxSearch).GetMeanDistance(p, querydw) / bfd - 1);
                    }
                }

                lines.Add(standardDist.Average() + "," + standardDist_f.Average() + "," + queryDist.Average());
            }

            System.IO.File.WriteAllLines(filename, lines);
        }

        public static void Full_Sys_Test_Alternative_Extreme()
        {
            int dim = 32;
            int testcases = 20;
            int dws = 80;
            int K = 20;
            int maxSearch = 500;
            int ntrees = 3;

            int nrand = 100;
            int ddeter = 2;
            double ratio = .4;
            double prune = 1.5;
            double selper = .03;

            Random random = new Random();

            PointSet ps = new PointSet("ColorHistogram.asc", 1, ' ');
            ps.NormalizePoints();

            //Indexes
            var bruteForce = new BruteForce(ps);

            var standardTree = new KDTree(ps, false);
            var standardForest = new KDTreeForestHolder(ps, ddeter, nrand, true, false);

            //Result holders
            var standardDist = new List<double>();
            var queryDist = new List<double>();
            var standardDist_f = new List<double>();

            for (int i = 0; i < dws; i++)
            {
                Console.WriteLine(dim + " " + i);
                var querydw = new DimWeight(dim, selper, random);

                var queryTree = new KDTree(ps, querydw, false);

                for (int j = 0; j < testcases; j++)
                {
                    var p = ps.Points[random.Next(ps.Points.Count())];
                    var bfd = bruteForce.GetKNN(p, K, querydw).GetMeanDistance(p, querydw);

                    standardDist.Add(standardTree.root.GetANN(p, K, maxSearch, querydw).GetMeanDistance(p, querydw) / bfd - 1);
                    queryDist.Add(queryTree.root.GetANN(p, K, maxSearch, querydw).GetMeanDistance(p, querydw) / bfd - 1);
                    standardDist_f.Add(standardForest.GetANN(p, querydw, K, ntrees, ratio, prune, maxSearch).GetMeanDistance(p, querydw) / bfd - 1);
                }
            }

            var standardDist2 = new List<double>();
            var queryDist2 = new List<double>();
            var standardDist_f2 = new List<double>();

            for (int i = 0; i < standardDist.Count(); i++)
            {
                if (!Double.IsNaN(standardDist[i]) && !Double.IsInfinity(standardDist[i]))
                    standardDist2.Add(standardDist[i]);
                if (!Double.IsNaN(queryDist[i]) && !Double.IsInfinity(queryDist[i]))
                    queryDist2.Add(queryDist[i]);
                if (!Double.IsNaN(standardDist_f[i]) && !Double.IsInfinity(standardDist_f[i]))
                    standardDist_f2.Add(standardDist_f[i]);
            }
            
            Console.WriteLine(standardDist2.Average() + "," + standardDist_f2.Average() + "," + queryDist2.Average());

        }

        public static void Full_Sys_Test_Alternative()
        {
            int dim = 32;
            int testcases = 20;
            int dws = 80;
            int K = 20;
            int maxSearch = 500;
            int ntrees = 3;

            int nrand = 100;
            int ddeter = 1;
            double ratio = .4;
            double prune = 1.5;

            Random random = new Random();

            PointSet ps = new PointSet("ColorHistogram.asc", 1, ' ');
            ps.NormalizePoints();

            //Indexes
            var bruteForce = new BruteForce(ps);

            var standardTree = new KDTree(ps, false);
            var standardForest = new KDTreeForestHolder(ps, ddeter, nrand, false, false);

            //Result holders
            var standardDist = new List<double>();
            var queryDist = new List<double>();
            var standardDist_f = new List<double>();

            for (int i = 0; i < dws; i++)
            {
                Console.WriteLine(dim + " " + i);
                var querydw = new DimWeight(dim, random);

                var queryTree = new KDTree(ps, querydw, false);

                for (int j = 0; j < testcases; j++)
                {
                    var p = ps.Points[random.Next(ps.Points.Count())];
                    var bfd = bruteForce.GetKNN(p, K, querydw).GetMeanDistance(p, querydw);

                    standardDist.Add(standardTree.root.GetANN(p, K, maxSearch, querydw).GetMeanDistance(p, querydw) / bfd - 1);
                    queryDist.Add(queryTree.root.GetANN(p, K, maxSearch, querydw).GetMeanDistance(p, querydw) / bfd - 1);
                    standardDist_f.Add(standardForest.GetANN(p, querydw, K, ntrees, ratio, prune, maxSearch).GetMeanDistance(p, querydw) / bfd - 1);
                }
            }

            Console.WriteLine(standardDist.Average() + "," + standardDist_f.Average() + "," + queryDist.Average());
        }

        public static void Full_Sys_Test_Trees()
        {
            int dim = 8;
            int npoint = 100000;
            int testcases = 20;
            int dws = 80;
            int K = 20;
            int maxSearch = 500;
            var ntree = new List<int>(){1, 5, 10, 20};

            int nrand = 100;
            int ddeter = 1;
            double ratio = .4;
            double prune = 1.5;

            var lines = new List<string>();
            string filename = "resultsdim.csv";

            Random random = new Random();

            PointSet ps = new PointSet(dim);

            for (int i = 0; i < npoint; i++)
            {
                ps.AddPoint(new Point(dim, random));
            }

            //Indexes
            var bruteForce = new BruteForce(ps);

            var standardTree = new KDTree(ps, false);
            var standardForest = new KDTreeForestHolder(ps, ddeter, nrand, false, false);

            //Result holders
            var standardDist = new List<double>();
            var queryDist = new List<double>();
            var standardDist_f = new List<List<double>>();
            for (int i = 0; i < ntree.Count(); i++)
            {
                standardDist_f.Add(new List<double>());
            }

            for (int i = 0; i < dws; i++)
            {
                Console.WriteLine(dim + " " + i);
                var querydw = new DimWeight(dim, random);

                var queryTree = new KDTree(ps, querydw, false);

                for (int j = 0; j < testcases; j++)
                {
                    var p = new Point(dim, random);
                    var bfd = bruteForce.GetKNN(p, K, querydw).GetMeanDistance(p, querydw);

                    standardDist.Add(standardTree.root.GetANN(p, K, maxSearch, querydw).GetMeanDistance(p, querydw) / bfd - 1);
                    queryDist.Add(queryTree.root.GetANN(p, K, maxSearch, querydw).GetMeanDistance(p, querydw) / bfd - 1);
                    int indx = 0;
                    foreach (var ntrees in ntree)
                    {
                        standardDist_f[indx++].Add(standardForest.GetANN(p, querydw, K, ntrees, ratio, prune, maxSearch).GetMeanDistance(p, querydw) / bfd - 1);
                    }
                }
            }

            lines.Add(standardDist.Average() + "," + queryDist.Average());
            foreach (var list in standardDist_f)
            {
                lines.Add(list.Average().ToString());
            }

            System.IO.File.WriteAllLines(filename, lines);
        }

        public static void Full_Sys_Test_nSearch()
        {
            int dim = 8;
            int npoint = 100000;
            int testcases = 20;
            int dws = 80;
            int K = 20;
            var maxSearchs = new List<int>() {50, 200, 500, 2000, 8000};
            int ntrees = 3;

            int nrand = 100;
            int ddeter = 1;
            double ratio = .4;
            double prune = 1.5;

            var lines = new List<string>();
            string filename = "resultsnsearch.csv";

            Random random = new Random();

            PointSet ps = new PointSet(dim);

            for (int i = 0; i < npoint; i++)
            {
                ps.AddPoint(new Point(dim, random));
            }

            //Indexes
            var bruteForce = new BruteForce(ps);

            var standardTree = new KDTree(ps, false);
            var standardForest = new KDTreeForestHolder(ps, ddeter, nrand, false, false);

            //Result holders
            var standardDist = new List<List<double>>();
            var queryDist = new List<List<double>>();
            var standardDist_f = new List<List<double>>();
            for (int i = 0; i < maxSearchs.Count(); i++)
            {
                standardDist.Add(new List<double>());
                queryDist.Add(new List<double>());
                standardDist_f.Add(new List<double>());
            }

            for (int i = 0; i < dws; i++)
            {
                Console.WriteLine(dim + " " + i);
                var querydw = new DimWeight(dim, random);

                var queryTree = new KDTree(ps, querydw, false);

                for (int j = 0; j < testcases; j++)
                {
                    var p = new Point(dim, random);
                    var bfd = bruteForce.GetKNN(p, K, querydw).GetMeanDistance(p, querydw);
                    foreach (var maxSearch in maxSearchs)
                    {
                        int indx = 0;
                        standardDist[indx].Add(standardTree.root.GetANN(p, K, maxSearch, querydw).GetMeanDistance(p, querydw) / bfd - 1);
                        queryDist[indx].Add(queryTree.root.GetANN(p, K, maxSearch, querydw).GetMeanDistance(p, querydw) / bfd - 1);


                        standardDist_f[indx].Add(standardForest.GetANN(p, querydw, K, ntrees, ratio, prune, maxSearch).GetMeanDistance(p, querydw) / bfd - 1);
                        indx++;
                    }
                }
            }

            for (int i = 0; i < standardDist.Count(); i++)
            {
                lines.Add(standardDist[i].Average() + "," + queryDist[i].Average() + "," + standardDist_f[i].Average());
            }

            System.IO.File.WriteAllLines(filename, lines);
        }

        public static void Full_Sys_Test_K()
        {
            int dim = 8;
            int npoint = 100000;
            int testcases = 20;
            int dws = 80;
            var Ks = new List<int>() { 1, 5, 20, 50, 100 };
            int maxSearch = 500;
            int ntrees = 3;

            int nrand = 100;
            int ddeter = 1;
            double ratio = .4;
            double prune = 1.5;

            var lines = new List<string>();
            string filename = "resultsks.csv";

            Random random = new Random();

            PointSet ps = new PointSet(dim);

            for (int i = 0; i < npoint; i++)
            {
                ps.AddPoint(new Point(dim, random));
            }

            //Indexes
            var bruteForce = new BruteForce(ps);

            var standardTree = new KDTree(ps, false);
            var standardForest = new KDTreeForestHolder(ps, ddeter, nrand, false, false);

            //Result holders
            var standardDist = new List<List<double>>();
            var queryDist = new List<List<double>>();
            var standardDist_f = new List<List<double>>();
            for (int i = 0; i < Ks.Count(); i++)
            {
                standardDist.Add(new List<double>());
                queryDist.Add(new List<double>());
                standardDist_f.Add(new List<double>());
            }

            for (int i = 0; i < dws; i++)
            {
                Console.WriteLine(dim + " " + i);
                var querydw = new DimWeight(dim, random);

                var queryTree = new KDTree(ps, querydw, false);

                for (int j = 0; j < testcases; j++)
                {
                    var p = new Point(dim, random);
                    int indx = 0;
                    foreach (var K in Ks)
                    {
                        var bfd = bruteForce.GetKNN(p, K, querydw).GetMeanDistance(p, querydw);
                        standardDist[indx].Add(standardTree.root.GetANN(p, K, maxSearch, querydw).GetMeanDistance(p, querydw) / bfd - 1);
                        queryDist[indx].Add(queryTree.root.GetANN(p, K, maxSearch, querydw).GetMeanDistance(p, querydw) / bfd - 1);
                        standardDist_f[indx].Add(standardForest.GetANN(p, querydw, K, ntrees, ratio, prune, maxSearch).GetMeanDistance(p, querydw) / bfd - 1);
                        indx++;
                    }
                }
            }

            for (int i = 0; i < standardDist.Count(); i++)
            {
                lines.Add(standardDist[i].Average() + "," + queryDist[i].Average() + "," + standardDist_f[i].Average());
            }

            System.IO.File.WriteAllLines(filename, lines);
        }

        public static void Full_Sys_Test_Size()
        {
            int dim = 8;
            var npoints = new List<int>() { 500000, 1000000 };
            int testcases = 20;
            int dws = 80;
            int K = 20;
            int maxSearch = 500;
            int ntrees = 3;

            int nrand = 100;
            int ddeter = 1;
            double ratio = .4;
            double prune = 1.5;

            var lines = new List<string>();
            string filename = "resultssize.csv";

            Random random = new Random();

            foreach (var npoint in npoints)
            {
                PointSet ps = new PointSet(dim);

                for (int i = 0; i < npoint; i++)
                {
                    ps.AddPoint(new Point(dim, random));
                }

                //Indexes
                var bruteForce = new BruteForce(ps);

                var standardTree = new KDTree(ps, false);
                var standardForest = new KDTreeForestHolder(ps, ddeter, nrand, true, false);

                //Result holders
                var standardDist = new List<double>();
                var queryDist = new List<double>();
                var standardDist_f = new List<double>();

                for (int i = 0; i < dws; i++)
                {
                    Console.WriteLine(dim + " " + i);
                    var querydw = new DimWeight(dim, random);

                    var queryTree = new KDTree(ps, querydw, false);

                    for (int j = 0; j < testcases; j++)
                    {
                        var p = new Point(dim, random);
                        var bfd = bruteForce.GetKNN(p, K, querydw).GetMeanDistance(p, querydw);

                        standardDist.Add(standardTree.root.GetANN(p, K, maxSearch, querydw).GetMeanDistance(p, querydw) / bfd - 1);
                        queryDist.Add(queryTree.root.GetANN(p, K, maxSearch, querydw).GetMeanDistance(p, querydw) / bfd - 1);
                        double val = standardForest.GetANN(p, querydw, K, ntrees, ratio, prune, maxSearch).GetMeanDistance(p, querydw) / bfd - 1;
                        if (!Double.IsNaN(val) && val < 1000)
                        {
                            standardDist_f.Add(val);
                        }
                    }
                }

                lines.Add(standardDist.Average() + "," + standardDist_f.Average() + "," + queryDist.Average());
            }

            System.IO.File.WriteAllLines(filename, lines);
        }

        public static void Full_Sys_Test_Nrand()
        {
            int dim = 8;
            int npoint = 100000;
            int testcases = 20;
            int dws = 80;
            int K = 20;
            int maxSearch = 500;
            int ntrees = 3;

            var nrands = new List<int>() { 20, 50, 100, 200, 400 };
            int ddeter = 1;
            double ratio = .4;
            double prune = 1.5;

            var lines = new List<string>();
            string filename = "resultsnrand.csv";

            Random random = new Random();

            foreach (var nrand in nrands)
            {
                PointSet ps = new PointSet(dim);

                for (int i = 0; i < npoint; i++)
                {
                    ps.AddPoint(new Point(dim, random));
                }

                //Indexes
                var bruteForce = new BruteForce(ps);

                var standardTree = new KDTree(ps, false);
                var standardForest = new KDTreeForestHolder(ps, ddeter, nrand, false, false);

                //Result holders
                var standardDist = new List<double>();
                var queryDist = new List<double>();
                var standardDist_f = new List<double>();

                for (int i = 0; i < dws; i++)
                {
                    Console.WriteLine(dim + " " + i);
                    var querydw = new DimWeight(dim, random);

                    var queryTree = new KDTree(ps, querydw, false);

                    for (int j = 0; j < testcases; j++)
                    {
                        var p = new Point(dim, random);
                        var bfd = bruteForce.GetKNN(p, K, querydw).GetMeanDistance(p, querydw);

                        standardDist.Add(standardTree.root.GetANN(p, K, maxSearch, querydw).GetMeanDistance(p, querydw) / bfd - 1);
                        queryDist.Add(queryTree.root.GetANN(p, K, maxSearch, querydw).GetMeanDistance(p, querydw) / bfd - 1);
                        double val = standardForest.GetANN(p, querydw, K, ntrees, ratio, prune, maxSearch).GetMeanDistance(p, querydw) / bfd - 1;
                        if (!Double.IsNaN(val) && val < 1000)
                        {
                            standardDist_f.Add(val);
                        }
                    }
                }

                lines.Add(standardDist.Average() + "," + standardDist_f.Average() + "," + queryDist.Average());
            }

            System.IO.File.WriteAllLines(filename, lines);
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
