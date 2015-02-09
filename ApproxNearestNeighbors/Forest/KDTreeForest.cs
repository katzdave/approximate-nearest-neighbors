using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using ApproxNearestNeighbors.General;
using ApproxNearestNeighbors.RandomKDTree;

namespace ApproxNearestNeighbors.Forest
{
    class KDTreeForest
    {
        public readonly int NTrees;
        private List<KDTree> trees;

        private List<Boolean> childHolds;
        private List<Mutex> mutexes;
        private Dictionary<int, int> threadIds; //Converts threadId to mutexId
        private List<Thread> threads;
        private List<Boolean> returned;

        private Point p;
        private int K;
        private int maxSearch;
        private DimWeight dw;
        private PointSet searched;
        MaxHeap<Point> heap;
        PointCompare pc;

        public KDTreeForest(int ntrees, PointSet ps)
        {
            NTrees = ntrees;
            trees = new List<KDTree>();
            for (int i = 0; i < ntrees; i++)
            {
                trees.Add(new KDTree(ps));
            }
        }

        public PointSet GetANN(Point p, int K, int maxSearch, DimWeight dw)
        {
            this.p = p;
            this.K = K;
            this.maxSearch = maxSearch;
            this.dw = dw;
            searched = new PointSet(p.NumDim);
            pc = new PointCompare(dw, p);
            heap = new MaxHeap<Point>(pc);
            
            childHolds = new List<bool>();
            mutexes = new List<Mutex>();
            threadIds = new Dictionary<int, int>();
            threads = new List<Thread>();
            returned = new List<bool>();

            for (int currNum = 0; currNum < NTrees; currNum++)
            {
                mutexes.Add(new Mutex());
                childHolds.Add(false);
                returned.Add(false);
                mutexes[currNum].WaitOne();
                Thread t = new Thread(new ThreadStart(searchStuff));
                threadIds.Add(t.ManagedThreadId, currNum);
                threads.Add(t);
                t.Start();
            }
            while(!returned[0])
                performAction(0);

            //performAction(1);
            //performAction(0);

            cleanupThreads();

            PointSet ps = new PointSet(p.NumDim);
            while (heap.Count > 0)
            {
                ps.AddPoint(heap.ExtractDominating());
            }

            return ps;
        }

        private void printStuff()
        {
            int id = System.Threading.Thread.CurrentThread.ManagedThreadId;
            threadIds.TryGetValue(id, out id);
            while (true)
            {
                if (!childHolds[id])
                {
                    mutexes[id].WaitOne();
                    childHolds[id] = true;
                    Console.WriteLine(id + " got mutex");
                    mutexes[id].ReleaseMutex();
                }
            }
        }

        private void searchStuff()
        {
            int id = System.Threading.Thread.CurrentThread.ManagedThreadId;
            threadIds.TryGetValue(id, out id);
            trees[id].root.SearchDownThreaded(p, K, maxSearch, dw, searched, heap, pc, childHolds, mutexes, id);
            returned[id] = true;
        }

        private void performAction(int id)
        {
            mutexes[id].ReleaseMutex();
            while (!childHolds[id]) ;
            mutexes[id].WaitOne();
            childHolds[id] = false;
        }

        private void cleanupThreads()
        {
            foreach (var thread in threads)
            {
                if(thread.IsAlive)
                    thread.Abort();
            }
        }
    }
}
