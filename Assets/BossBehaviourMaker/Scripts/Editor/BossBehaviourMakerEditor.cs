using UnityEditor;
using UnityEngine.UIElements;

namespace BossBehaviourMaker.Scripts.Editor
{
    public class BossBehaviourMakerEditor : EditorWindow
    {
        [MenuItem("Tools/BossBehaviourMaker")]
        public static void OpenTreeEditor()
        {
            GetWindow<BossBehaviourMakerEditor>("Boss Behaviour Maker");
        }

        public void CreateGUI()
        {
            VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                    "Assets/BossBehaviourMaker/UIBuilder/BossBehaviourMakerEditor.uxml");
            visualTree.CloneTree(rootVisualElement);
        }
    }
}