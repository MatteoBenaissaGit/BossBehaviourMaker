using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace BossBehaviorMaker.Scripts.Editor
{
    public class BossBehaviorMakerGraphView : GraphView
    {
        public new class UxmlFactory : UxmlFactory<BossBehaviorMakerGraphView, UxmlTraits>{}

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
    }
}