using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace BossBehaviorMaker.Scripts.Editor
{
    public class SplitViewBbm : TwoPaneSplitView
    {
        private VisualElement _inspectorPanel;
        
        public new class UxmlFactory : UxmlFactory<SplitViewBbm, UxmlTraits>
        {
            
        }

        public SplitViewBbm()
        {
            _inspectorPanel = new VisualElement(){name = "inspector-panel"};
            Add(_inspectorPanel);
            
            OnSelectionChanged(new List<GraphElement>());
        }
        
        public void OnSelectionChanged(IEnumerable<GraphElement> selection)
        {
            _inspectorPanel.Clear();
            
            Label title = new Label("Node Properties");
            title.style.unityFontStyleAndWeight = FontStyle.Bold;
            title.style.fontSize = 15;
            
            _inspectorPanel.Add(title);
            
            Label description = new Label($"Select a node to view its properties.");
            _inspectorPanel.Add(description);
            
            BossBehaviorMakerNodeView selectedNode = selection.OfType<BossBehaviorMakerNodeView>().FirstOrDefault();

            if (selectedNode != null)
            {
                Label nodeDescription = new Label($"\n{selectedNode.Node.NodeDescription()}\n");
                _inspectorPanel.Add(nodeDescription);
                
                PropertyInfo[] properties = selectedNode.Node.GetType().GetProperties();
                for (int i = 0; i < properties.Length; i++)
                {
                    PropertyInfo property = properties[i];

                    switch (Type.GetTypeCode(property.PropertyType))
                    {
                        case TypeCode.Int32:
                            IntegerField intField = new IntegerField(property.Name);
                            intField.value = (int)property.GetValue(selectedNode.Node);
                            intField.RegisterValueChangedCallback(evt => property.SetValueOptimized(selectedNode.Node, evt.newValue));
                            intField.RegisterValueChangedCallback(evt => EditorUtility.SetDirty(selectedNode.Node));
                            _inspectorPanel.Add(intField);
                            break;

                        case TypeCode.Double:
                            DoubleField doubleField = new DoubleField(property.Name);
                            doubleField.value = (double)property.GetValue(selectedNode.Node);
                            doubleField.RegisterValueChangedCallback(evt => property.SetValueOptimized(selectedNode.Node, evt.newValue));
                            doubleField.RegisterValueChangedCallback(evt => EditorUtility.SetDirty(selectedNode.Node));
                            _inspectorPanel.Add(doubleField);
                            break;

                        case TypeCode.String:
                            if (property.Name == "Guid")
                            {
                                continue;
                            }
                            TextField stringField = new TextField(property.Name);
                            stringField.value = (string)property.GetValue(selectedNode.Node);
                            stringField.RegisterValueChangedCallback(evt => property.SetValueOptimized(selectedNode.Node, evt.newValue));
                            stringField.RegisterValueChangedCallback(evt => EditorUtility.SetDirty(selectedNode.Node));
                            _inspectorPanel.Add(stringField);
                            break;

                        case TypeCode.Boolean:
                            Toggle boolField = new Toggle(property.Name);
                            boolField.value = (bool)property.GetValue(selectedNode.Node);
                            boolField.RegisterValueChangedCallback(evt => property.SetValueOptimized(selectedNode.Node, evt.newValue));
                            boolField.RegisterValueChangedCallback(evt => EditorUtility.SetDirty(selectedNode.Node));
                            _inspectorPanel.Add(boolField);
                            break;

                        // Add more cases for other types as needed

                        default:
                            // Handle unknown types
                            break;
                    }
                }
            }
        }
    }
}