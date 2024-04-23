using System;
using System.Collections.Generic;
using System.Linq;
using BossBehaviorMaker.Scripts.Decorators;
using BossBehaviorMaker.Scripts.Runtime;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace BossBehaviorMaker.Scripts.Editor
{
    public class BossBehaviorMakerGraphView : GraphView
    {
        public BossBehaviorMakerNodeView CurrentSelectedNodeView
        {
            get => _currentSelectedNodeView;
            set
            {
                _currentSelectedNodeView = value;
                if (_currentSelectedNodeView == null)
                {
                    _rootNodeButton.style.backgroundColor = new StyleColor(new Color32(24, 24, 24, 255));
                    return;
                }
                _rootNodeButton.style.backgroundColor = new StyleColor(new Color32(50, 50, 50, 255));
            }
        }
        public BossBehaviorMakerNodeView RootNodeView { get; set;}
        
        private BehaviorTreeBbm _tree;
        private bool _hasTree => _tree != null;
        private BossBehaviorMakerNodeView _currentSelectedNodeView;
        private ToolbarButton _rootNodeButton;
        
        public new class UxmlFactory : UxmlFactory<BossBehaviorMakerGraphView, UxmlTraits>
        {
        }

        public BossBehaviorMakerGraphView()
        {
            style.flexGrow = 1f;
            CreateGridBackground();
            AddManipulators();
            CreateMiniMap();
            SetToolbarButtons();
        }

        private void CreateGridBackground()
        {
            GridBackground gridBackground = new GridBackground()
            {
                style =
                {
                    position = Position.Absolute,
                    width = 10000,
                    height = 10000,
                },
            };
            Insert(0, gridBackground);

            StyleSheet styleSheet = (StyleSheet)EditorGUIUtility.Load("Assets/BossBehaviorMaker/UIBuilder/BbmGraphViewStyles.uss");
            styleSheets.Add(styleSheet);
        }

        private void CreateMiniMap()
        {
            MiniMap miniMap = new MiniMap { anchored = true, 
                capabilities = Capabilities.Movable | Capabilities.Selectable | Capabilities.Resizable};
            
            miniMap.SetPosition(new Rect(20, 50, 100, 100));
            miniMap.maxHeight = 100;
            miniMap.maxWidth = 100;

            StyleColor backgroundColor = new StyleColor(new Color32(29, 29, 30, 255));
            StyleColor borderColor = new StyleColor(new Color32(50, 50, 50, 255));
            miniMap.style.backgroundColor = backgroundColor;
            miniMap.style.borderTopColor = borderColor;
            miniMap.style.borderRightColor = borderColor;
            miniMap.style.borderBottomColor = borderColor;
            miniMap.style.borderTopColor = borderColor;
            
            Add(miniMap);
        }

        private void SetToolbarButtons()
        {
            Toolbar toolbar = new Toolbar
            {
                style =
                {
                    height = 25
                }
            };
            Add(toolbar);
            
            ToolbarButton minimapButton = new ToolbarButton
            {
                text = "Toggle MiniMap",
                clickable = new Clickable(ToggleMiniMap),
                style =
                {
                    borderBottomRightRadius = 5,
                    borderBottomLeftRadius = 5,
                    borderTopRightRadius = 5,
                    borderTopLeftRadius = 5,
                    marginLeft = 10,
                    height = 20,
                    backgroundColor = new StyleColor(new Color32(50, 50, 50, 255)),
                    alignSelf = new StyleEnum<Align>(Align.Center),
                    unityTextAlign = new StyleEnum<TextAnchor>(TextAnchor.MiddleCenter)
                }
            };
            
            _rootNodeButton = new ToolbarButton
            {
                text = "Set Root Node",
                clickable = new Clickable(SetRootNode),
                style =
                {
                    borderBottomRightRadius = 5,
                    borderBottomLeftRadius = 5,
                    borderTopRightRadius = 5,
                    borderTopLeftRadius = 5,
                    marginLeft = 10,
                    height = 20,
                    backgroundColor = new StyleColor(new Color32(50, 50, 50, 255)),
                    alignSelf = new StyleEnum<Align>(Align.Center),
                    unityTextAlign = new StyleEnum<TextAnchor>(TextAnchor.MiddleCenter)
                }
            };
            CurrentSelectedNodeView = null;

            toolbar.Add(minimapButton);
            toolbar.Add(_rootNodeButton);
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
            UpdateInspectorPanel();
            
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
        
        public void UpdateInspectorPanel()
        {
            SplitViewBbm inspectorPanel = BossBehaviorMakerEditor.Instance.rootVisualElement.Q<SplitViewBbm>("SplitViewBbm");
            IEnumerable<GraphElement> selectedElements = selection.OfType<GraphElement>();
            inspectorPanel.OnSelectionChanged(selectedElements);
        }

        private void CreateNodeView(NodeBbm node)
        {
            BossBehaviorMakerNodeView nodeView = new BossBehaviorMakerNodeView(node);
            Rect rect = new Rect(node.NodeGraphPosition, node.NodeGraphSize);
            AddElement(nodeView);
            nodeView.SetPosition(rect);
            if (nodeView.Node == _tree.RootNode)
            {
                nodeView.SetAsRoot(true);
                RootNodeView = nodeView;
            }
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
        
        private void ToggleMiniMap()
        {
            MiniMap miniMap = this.Q<MiniMap>();
            miniMap.visible = miniMap.visible == false;
        }
        
        private void SetRootNode()
        {
            if (CurrentSelectedNodeView == null)
            {
                return;
            }

            RootNodeView?.SetAsRoot(false);
            RootNodeView = CurrentSelectedNodeView;
            RootNodeView.SetAsRoot(true);
            
            _tree.RootNode = RootNodeView.Node;

            EditorUtility.SetDirty(_tree);
            EditorUtility.SetDirty(_tree.RootNode);
        }
    }
}