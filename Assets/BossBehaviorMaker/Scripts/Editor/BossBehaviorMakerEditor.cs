using BossBehaviorMaker.Scripts.Runtime;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace BossBehaviorMaker.Scripts.Editor
{
    public class BossBehaviorMakerEditor : EditorWindow
    {
        private BossBehaviorMakerGraphView _treeView;
        
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

            _treeView = rootVisualElement.Q<BossBehaviorMakerGraphView>();
            rootVisualElement.Q<VisualElement>("InspectorView");
            
            OnSelectionChange();
        }

        const string TextFieldName = "BehaviorTreeName";
        const string GraphViewName = "BehaviorTreeGraphView";
        private void OnSelectionChange()
        {
            BehaviorTreeBbm tree = Selection.activeObject as BehaviorTreeBbm;
            
            if (tree != null)
            {
                //bind the properties of the tree to the elements in the tree view
                SerializedObject serializedObject = new SerializedObject(tree);
                rootVisualElement.Bind(serializedObject);
                
                //populate the tree view
                _treeView = rootVisualElement.Q<BossBehaviorMakerGraphView>(GraphViewName);
                if (_treeView != null)
                {
                    _treeView.PopulateView(tree);
                }
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