using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace BossBehaviourMaker.Scripts.Editor
{
    public class BossBehaviourMakerView : GraphView
    {
        public new class UxmlFactory : UxmlFactory<BossBehaviourMakerView, UxmlTraits>{}

        public BossBehaviourMakerView()
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
    }
}