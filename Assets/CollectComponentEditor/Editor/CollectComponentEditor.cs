using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;

namespace CollectComponentEditor.Editor
{
    public class CollectComponentEditor : EditorWindow
    {
        [MenuItem("Utilities/CollectComponentWindow")]
        public static void ShowExample()
        {
            var wnd = GetWindow<CollectComponentEditor>("Collect Component Window");
        }

        private void OnInspectorUpdate()
        {
            Repaint();
        }

        private ObjectField _objectField;
        private Label _statLabel;

        private List<VisualElement> _componentViewList = new();

        private void CreateGUI()
        {
            var root = rootVisualElement;

            var toolbar = new Toolbar();
            root.Add(toolbar);

            var updatedButton = new ToolbarButton() { text = "Update" };
            toolbar.Add(updatedButton);

            _objectField = new ObjectField("Script") { objectType = typeof(MonoScript) };
            root.Add(_objectField);

            _statLabel = new Label();
            root.Add(_statLabel);

            // ボタンを押したらCollectObjectsが呼ばれるように
            updatedButton.clicked += CollectObjects;

            // 生成時に一回更新
            CollectObjects();
        }

        private void CollectObjects()
        {
            var root = rootVisualElement;

            foreach (var view in _componentViewList)
            {
                root.Remove(view);
            }
            _componentViewList.Clear();

            var script = ((_objectField.value as MonoScript));
            if (script == null)
            {
                _statLabel.text = $"(not found)";
                return;
            }

            var type = script.GetClass();
            

            var targets = FindObjectsOfType(type).ToList();
            targets.Sort((lhs, rhs) => String.Compare(lhs.name, rhs.name, StringComparison.Ordinal));

            _statLabel.text = $"total: {targets.Count}";

            foreach (var targetComponent in targets)
            {
                var box = new VisualElement();
                var label = new Label() { text = targetComponent.name };
                label.style.marginTop = 20;
                box.Add(label);
                box.Add(new InspectorElement(new SerializedObject(targetComponent)));
                
                root.Add(box);
                _componentViewList.Add(root);
            }
        }
    }
}