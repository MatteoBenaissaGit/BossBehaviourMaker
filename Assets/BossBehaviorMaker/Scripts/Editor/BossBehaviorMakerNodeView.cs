
using BossBehaviorMaker.Scripts.Runtime;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

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
            base.title = Node.name;

            viewDataKey = Node.Guid;
            style.left = Node.NodeGraphPosition.x;
            style.top = Node.NodeGraphPosition.y;
            
            SetColor();
            
            CreateInputPorts();
            CreateOutputPorts();
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
            InputPort = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Multi, typeof(NodeBbm));
            if (InputPort == null)
            {
                return;
            }

            InputPort.portName = "";
            InputPort.portColor = new Color(0.27f, 0.58f, 0.26f);
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

            OutputPort.portName = "";
            OutputPort.portColor = new Color(0.63f, 0.22f, 0.23f);
            outputContainer.Add(OutputPort);
        }

        private void SetColor()
        {
            Color color = Color.white;
            switch (Node)
            {
                case ActionNodeBbm :
                    color = new Color(1f, 0.94f, 0f, 0.25f);
                    style.backgroundColor = color;
                    elementTypeColor = new Color(color.r,color.g,color.b,1);
                    break;
                case CompositeNodeBbm :
                    color = new Color(0.32f, 1f, 0f, 0.25f);
                    style.backgroundColor = color;
                    elementTypeColor = new Color(color.r,color.g,color.b,1);
                    break;
                case DecoratorNodeBbm :
                    color = new Color(1f, 0.45f, 0f, 0.25f);
                    style.backgroundColor = color;
                    elementTypeColor = new Color(color.r,color.g,color.b,0.5f);
                    break;
            }

            style.borderBottomLeftRadius = 10;
            style.borderBottomRightRadius = 10;
            style.borderTopLeftRadius = 10;
            style.borderTopRightRadius = 10;
        }
    }
}