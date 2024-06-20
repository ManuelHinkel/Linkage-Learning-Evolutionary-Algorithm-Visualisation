using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Point = Avalonia.Point;

namespace LLEAV.Models.Tree
{
    /// <summary>
    /// Represents the tree structure used for visualization.
    /// </summary>
    public class Tree : ReactiveObject
    {
        /// <summary>
        /// Gets the originating FOS for this tree.
        /// </summary>
        public FOS OriginatingFOS { get; }

        /// <summary>
        /// Gets or sets the width of the tree.
        /// </summary>
        [Reactive]
        public int Width { get; private set; }

        /// <summary>
        /// Gets or sets the height of the tree.
        /// </summary>
        [Reactive]
        public int Height { get; private set; }

        /// <summary>
        /// Gets or sets the collection of nodes in the tree.
        /// </summary>
        public ObservableCollection<Node> Nodes { get; set; }

        /// <summary>
        /// Gets or sets the collection of edges in the tree.
        /// </summary>
        public ObservableCollection<Edge> Edges { get; set; }

        /// <summary>
        /// Gets or sets the root node of the tree.
        /// </summary>
        public Node Root { get; set; }

        /// <summary>
        /// Initializes a new instance of the tree with the specified nodes, edges, and root node.
        /// </summary>
        /// <param name="nodes">The nodes in the tree.</param>
        /// <param name="edges">The edges in the tree.</param>
        /// <param name="root">The root node of the tree.</param>
        public Tree(IList<Node> nodes, IList<Edge> edges, Node root)
        {
            Nodes = new ObservableCollection<Node>(nodes);
            Edges = new ObservableCollection<Edge>(edges);
            Root = root;
        }

        /// <summary>
        /// Calculates the dimensions of the tree (width and height) based on the positions and sizes of the nodes.
        /// </summary>
        public void CalculateDimensions()
        {
            Width = 0;
            Height = 0;
            foreach (Node n in Nodes)
            {
                int currentHeight = n.Height + n.Y;
                int currentWidth = n.Width + n.X;
                if (currentHeight > Height) { Height = currentHeight; }
                if (currentWidth > Width) { Width = currentWidth; }
            }

            Width = (int)(Width * 1.02f);
            Height = (int)(Height * 1.02f);
        }
    }

    /// <summary>
    /// Represents a node in the tree structure used for visualization.
    /// </summary>
    public class Node : ReactiveObject
    {
        /// <summary>
        /// Gets or sets the x position of the node.
        /// </summary>
        [Reactive]
        public int X { get; set; }

        /// <summary>
        /// Gets or sets the y position of the node.
        /// </summary>
        [Reactive]
        public int Y { get; set; }

        /// <summary>
        /// Gets the text shown in the node.
        /// </summary>
        [Reactive]
        public string Text { get; private set; }

        /// <summary>
        /// Gets or sets if the node is new.
        /// </summary>
        [Reactive]
        public bool IsNewNode { get; set; }


        /// <summary>
        /// Gets or sets the color of the node.
        /// </summary>
        [Reactive]
        public string Color { get; set; } = "";

        /// <summary>
        /// Gets the width of the node.
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        /// Gets or sets the height of the node.
        /// </summary>
        public int Height { get; private set; }

        /// <summary>
        /// Gets or sets the children of the node.
        /// </summary>
        public IList<Node> Children { get; set; } = new List<Node>();

        /// <summary>
        /// Gets the cluster associated to the node
        /// </summary>
        public Cluster Cluster { get; private set; }

        /// <summary>
        /// Initializes a new instance of a node with the specified cluster.
        /// </summary>
        public Node(Cluster cluster)
        {
            Cluster = cluster;
        }

        /// <summary>
        /// Sets the text of the node and adjusts the node's width and height based on the text length.
        /// </summary>
        /// <param name="text">The text to set for the node.</param>
        public void SetText(string text)
        {
            Text = text;
            Width = (25 + Text.Length * 6);
            Height = (int)(20 + Math.Sqrt(Width));
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current node.
        /// </summary>
        /// <param name="obj">The object to compare with the current node.</param>
        /// <returns>true if the specified object is equal to the current node; otherwise, false.</returns>
        public override bool Equals(object? obj)
        {
            if (obj == null || !(obj is Node))
            {
                return false;
            }
            else if (Cluster == null)
            {
                return ((Node)obj).Cluster == null;
            }
            else if (((Node)obj).Cluster == null)
            {
                return Cluster == null;
            }
            else
            {
                return Cluster.Mask.Equals(((Node)obj).Cluster.Mask);
            }
        }

        /// <summary>
        /// Gets the leaf nodes of the tree starting from the current node.
        /// </summary>
        /// <returns>A list of leaf nodes.</returns>
        public IList<Node> GetLeafs()
        {
            List<Node> leafs = new List<Node>();
            List<Node> bfs = [this];
            while (bfs.Count > 0)
            {
                Node current = bfs[0];

                if (current.Children.Count == 0)
                {
                    leafs.Add(current);
                }
                else
                {
                    bfs.AddRange(current.Children);
                }

                bfs.RemoveAt(0);
            }
            return leafs;
        }
    }

    /// <summary>
    /// Represents an edge in the tree structure used for visualization.
    /// </summary>
    public class Edge : ReactiveObject
    {
        /// <summary>
        /// Gets the starting point of the edge.
        /// </summary>
        [Reactive]
        public Point Start { get; private set; }

        /// <summary>
        /// Gets the ending point of the edge.
        /// </summary>
        [Reactive]
        public Point End { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether the edge is appearing.
        /// </summary>
        [Reactive]
        public bool Appearing { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the edge is disappearing.
        /// </summary>
        [Reactive]
        public bool Disappering { get; set; }

        private Node _startNode;
        private Node _endNode;

        /// <summary>
        /// Initializes a new instance of thean edge with the specified start and end nodes.
        /// </summary>
        /// <param name="startNode">The starting node of the edge.</param>
        /// <param name="endNode">The ending node of the edge.</param>
        public Edge(Node startNode, Node endNode)
        {
            _startNode = startNode;
            _endNode = endNode;
        }

        /// <summary>
        /// Calculates the start and end points of the edge based on the positions of the connected nodes.
        /// </summary>
        /// <param name="revert">If set to true,calculate points bases on areverse tree.</param>
        public void CalculatePoints(bool revert)
        {
            if (revert)
            {
                Start = new Point(_startNode.X + _startNode.Width / 2, _startNode.Y + _startNode.Height);
                End = new Point(_endNode.X + _endNode.Width / 2, _endNode.Y);
            }
            else
            {
                Start = new Point(_startNode.X + _startNode.Width / 2, _startNode.Y);
                End = new Point(_endNode.X + _endNode.Width / 2, _endNode.Y + _endNode.Height);
            }
        }

        /// <summary>
        /// Recalculates whether the edge is appearing based on the new node status of the connected nodes.
        /// </summary>
        public void RecalcAppearing()
        {
            Appearing = _startNode.IsNewNode || _endNode.IsNewNode;
        }
    }

}
