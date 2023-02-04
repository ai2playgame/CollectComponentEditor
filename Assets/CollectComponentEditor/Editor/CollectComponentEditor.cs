using System;
using System.Linq;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;

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

        private ListView _listView;

        private void CollectObjects()
        {
            var root = rootVisualElement;

            if (_listView != null)
            {
                root.Remove(_listView);
                _listView = null;
            }

            var script = ((_objectField.value as MonoScript));
            if (script == null)
            {
                _statLabel.text = $"(not found)";
                return;
            }

            var type = script.GetClass();
            
            _listView = new ListView();

            var targets = FindObjectsOfType(type).ToList();
            targets.Sort((lhs, rhs) => String.Compare(lhs.name, rhs.name, StringComparison.Ordinal));

            _statLabel.text = $"total: {targets.Count}";

            _listView.itemsSource = targets;
            _listView.makeItem = () =>
            {
                var box = new VisualElement();
                box.Add(new Label());
                box.Add(new InspectorElement());
                return box;
            };
            _listView.bindItem = (VisualElement element, int index) =>
            {
                (element.ElementAt(0) as Label).text = targets[index].name;
                (element.ElementAt(1) as InspectorElement).Bind(new SerializedObject(targets[index]));
            };

            _listView.fixedItemHeight = 100;
            root.Add(_listView);
        }
    }
}