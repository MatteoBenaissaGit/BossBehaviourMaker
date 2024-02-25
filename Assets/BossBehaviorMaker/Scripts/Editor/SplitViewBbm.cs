using UnityEngine.UIElements;

namespace BossBehaviorMaker.Scripts.Editor
{
    public class SplitViewBbm : TwoPaneSplitView
    {
        public new class UxmlFactory : UxmlFactory<SplitViewBbm, UxmlTraits>{}
    }
}