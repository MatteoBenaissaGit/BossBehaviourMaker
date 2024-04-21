using System.Collections.Generic;
using System.Linq;
using BossBehaviorMaker.Scripts.Runtime;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace BossBehaviorMaker.Scripts.Editor
{
    public class BossBehaviorMakerEditor : EditorWindow
    {
        public static BossBehaviorMakerEditor Instance { get; private set; }
        
        public BossBehaviorMakerGraphView TreeView { get; private set; }
        
        [MenuItem("Tools/BossBehaviorMaker")]
        public static void OpenTreeEditor()
        {
            GetWindow<BossBehaviorMakerEditor>("Boss Behavior Maker");
        }

        public void CreateGUI()
        {
            Instance = this;
            
            VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/BossBehaviorMaker/UIBuilder/BossBehaviorMakerEditor.uxml");
            visualTree.CloneTree(rootVisualElement);

            TreeView = rootVisualElement.Q<BossBehaviorMakerGraphView>();
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
                TreeView = rootVisualElement.Q<BossBehaviorMakerGraphView>(GraphViewName);
                if (TreeView != null)
                {
                    TreeView.PopulateView(tree);
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

        public BossBehaviorMakerGraphView GetGraphView()
        {
            return rootVisualElement.Q<BossBehaviorMakerGraphView>("BehaviorTreeGraphView");
        }
    }
}