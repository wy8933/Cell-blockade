using RewardTables;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace RewardTables.Editor
{
    [CustomPropertyDrawer(typeof(RewardTable<>), true)]
    public class RewardTableEditor : PropertyDrawer
    {
        private VisualElement _gauge;

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var root = new Foldout()
            {
                text = property.displayName
            };
            _gauge = new Foldout()
            {
                text = "Visualization",
                style =
                {
                    width = new Length(100, LengthUnit.Percent),
                    paddingLeft = 6,
                    paddingRight = 6,
                    flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Column)
                }
            };

            root.Add(_gauge);

            var entries = property.FindPropertyRelative("entries");
            var entriesCount = entries.arraySize;

            for (var i = 0; i < entriesCount; i++)
            {
                var entry = entries.GetArrayElementAtIndex(i);

                CreateGauge(entry, (float)i / entriesCount);
            }

            var entriesPropertyField = new PropertyField(entries);
            entriesPropertyField.TrackPropertyValue(entries, UpdateGauges);
            root.Add(entriesPropertyField);

            return root;
        }

        private void UpdateGauges(SerializedProperty serializedProperty)
        {
            var entriesCount = serializedProperty.arraySize;
            _gauge.Clear();
            for (var i = 0; i < entriesCount; i++)
            {
                var entry = serializedProperty.GetArrayElementAtIndex(i);

                CreateGauge(entry, (float)i / entriesCount);
            }
        }

        private void CreateGauge(SerializedProperty entry, float hue)
        {
            var chance = entry.FindPropertyRelative("chance");
            var chanceFloat = chance.floatValue;

            var pair = new VisualElement()
            {
                style =
                {
                    flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row)
                }
            };
            var minCount = entry.FindPropertyRelative("minCount").intValue;
            var maxCount = entry.FindPropertyRelative("maxCount").intValue;
            var countString = minCount != maxCount ? $"{Mathf.Min(minCount, maxCount)} - {Mathf.Max(minCount, maxCount)}" : $"{minCount}";
            pair.Add(new Label($"{entry.displayName} {chanceFloat:P} -> {countString}")
            {
                style =
                {
                    backgroundColor = Color.HSVToRGB(hue, 1, 0.5f),
                    width = new StyleLength(new Length(chanceFloat * 100f, LengthUnit.Percent))
                }
            });
            _gauge.Add(pair);
        }
    }
}