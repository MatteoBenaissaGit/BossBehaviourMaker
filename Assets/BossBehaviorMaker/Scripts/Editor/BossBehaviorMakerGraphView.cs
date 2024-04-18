﻿using System;
using System.Collections.Generic;
using System.Linq;
using BossBehaviorMaker.Scripts.Decorators;
using BossBehaviorMaker.Scripts.Runtime;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace BossBehaviorMaker.Scripts.Editor
{
    public class BossBehaviorMakerGraphView : GraphView
    {
        public new class UxmlFactory : UxmlFactory<BossBehaviorMakerGraphView, UxmlTraits>
        {
        }

        private BehaviorTreeBbm _tree;
        private bool _hasTree => _tree != null;

        public BossBehaviorMakerGraphView()
        {
            style.flexGrow = 1f;
            Insert(0, new GridBackground());
            AddManipulators();
            
            CreateMiniMap();
        }

        private void CreateMiniMap()
        {
            MiniMap miniMap = new MiniMap { anchored = true, 
                capabilities = Capabilities.Movable | Capabilities.Selectable | Capabilities.Resizable};
            
            miniMap.SetPosition(new Rect(19, 30, 100, 100));
            miniMap.maxHeight = 100;
            miniMap.maxWidth = 100;

            StyleColor backgroundColor = new StyleColor(new Color32(29, 29, 30, 255));
            StyleColor borderColor = new StyleColor(new Color32(51, 51, 51, 255));
            miniMap.style.backgroundColor = backgroundColor;
            miniMap.style.borderTopColor = borderColor;
            miniMap.style.borderRightColor = borderColor;
            miniMap.style.borderBottomColor = borderColor;
            miniMap.style.borderTopColor = borderColor;
            
            Add(miniMap);
        }

        private void AddManipulators()
        {
            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
        }

        public void PopulateView(BehaviorTreeBbm behaviorTreeBbm)
        {
            _tree = behaviorTreeBbm;

            graphViewChanged -= OnGraphViewChanged;
            DeleteElements(graphElements);
            graphViewChanged += OnGraphViewChanged;

            if (_hasTree == false)
            {
                return;
            }
            
            // Load all nodes from the tree in the node list
            string parentPath = AssetDatabase.GetAssetPath(_tree);
            UnityEngine.Object[] allChildren = AssetDatabase.LoadAllAssetsAtPath(parentPath);
            foreach (UnityEngine.Object obj in allChildren)
            {
                NodeBbm node = obj as NodeBbm;
                if (_tree.Nodes.Contains(node) == false && node != null)
                {
                    _tree.Nodes.Add(node);
                }
            }
            _tree.Nodes.Remove(null);
            
            //add nodes
            _tree.Nodes.ForEach(CreateNodeView);

            //aad edges
            foreach (NodeBbm node in _tree.Nodes)
            {
                BossBehaviorMakerNodeView nodeView = GetNodeByGuid(node.Guid) as BossBehaviorMakerNodeView;
                foreach (NodeBbm child in _tree.GetChildren(node))
                {
                    BossBehaviorMakerNodeView childView = GetNodeByGuid(child.Guid) as BossBehaviorMakerNodeView;
                    Edge edge = nodeView?.OutputPort.ConnectTo(childView?.InputPort);
                    if (edge == null)
                    {
                        continue;
                    }

                    AddElement(edge);
                }
            }
        }

        private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
            if (graphViewChange.elementsToRemove != null)
            {
                foreach (GraphElement element in graphViewChange.elementsToRemove)
                {
                    BossBehaviorMakerNodeView nodeView = element as BossBehaviorMakerNodeView;
                    if (nodeView != null)
                    {
                        DeleteNode(nodeView.Node);
                    }

                    Edge edge = element as Edge;
                    if (edge != null)
                    {
                        BossBehaviorMakerNodeView parentView = edge.output.node as BossBehaviorMakerNodeView;
                        BossBehaviorMakerNodeView childView = edge.input.node as BossBehaviorMakerNodeView;
                        if (parentView == null || childView == null)
                        {
                            continue;
                        }

                        _tree.RemoveChild(parentView.Node, childView.Node);
                    }
                }
            }

            if (graphViewChange.edgesToCreate != null && _hasTree)
            {
                foreach (Edge edge in graphViewChange.edgesToCreate)
                {
                    BossBehaviorMakerNodeView parentView = edge.output.node as BossBehaviorMakerNodeView;
                    BossBehaviorMakerNodeView childView = edge.input.node as BossBehaviorMakerNodeView;
                    if (parentView == null || childView == null)
                    {
                        continue;
                    }

                    _tree.AddChild(parentView.Node, childView.Node);
                }
            }

            return graphViewChange;
        }

        private void CreateNodeView(NodeBbm node)
        {
            BossBehaviorMakerNodeView nodeView = new BossBehaviorMakerNodeView(node);
            Rect rect = new Rect(node.NodeGraphPosition, node.NodeGraphSize);
            AddElement(nodeView);
            nodeView.SetPosition(rect);
        }

        private void CreateNode(System.Type type)
        {
            if (_hasTree == false)
            {
                return;
            }

            NodeBbm node = ScriptableObject.CreateInstance(type) as NodeBbm;
            node.name = type.Name;
            node.Guid = GUID.Generate().ToString();

            _tree.Nodes.Add(node);
            CreateNodeView(node);

            if (_tree.RootNode == null)
            {
                _tree.RootNode = node;
            }

            AssetDatabase.AddObjectToAsset(node, _tree);
            AssetDatabase.SaveAssets();
        }

        private void DeleteNode(NodeBbm node)
        {
            if (_hasTree == false)
            {
                return;
            }

            _tree.Nodes.Remove(node);

            if (_tree.RootNode == node)
            {
                _tree.RootNode = _tree.Nodes.FirstOrDefault();
            }

            AssetDatabase.RemoveObjectFromAsset(node);
            AssetDatabase.SaveAssets();
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent menuEvent)
        {
            if (_hasTree == false)
            {
                menuEvent.menu.AppendAction($"No tree selected", null);
                return;
            }

            TypeCache.TypeCollection types = TypeCache.GetTypesDerivedFrom<NodeBbm>();

            foreach (Type type in types)
            {
                if (type.IsAbstract)
                {
                    continue;
                }

                menuEvent.menu.AppendAction($"{type.BaseType.Name}/{type.Name}", _ => CreateNode(type));
            }

            base.BuildContextualMenu(menuEvent);
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            return ports.ToList()!.Where(
                endPort => endPort.direction != startPort.direction
                           && endPort.node != startPort.node
                           && endPort.portType == startPort.portType).ToList();
        }
    }
}