using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace task2
{
    class Edge
    {
        public int maxStream;
        public int currentStream;
        public string @from;
        public string to;

        public Edge(string @from, string to, int maxStream)
        {
            this.@from = @from;
            this.to = to;
            this.maxStream = maxStream;
            this.currentStream = 0;
        }

        public int GetH(string startNode)
        {
            if (startNode == @from)
            {
                return maxStream - currentStream;
            }

            if (startNode == to)
            {
                return currentStream;
            }

            throw new Exception("НЕ ПОНЯЛ");
        }

        public override string ToString()
        {
            return $"{@from} -> {to} : {currentStream}/{maxStream}";
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var graph = ParseGraph();

            Solve(graph);
            WriteResult(graph);
        }

        public static Dictionary<string, List<Edge>> ParseGraph()
        {
            var file = new StreamReader("in.txt");
            var firstLine = file.ReadLine().Split(' ');
            var k = int.Parse(firstLine[0]);
            var l = int.Parse(firstLine[1]);
            var nodeToEdges = new Dictionary<string, List<Edge>>();
            nodeToEdges.Add("s", new List<Edge>());
            for (var i = 1; i <= k; i++)
            {
                nodeToEdges.Add("x" + i, new List<Edge>());
                nodeToEdges["s"].Add(new Edge("s", "x" + i, 1));
            }

            for (var i = 1; i <= l; i++)
            {
                nodeToEdges.Add("y" + i, new List<Edge>());
                nodeToEdges["y" + i].Add(new Edge("y" + i, "t", 1));
            }

            var arraySize = int.Parse(file.ReadLine());

            var nodes = file.ReadToEnd()
                .Replace("\r", "")
                .Replace("\n", " ")
                .Trim()
                .Split();

            for (var xNumber = 1; xNumber <= k; xNumber++)
            {
                var xNode = "x" + xNumber;
                var startPos = int.Parse(nodes[xNumber - 1]);
                var endPos = xNumber == k ? arraySize - 1 : int.Parse(nodes[xNumber]);
                for (var pos = startPos; pos < endPos; pos++)
                {
                    if (nodes[pos] == "0")
                        continue;
                    var yNode = "y" + nodes[pos];
                    var edge = new Edge(xNode, yNode, 1);
                    nodeToEdges[xNode].Add(edge);
                    nodeToEdges[yNode].Add(edge);
                }
            }

            file.Close();

            return nodeToEdges;
        }

        public static void Solve(Dictionary<string, List<Edge>> nodeToEdges)
        {
            var chain = TryGetChain(nodeToEdges);
            while (chain != null)
            {
                for (var i = 0; i < chain.Count - 1; i++)
                {
                    var edge = nodeToEdges[chain[i]].First(x => x.to == chain[i + 1] || x.@from == chain[i + 1]);
                    if (edge.@from == chain[i])
                    {
                        edge.currentStream = 1;
                    }
                    else
                    {
                        edge.currentStream = 0;
                    }
                }

                chain = TryGetChain(nodeToEdges);
            }
        }

        public static List<string> TryGetChain(Dictionary<string, List<Edge>> nodeToEdges)
        {
            var queue = new Queue<string>();
            var previous = new Dictionary<string, string>();
            queue.Enqueue("s");
            previous.Add("s", "-1");
            while (queue.Count != 0)
            {
                var current = queue.Dequeue();
                foreach (var edge in nodeToEdges[current].Where(x => x.GetH(current) != 0))
                {
                    var newNode = "";
                    if (current == edge.@from)
                        newNode = edge.to;
                    else
                        newNode = edge.@from;
                    if (previous.ContainsKey(newNode))
                    {
                        continue;
                    }

                    queue.Enqueue(newNode);
                    previous.Add(newNode, current);
                    if (newNode == "t")
                    {
                        var result = new List<string> {"t"};
                        while (previous.ContainsKey(current))
                        {
                            result.Add(current);
                            current = previous[current];
                        }

                        result.Reverse();
                        return result;
                    }
                }
            }

            return null;
        }

        public static void WriteResult(Dictionary<string, List<Edge>> nodeToEdges)
        {
            var file = new StreamWriter("out.txt");
            var k = nodeToEdges.Count(kv => kv.Key.StartsWith("x"));
            for (var i = 1; i <= k; i++)
            {
                var edge = nodeToEdges["x" + i].FirstOrDefault(x => x.currentStream != 0);
                if (edge is not null)
                {
                    file.Write(edge.to);
                }
                else
                {
                    file.Write(0);
                }

                if (i != k)
                {
                    file.Write(" ");
                }
            }

            file.Flush();
            file.Close();
        }
    }
}
