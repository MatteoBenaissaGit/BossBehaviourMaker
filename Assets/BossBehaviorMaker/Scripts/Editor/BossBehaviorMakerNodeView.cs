using BossBehaviorMaker.Scripts.Decorators;
using BossBehaviorMaker.Scripts.Runtime;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace BossBehaviorMaker.Scripts.Editor
{
    public class BossBehaviorMakerNodeView : UnityEditor.Experimental.GraphView.Node
    {
        public NodeBbm Node { get; private set; }

        public Port InputPort { get; set; }
        public Port OutputPort { get; set; }

        public BossBehaviorMakerNodeView(NodeBbm node)
        {
            Node = node;
            if (Node == null)
            {
                return;
            }
            base.title = Node.ToString();

            viewDataKey = Node.Guid;
            style.left = Node.NodeGraphPosition.x;
            style.top = Node.NodeGraphPosition.y;
            
            SetColor();
            
            CreateInputPorts();
            CreateOutputPorts();
        }

        public override void OnSelected()
        {
            base.OnSelected();
            
            BossBehaviorMakerEditor.Instance.GetGraphView().UpdateInspectorPanel();
            BossBehaviorMakerEditor.Instance.GetGraphView().CurrentSelectedNodeView = this;
        }

        public override void OnUnselected()
        {
            base.OnUnselected();
            
            BossBehaviorMakerEditor.Instance.GetGraphView().UpdateInspectorPanel();
            BossBehaviorMakerEditor.Instance.GetGraphView().CurrentSelectedNodeView = null;
            
            EditorUtility.SetDirty(Node);
            AssetDatabase.SaveAssets();
        }

        public override void SetPosition(Rect rect)
        {
            base.SetPosition(rect);
            
            Vector2 newPosition = new Vector2(rect.xMin, rect.yMin);
            Node.NodeGraphPosition = newPosition;
            
            Node.NodeGraphSize = rect.size;
        }

        private void CreateInputPorts()
        {
            InputPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(NodeBbm));
            if (InputPort == null)
            {
                return;
            }

            InputPort.portName = "Input";
            InputPort.portColor = Color.white;
            inputContainer.Add(InputPort);
        }

        private void CreateOutputPorts()
        {
            switch (Node)
            {
                case ActionNodeBbm :
                    break;
                case CompositeNodeBbm :
                    OutputPort = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Multi, typeof(NodeBbm));
                    break;
                case DecoratorNodeBbm :
                    OutputPort = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Single, typeof(NodeBbm));
                    break;
            }
            
            if (OutputPort == null)
            {
                return;
            }

            OutputPort.portName = "Output";
            OutputPort.portColor = Color.white;
            outputContainer.Add(OutputPort);
        }

        private void SetColor()
        {
            Color color = Node switch
            {
                ActionNodeBbm => new Color(1f, 0.94f, 0f, 1f),
                CompositeNodeBbm => new Color(0.32f, 1f, 0f, 1f),
                IfBossLife => new Color(0.16f, 0.27f, 1f, 1f),
                DecoratorNodeBbm => new Color(1f, 0.45f, 0f, 1f),
                _ => Color.white
            };

            style.borderTopColor = color;
            style.borderTopWidth = 6f;
            elementTypeColor = color;

            style.borderBottomLeftRadius = 10;
            style.borderBottomRightRadius = 10;
            style.borderTopLeftRadius = 10;
            style.borderTopRightRadius = 10;
        }

        private bool _isRoot;
        public void SetAsRoot(bool isRoot)
        {
            if (isRoot == _isRoot)
            {
                return;
            }

            _isRoot = isRoot;
            
            style.borderBottomWidth = isRoot ? 5 : 0;
            style.borderBottomColor = Color.white;
            style.borderLeftWidth = isRoot ? 5 : 0;
            style.borderLeftColor = Color.white;
            style.borderRightWidth = isRoot ? 5 : 0;
            style.borderRightColor = Color.white;
            
            title = isRoot ? title + " (ROOT)" : title.Substring(0, title.Length - 7);
        }
        
        public void UpdateTitle()
        {
            title = Node.ToString();
        }
    }
}