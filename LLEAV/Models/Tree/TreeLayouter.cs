using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace LLEAV.Models.Tree
{
    /// <summary>
    /// A class responsible for laying out a tree structure for visualization.
    /// </summary>
    public class TreeLayouter
    {
        private const int X_SPACING = 50;
        private const int Y_SPACING = 50;

        private Node _root = new Node(null);

        private List<Node> _leafs;

        private List<Edge> _edges;

        private List<Cluster> _clusters;
        private List<Node> _nodes;


        /// <summary>
        /// Calculates a tree layout based on the given FOS.
        /// </summary>
        /// <param name="currentFOS">The current FOS to generate the tree from.</param>
        /// <param name="revert">Indicates whether to revert the tree vertically.</param>
        /// <returns>A tree representing the layout.</returns>
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
                _clusters.RemoveAt(_clusters.Count - 1);
            }

            // Create a new node for each cluster
            _nodes = _clusters.Select(
                c =>
                {
                    var n = new Node(c);
                    n.SetText(c.BitPositions());
                    return n;
                }).ToList();

            for (int i = 0; i < _clusters.Count; i++)
            {
                // Find parent
                bool parentFound = false;
                for (int j = i + 1; j < _clusters.Count; j++)
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

        /// <summary>
        /// Updates the tree layout based on the given FOS and existing tree.
        /// </summary>
        /// <param name="currentFOS">The current FOS to generate the tree from.</param>
        /// <param name="tree">The existing tree layout.</param>
        /// <param name="revert">Indicates whether to revert the tree vertically.</param>
        /// <returns>A <tree representing the updated layout.</returns>
        public Tree UpdateTree(FOS currentFOS, Tree tree, bool revert = true)
        {
            Tree currentTree = CalculateTree(currentFOS);

            foreach (Node current in currentTree.Nodes)
            {
                if (!tree.Nodes.Contains(current))
                {
                    current.IsNewNode = true;
                }
            }

            foreach (Edge currentEdge in currentTree.Edges)
            {
                if (!tree.Edges.Contains(currentEdge))
                {
                    currentEdge.Appearing = true;
                }
            }

            foreach (Edge pastEdge in tree.Edges)
            {
                if (!pastEdge.Disappering && !currentTree.Edges.Contains(pastEdge))
                {
                    pastEdge.Appearing = false;
                    pastEdge.Disappering = true;
                    currentTree.Edges.Add(pastEdge);
                }
            }

            return currentTree;
        }

        /// <summary>
        /// Creates edges between nodes.
        /// </summary>
        private void CreateEdges()
        {
            List<Node> dfs = [_root];
            while (dfs.Count > 0)
            {
                Node current = dfs[0];
                dfs.RemoveAt(0);
                if (current.Children.Count == 0)
                {
                    _leafs.Insert(0, current);
                }
                foreach (Node child in current.Children)
                {
                    dfs.Insert(0, child);

                    Edge newEdge = new Edge(current, child);

                    _edges.Add(newEdge);
                }
            }
        }

        /// <summary>
        /// Lays out the tree by positioning nodes and calculating edge points.
        /// </summary>
        /// <param name="revert">Indicates whether to revert the tree vertically.</param>
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

        /// <summary>
        /// Reverts the tree vertically by adjusting node positions.
        /// </summary>
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

        /// <summary>
        /// Recursively layouts the nodes, positioning them based on their children.
        /// </summary>
        /// <param name="node">The node to layout.</param>
        private void LayoutNode(Node node)
        {
            if (node.Children.Count == 0) return;

            foreach (Node child in node.Children)
            {
                LayoutNode(child);
            }


            var leafs = node.GetLeafs();
            int widthAllLeafs = leafs[0].X + leafs.Last().X + leafs.Last().Width;
            node.X = widthAllLeafs / 2 - node.Width / 2;

            int maxYAllChildren = node.Children.Max(x => x.Y);

            node.Y = maxYAllChildren + Y_SPACING + node.Height;

        }
    }
}
