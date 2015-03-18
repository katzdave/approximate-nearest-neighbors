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
        //static void Main(string[] args)
        //{
        //    int dim = 10;
        //    int npoint = 100000;
        //    int testcases = 100;
        //    int K = 5;
        //    int maxSearch = 400;
        //    int ntrees = 3;

        //    Random random = new Random();
        //    PointSet ps = new PointSet(dim);

        //    for (int i = 0; i < npoint; i++)
        //    {
        //        List<double> d = new List<double>();
        //        for(int j=0; j<dim; j++)
        //        {
        //            d.Add(random.NextDouble());
        //        }
        //        ps.AddPoint(new Point(d));
        //    }

        //    Serializer s = new Serializer();
        //    var forestHolder = new KDTreeForestHolder(ps, 1, 10, false);

        //    var weights = new List<double>();

        //    for (int j = 0; j < dim; j++)
        //    {
        //        weights.Add(random.NextDouble());
        //    }

        //    var dw = new DimWeight(dim);
        //    var querydw = new DimWeight(weights);

        //    var dw5 = new DimWeight(weights);
        //    dw5.eliminateBelowThresh(.1);

        //    var dw6 = new DimWeight(weights);
        //    dw6.keepTopK(7);

        //    var tree = new KDTree(ps);
        //    var tree4 = new KDTree(ps, querydw);
        //    var tree5 = new KDTree(ps, dw5);
        //    var tree6 = new KDTree(ps, dw6);

        //    //IFormatter formatter = new BinaryFormatter();
        //    //Stream stream = new FileStream("MyFile.bin", FileMode.Create, FileAccess.Write, FileShare.None);
        //    //formatter.Serialize(stream, tree);
        //    //stream.Close();

        //    //Stream streamDes = new FileStream("MyFile.bin", FileMode.Open, FileAccess.Read, FileShare.Read);
        //    //KDTree treeDes = (KDTree)formatter.Deserialize(streamDes);
        //    //streamDes.Close();

        //    var bf = new BruteForce(ps);

        //    var forest = new KDTreeForest(ntrees, ps);

        //    double mse1 = 0;
        //    double mse2 = 0;
        //    double mse3 = 0;
        //    double mse4 = 0;
        //    double mse5 = 0;
        //    double mse6 = 0;

        //    for (int i = 0; i < testcases; i++)
        //    {
        //        List<double> d = new List<double>();
        //        for (int j = 0; j < dim; j++)
        //        {
        //            d.Add(random.NextDouble());
        //        }
        //        Point p = new Point(d);

        //        //var res = treeDes.root.GetANN(p, K, maxSearch, querydw);

        //        var settst = forestHolder.GetANN(p, querydw, K, 3, .03, maxSearch);

        //        var set = bf.GetKNN(p, K, querydw);
        //        var set2 = tree.root.GetANN(p, K, maxSearch, querydw);
        //        var set3 = forest.GetANN(p, K, maxSearch, querydw);
        //        var set4 = tree4.root.GetANN(p, K, maxSearch, querydw);
        //        var set5 = tree5.root.GetANN(p, K, maxSearch, querydw);
        //        var set6 = tree6.root.GetANN(p, K, maxSearch, querydw);

        //        for (int e = 0; e < K; e++)
        //        {
        //            mse1 += set.Points[e].ComputeDistance(p, querydw);
        //            mse2 += set2.Points[e].ComputeDistance(p, querydw);
        //            mse3 += set3.Points[e].ComputeDistance(p, querydw);
        //            mse4 += set4.Points[e].ComputeDistance(p, querydw);
        //            mse5 += set5.Points[e].ComputeDistance(p, querydw);
        //            mse6 += set6.Points[e].ComputeDistance(p, querydw);
        //        }
        //    }

        //    Console.WriteLine(mse1 + " " + mse2 + " " + mse3 + " " + mse4 + " " + mse5 + " " + mse6);
        //}

        static void Main(string[] args)
        {
            Test_Tree_maxSearch();
            //DRV_Matching_Test();
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
    }
}
