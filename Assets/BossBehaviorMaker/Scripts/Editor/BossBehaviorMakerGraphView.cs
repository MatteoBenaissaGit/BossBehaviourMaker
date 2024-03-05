using System;
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

            _tree.Nodes.ForEach(CreateNodeView);
        }

        private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
            if (graphViewChange.elementsToRemove == null)
            {
                goto returnChanges;
            }

            foreach (GraphElement element in graphViewChange.elementsToRemove)
            {
                BossBehaviorMakerNodeView nodeView = element as BossBehaviorMakerNodeView;

                if (nodeView == null)
                {
                    continue;
                }
                DeleteNode(nodeView.Node);
            }
            
            returnChanges:
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
    }
}