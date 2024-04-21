using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.VisualScripting;
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
        }
        
        public void OnSelectionChanged(IEnumerable<GraphElement> selection)
        {
            _inspectorPanel.Clear();
            
            Label title = new Label("-- Node Properties --");
            title.style.unityFontStyleAndWeight = FontStyle.Bold;
            title.style.fontSize = 20;
            _inspectorPanel.Add(title);
            
            BossBehaviorMakerNodeView selectedNode = selection.OfType<BossBehaviorMakerNodeView>().FirstOrDefault();

            if (selectedNode != null)
            {
                FieldInfo[] fields = selectedNode.Node.GetType().GetFields();
                for (int i = 0; i < fields.Length; i++)
                {
                    FieldInfo field = fields[i];
                    
                    switch (Type.GetTypeCode(field.FieldType))
                    {
                        case TypeCode.Int32:
                            IntegerField intField = new IntegerField(field.Name);
                            intField.value = (int)field.GetValue(selectedNode.Node);
                            intField.RegisterValueChangedCallback(evt => field.SetValue(selectedNode.Node, evt.newValue));
                            _inspectorPanel.Add(intField);
                            break;
                        
                        case TypeCode.Double:
                            DoubleField doubleField = new DoubleField(field.Name);
                            doubleField.value = (double)field.GetValue(selectedNode.Node);
                            doubleField.RegisterValueChangedCallback(evt => field.SetValue(selectedNode.Node, evt.newValue));
                            _inspectorPanel.Add(doubleField);
                            break;

                        case TypeCode.String:
                            if (field.Name == "Guid")
                            {
                                continue;
                            }
                            TextField stringField = new TextField(field.Name);
                            stringField.value = (string)field.GetValue(selectedNode.Node);
                            stringField.RegisterValueChangedCallback(evt => field.SetValue(selectedNode.Node, evt.newValue));
                            _inspectorPanel.Add(stringField);
                            break;

                        case TypeCode.Boolean:
                            Toggle boolField = new Toggle(field.Name);
                            boolField.value = (bool)field.GetValue(selectedNode.Node);
                            boolField.RegisterValueChangedCallback(evt => field.SetValue(selectedNode.Node, evt.newValue));
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