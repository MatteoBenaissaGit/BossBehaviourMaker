
using BossBehaviorMaker.Scripts.Runtime;
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
            base.title = Node.name;

            viewDataKey = Node.Guid;
            style.left = Node.NodeGraphPosition.x;
            style.top = Node.NodeGraphPosition.y;
            
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
    }
}