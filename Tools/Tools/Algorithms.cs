using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Tools
{
    class Algorithms
    {
        class Node
        {
            public Dictionary<int, Path> connections;
            public int value;
            public Node(int v)
            {
                value = v;
                connections = new Dictionary<int, Path>();
            }
        }

        class Path
        {
            public int weight;
            public int start, end;
            public Path(int s, int e, int w)
            {
                start = s;
                end = e;
                weight = w;
            }
        }

        static Node[] nodes;

        static void temp(string[] args)
        {
            //Task task = MDPStreams.query(args);
            //task.Wait();
            StreamReader inputReader = new StreamReader(@"D:\Contest\4.txt");
            StreamWriter outputWriter = new StreamWriter(@"D:\Contest\4.solution.txt");

            string line = inputReader.ReadLine();
            int T = int.Parse(line);
            for (int count = 0; count < T; count++)
            {
                line = inputReader.ReadLine();
                int n = int.Parse(line);
                Console.WriteLine("N : " + n);
                int graphSize = 2 * n + 2;

                nodes = new Node[graphSize];
                for (int i = 0; i < n; i++)
                {
                    nodes[i] = new Node(i);
                    nodes[i + n] = new Node(i);
                }
                nodes[2 * n] = new Node(0);
                nodes[2 * n + 1] = new Node(n);

                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        int numI = i + 1;
                        int numJ = j + 1;
                        int indexI = i;
                        int indexJ = j + n;
                        if ((numI & numJ) == 0)
                        {
                            Path p = new Path(indexI, indexJ, 1);
                            nodes[indexI].connections.Add(indexJ, p);
                        }
                    }
                }

                for (int i = 0; i < n; i++)
                {
                    Path sourcePath = new Path(2 * n, i, 1);
                    nodes[2 * n].connections.Add(i, sourcePath);
                    Path sinkPath = new Path(i, 2 * n + 1, 1);
                    nodes[i + n].connections.Add(2 * n + 1, sinkPath);
                }

                //Console.WriteLine("Before: ");
                //printMatrix();
                int maxFlow = FordFulkerson(2 * n, 2 * n + 1);
                //Console.WriteLine("After: ");
                //printMatrix();
                //Console.WriteLine("");

                if (maxFlow == n)
                {
                    Console.WriteLine("YES");
                    outputWriter.WriteLine("YES");
                    for (int i = 0; i < n; i++)
                    {
                        if (i != 0)
                        {
                            Console.Write(" ");
                            outputWriter.Write(" ");
                        }
                        Console.Write(nodes[i + n].connections.First().Value.end + 1);
                        outputWriter.Write(nodes[i + n].connections.First().Value.end + 1);
                    }
                    Console.WriteLine();
                    outputWriter.WriteLine();
                }
                else
                {
                    Console.WriteLine("NO");
                    outputWriter.WriteLine("NO");
                }
                Console.WriteLine();
            }


            outputWriter.Close();
            inputReader.Close();

            Console.WriteLine("Done");
            Console.ReadLine();
        }

        static int FordFulkerson(int start, int end)
        {
            int maxFlow = 0;
            int[] path = new int[nodes.Length];
            for (int i = 0; i < path.Length; i++)
            {
                path[i] = -1;
            }
            while (BFS(start, end, path))
            {
                int pathFlow = int.MaxValue;
                int index = end;
                while (index != start)
                {
                    pathFlow = Math.Min(pathFlow, nodes[path[index]].connections[index].weight);
                    index = path[index];
                }
                maxFlow += pathFlow;

                index = end;
                while (index != start)
                {
                    int nextIndex = path[index];
                    nodes[nextIndex].connections[index].weight -= pathFlow;
                    if (nodes[nextIndex].connections[index].weight == 0)
                    {
                        nodes[nextIndex].connections.Remove(index);
                    }
                    if (!nodes[index].connections.ContainsKey(nextIndex))
                    {
                        Path p = new Path(index, nextIndex, 0);
                        nodes[index].connections.Add(nextIndex, p);
                    }
                    nodes[index].connections[nextIndex].weight += pathFlow;
                    index = nextIndex;
                }
            }
            return maxFlow;
        }
        static bool BFS(int start, int end, int[] path)
        {
            List<int> visited = new List<int>();
            List<int> queue = new List<int>();

            queue.Add(start);
            visited.Add(start);

            while (queue.Count > 0)
            {
                int index = queue[0];
                queue.RemoveAt(0);

                foreach (Path p in nodes[index].connections.Values)
                {
                    if (!visited.Contains(p.end))
                    {
                        queue.Add(p.end);
                        visited.Add(p.end);
                        path[p.end] = index;
                    }
                }
            }
            return visited.Contains(end);
        }

        static void printMatrix()
        {
            for (int i = 0; i < nodes.Length; i++)
            {
                for (int j = 0; j < nodes.Length; j++)
                {
                    if (nodes[i].connections.ContainsKey(j))
                    {
                        Console.Write(nodes[i].connections[j].weight + " ");
                    }
                    else
                    {
                        Console.Write(0 + " ");
                    }
                }
                Console.WriteLine();
            }
        }
    }
}
