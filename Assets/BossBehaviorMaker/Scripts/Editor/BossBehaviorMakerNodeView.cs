
using BossBehaviorMaker.Scripts.Runtime;
using UnityEngine;

namespace BossBehaviorMaker.Scripts.Editor
{
    public class BossBehaviorMakerNodeView : UnityEditor.Experimental.GraphView.Node
    {
        public NodeBbm Node { get; private set; }

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
        }

        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);
            Node.NodeGraphPosition.x = newPos.xMin;
            Node.NodeGraphPosition.y = newPos.yMin;
        }
    }
}