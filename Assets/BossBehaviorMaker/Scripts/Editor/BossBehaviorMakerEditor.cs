using BossBehaviorMaker.Scripts.Runtime;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace BossBehaviorMaker.Scripts.Editor
{
    public class BossBehaviorMakerEditor : EditorWindow
    {
        [MenuItem("Tools/BossBehaviorMaker")]
        public static void OpenTreeEditor()
        {
            GetWindow<BossBehaviorMakerEditor>("Boss Behavior Maker");
        }

        public void CreateGUI()
        {
            VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                    "Assets/BossBehaviorMaker/UIBuilder/BossBehaviorMakerEditor.uxml");
            visualTree.CloneTree(rootVisualElement);
        }

        const string TextFieldName = "BehaviorTreeName";
        private void OnSelectionChange()
        {
            BehaviorTreeBbm tree = Selection.activeObject as BehaviorTreeBbm;
            if (tree != null)
            {
                SerializedObject serializedObject = new SerializedObject(tree);
                rootVisualElement.Bind(serializedObject);
            }
            else
            {
                rootVisualElement.Unbind();

                TextField textField = rootVisualElement.Q<TextField>(TextFieldName);
                if (textField != null)
                {
                    textField.value = string.Empty;
                }
            }
        }
    }
}