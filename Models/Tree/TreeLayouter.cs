using LLEAV.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLEAV.Models.Tree
{
    public class TreeLayouter
    {
        private const int X_SPACING = 30; 
        private const int Y_SPACING = 50;

        private Node _root = new Node(null);

        private List<Node> _leafs;

        private List<Edge> _edges;

        private List<Cluster> _clusters;
        private List<Node> _nodes;


        public Tree CalculateTree(FOS currentFOS, bool revert = true)
        {
            _leafs = new List<Node>();
            _edges = new List<Edge>();
            _root.SetText("root");

            if (currentFOS == null || currentFOS.Count() == 0) return new Tree([_root], [], _root);

            // Cast to list to have access to sort function
            _clusters = new List<Cluster>(currentFOS.Clusters);
            _clusters = _clusters.OrderBy(c => c.Count() + c.PositionOfFirstBit() / (float)c.NumberOfBits).ToList();

            //Remove the cluster containing all bits
            if (_clusters.Last().Count() == _clusters.Last().NumberOfBits)
            {
                _clusters.RemoveAt(_clusters.Count-1);
            }

            // Create a new node for each cluster
            _nodes = _clusters.Select(
                c => {
                    var n = new Node(c);
                    n.SetText(c.BitPositions());
                    return n;
                }).ToList(); 

            for (int i = 0; i < _clusters.Count; i++)
            {
                // Find parent
                bool parentFound = false;
                for (int j = i+1; j < _clusters.Count; j++)
                {
                    if (_clusters[j].Contains(_clusters[i]))
                    {
                        _nodes[j].Children.Add(_nodes[i]);
                        parentFound = true;
                        break;
                    }
                }
                if (!parentFound)
                {
                    _root.Children.Add(_nodes[i]);
                }
            }

            _nodes.Add(_root);

            CreateEdges();

            LayoutTree(revert);

            Tree t = new Tree(_nodes, _edges, _root);

            t.CalculateDimensions();

            return t;
        }

        public void UpdateTree(FOS currentFOS, Tree tree, bool revert = true)
        {
            Tree currentTree = CalculateTree(currentFOS);
            
            IList<Node> currentNodes = currentTree.Nodes;

            // Copy because new nodes could be added to tree
            IList<Node> pastNodes = new List<Node>(tree.Nodes);

            foreach (Node pastNode in pastNodes)
            {
                pastNode.IsNewNode = false;

                int index = currentNodes.IndexOf(pastNode);
                if (index >= 0)
                {
                    pastNode.X = currentNodes[index].X;
                    pastNode.Y = currentNodes[index].Y;
                }
                else
                {
                    tree.Nodes.Remove(pastNode);
                }
            }

            foreach(Node current in currentNodes) 
            {
                if (!pastNodes.Contains(current))
                {
                    current.IsNewNode = true;
                    tree.Nodes.Add(current);
                }
            }

            IList<Edge> currentEdges = currentTree.Edges;

            // Copy because new edges could be added to tree
            IList<Edge> pastEdges = new List<Edge>(tree.Edges);


            foreach (Edge pastEdge in pastEdges)
            {
                if (pastEdge.Disappering)
                {
                    tree.Edges.Remove(pastEdge);
                }
                else if (!currentEdges.Contains(pastEdge))
                {
                    pastEdge.Disappering = true;
                }
                else
                {
                    pastEdge.Appearing = false;
                    pastEdge.CalculatePoints(revert);
                }
            }

            foreach (Edge current in currentEdges)
            {
                if (!pastEdges.Contains(current))
                {
                    current.Appearing = true;
                    tree.Edges.Add(current);
                }
            }

            tree.CalculateDimensions();
        }

        private void CreateEdges()
        {
            List<Node> dfs = [_root];
            while (dfs.Count > 0)
            {
                Node current = dfs[0];
                dfs.RemoveAt(0);
                if (current.Children.Count == 0)
                {
                    _leafs.Add(current);
                }
                foreach (Node child in current.Children)
                {
                    dfs.Insert(0,child);

                    Edge newEdge = new Edge(current, child);

                    _edges.Add(newEdge);
                }
            }
        }

        private void LayoutTree(bool revert)
        {
            int posX = 0;
            for (int i = 0; i < _leafs.Count; i++)
            {
                _leafs[i].X = posX;
                posX += _leafs[i].Width + X_SPACING;
            }
            LayoutNode(_root);

            if (revert)
            {
                RevertTree();
            }

            foreach (Edge e in _edges)
            {
                e.CalculatePoints(revert);
            }
        }

        private void RevertTree()
        {
            int maxY = _root.Y + _root.Height;
            List<Node> bfs = [_root];
            while (bfs.Count > 0)
            {
                Node current = bfs[0];
                bfs.AddRange(current.Children);

                current.Y = maxY - current.Y - current.Height;

                bfs.RemoveAt(0);
            }
        }


        private void LayoutNode(Node node)
        {
            if (node.Children.Count == 0) return;

            foreach (Node child in node.Children)
            {
                LayoutNode(child);
            }

            int widthAllChildren = node.Children[0].X + node.Children.Last().X + node.Children.Last().Width;
            node.X = widthAllChildren / 2 - node.Width / 2;

            int maxYAllChildren = node.Children.Max(x => x.Y);

            node.Y = maxYAllChildren + Y_SPACING + node.Height;

        }
    }
}
