using System;
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
        public new class UxmlFactory : UxmlFactory<BossBehaviorMakerGraphView, UxmlTraits>{}

        private BehaviorTreeBbm _tree;
        private bool _hasTree => _tree != null;
        
        public BossBehaviorMakerGraphView()
        {
            style.flexGrow = 1f;
            Insert(0, new GridBackground());
            AddManipulators();
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
            _tree.Nodes.ForEach(CreateNodeView);

            foreach (NodeBbm node in _tree.Nodes)
            {
                BossBehaviorMakerNodeView parentView = GetNodeByGuid(node.Guid) as BossBehaviorMakerNodeView;
                foreach (NodeBbm child in _tree.GetChildren(node))
                {
                    BossBehaviorMakerNodeView childView = GetNodeByGuid(child.Guid) as BossBehaviorMakerNodeView;
                    Edge edge = parentView?.OutputPort.ConnectTo(childView?.InputPort);
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
            AddElement(nodeView);
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