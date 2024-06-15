using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using Avalonia;
using DynamicData;
using LLEAV.Models;
using LLEAV.Util;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Point = Avalonia.Point;

namespace LLEAV.Models.Tree
{
    public class Tree: ReactiveObject
    {
        public FOS OriginatingFOS { get; }

        [Reactive]
        public int Width { get; private set; }
        [Reactive]
        public int Height { get; private set; } 

        public ObservableCollection<Node> Nodes { get; set; }
        public ObservableCollection<Edge> Edges { get; set; }

        public Node Root { get;  set; }

        public Tree(IList<Node> nodes, IList<Edge> edges, Node root)
        {
            Nodes = new ObservableCollection<Node>(nodes);
            Edges = new ObservableCollection<Edge>(edges);
            Root = root;
        }

        public void CalculateDimensions()
        {
            Width = 0;
            Height = 0;
            foreach (Node n in Nodes)
            {
                int currentHeight = n.Height + n.Y;
                int currentWidth = n.Width + n.X;   
                if (currentHeight > Height) { Height = currentHeight; }
                if (currentWidth > Width) {  Width = currentWidth; }
            }

            Width = (int)(Width * 1.02f);
            Height = (int)(Height * 1.02f);
        }
    }

    public abstract class TreeElement : ReactiveObject
    {
    }

    public class Node : TreeElement
    {
        [Reactive]
        public int X { get; set; }
        [Reactive]
        public int Y { get; set; }

        [Reactive]
        public string Text { get; private set; }
        [Reactive]
        public bool IsNewNode { get; set; }

        [Reactive]
        public string Color { get; set; } = "";

        public int Width { get; private set; }
        public int Height { get; private set; }

        public IList<Node> Children { get; set; } = new List<Node>();

        public Cluster Cluster { get; private set; }

        public Node(Cluster cluster)
        {
            Cluster = cluster;
        }

        public void SetText(string text)
        {
            Text = text;
            Width = (25 + Text.Length * 6);
            Height = (int)(20 + Math.Sqrt(Width));
        }

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
    }

    public class Edge : TreeElement
    {
        [Reactive]
        public Point Start { get; private set; }
        [Reactive]
        public Point End { get; private set; }
        [Reactive]
        public bool Appearing { get; set; }
        [Reactive]
        public bool Disappering { get; set; }

        private Node _startNode;
        private Node _endNode;

        public Edge(Node startNode, Node endNode)
        {
            _startNode = startNode;
            _endNode = endNode;
        }

        public void CalculatePoints(bool revert)
        {
            if (revert)
            {
                Start = new Point(_startNode.X + _startNode.Width / 2, _startNode.Y + _startNode.Height);
                End = new Point(_endNode.X + _endNode.Width / 2, _endNode.Y);
            } else
            {
                Start = new Point(_startNode.X + _startNode.Width / 2, _startNode.Y);
                End = new Point(_endNode.X + _endNode.Width / 2, _endNode.Y + _endNode.Height);
            }
        }

        public void RecalcAppearing()
        {
            Appearing = _startNode.IsNewNode || _endNode.IsNewNode;
        }
    }

}
