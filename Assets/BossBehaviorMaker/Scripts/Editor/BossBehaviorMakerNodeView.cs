
using BossBehaviorMaker.Scripts.Runtime;

namespace BossBehaviorMaker.Scripts.Editor
{
    public class BossBehaviorMakerNodeView : UnityEditor.Experimental.GraphView.Node
    {
        private NodeBbm _node;

        public BossBehaviorMakerNodeView(NodeBbm node)
        {
            _node = node;
            if (_node == null)
            {
                return;
            }
            base.title = _node.name;
        }
    }
}