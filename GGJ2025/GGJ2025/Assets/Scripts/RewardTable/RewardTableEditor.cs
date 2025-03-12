#if UNITY_EDITOR
using RewardTables;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Linq;

namespace RewardTables.Editor
{
    [CustomPropertyDrawer(typeof(RewardTable<>), true)]
    public class RewardTableEditor : PropertyDrawer
    {
        private VisualElement _entriesGauge;
        private VisualElement _rarityChancesGauge;

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var root = new Foldout() { text = property.displayName };

            _entriesGauge = new Foldout()
            {
                text = "Rarity Distribution (Entries)",
                style =
                {
                    width = new Length(100, LengthUnit.Percent),
                    paddingLeft = 6,
                    paddingRight = 6,
                    flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Column)
                }
            };
            root.Add(_entriesGauge);

            _rarityChancesGauge = new Foldout()
            {
                text = "Overall Rarity Chances",
                style =
                {
                    width = new Length(100, LengthUnit.Percent),
                    paddingLeft = 6,
                    paddingRight = 6,
                    flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Column)
                }
            };
            root.Add(_rarityChancesGauge);

            var entries = property.FindPropertyRelative("entries");
            UpdateEntriesGraph(entries);
            UpdateRarityChancesGraph(property);

            var entriesPropertyField = new PropertyField(entries);
            entriesPropertyField.TrackPropertyValue(entries, sp => UpdateEntriesGraph(sp));
            root.Add(entriesPropertyField);

            var overallChanceField = new VisualElement();
            overallChanceField.style.display = DisplayStyle.None;
            overallChanceField.Add(new PropertyField(property.serializedObject.FindProperty("commonChance")));
            overallChanceField.Add(new PropertyField(property.serializedObject.FindProperty("uncommonChance")));
            overallChanceField.Add(new PropertyField(property.serializedObject.FindProperty("rareChance")));
            overallChanceField.Add(new PropertyField(property.serializedObject.FindProperty("epicChance")));
            overallChanceField.Add(new PropertyField(property.serializedObject.FindProperty("legendaryChance")));
            overallChanceField.TrackPropertyValue(property.serializedObject.FindProperty("commonChance"), sp => UpdateRarityChancesGraph(property));
            root.Add(overallChanceField);

            return root;
        }

        private void UpdateEntriesGraph(SerializedProperty entries)
        {
            _entriesGauge.Clear();
            int totalCount = entries.arraySize;
            Dictionary<int, int> rarityCounts = new Dictionary<int, int>();

            for (int i = 0; i < totalCount; i++)
            {
                var entry = entries.GetArrayElementAtIndex(i);
                var rarityProp = entry.FindPropertyRelative("rarity");
                int rarityIndex = rarityProp.enumValueIndex;
                if (!rarityCounts.ContainsKey(rarityIndex))
                    rarityCounts[rarityIndex] = 0;
                rarityCounts[rarityIndex]++;
            }

            if (totalCount == 0)
                return;

            string[] rarityNames = entries.GetArrayElementAtIndex(0)
                                           .FindPropertyRelative("rarity")
                                           .enumDisplayNames;

            for (int i = 0; i < rarityNames.Length; i++)
            {
                int count = rarityCounts.ContainsKey(i) ? rarityCounts[i] : 0;
                float percentage = totalCount > 0 ? (float)count / totalCount : 0f;
                var labelText = $"{rarityNames[i]}: {count} entries ({percentage:P0})";

                var gaugeBar = new VisualElement()
                {
                    style =
                    {
                        flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row),
                        marginBottom = 2
                    }
                };

                var background = new VisualElement()
                {
                    style =
                    {
                        backgroundColor = Color.gray,
                        width = new StyleLength(new Length(100, LengthUnit.Percent)),
                        height = new StyleLength(new Length(18))
                    }
                };

                var foreground = new VisualElement()
                {
                    style =
                    {
                        backgroundColor = Color.HSVToRGB(i / (float)rarityNames.Length, 1, 0.8f),
                        width = new StyleLength(new Length(percentage * 100, LengthUnit.Percent)),
                        height = new StyleLength(new Length(18))
                    }
                };

                background.Add(foreground);

                var label = new Label(labelText)
                {
                    style =
                    {
                        marginLeft = 4,
                        unityTextAlign = TextAnchor.MiddleLeft,
                        flexGrow = 1
                    }
                };

                gaugeBar.Add(label);
                gaugeBar.Add(background);
                _entriesGauge.Add(gaugeBar);
            }
        }

        private void UpdateRarityChancesGraph(SerializedProperty property)
        {
            _rarityChancesGauge.Clear();

            SerializedProperty commonProp = property.serializedObject.FindProperty("commonChance");
            SerializedProperty uncommonProp = property.serializedObject.FindProperty("uncommonChance");
            SerializedProperty rareProp = property.serializedObject.FindProperty("rareChance");
            SerializedProperty epicProp = property.serializedObject.FindProperty("epicChance");
            SerializedProperty legendaryProp = property.serializedObject.FindProperty("legendaryChance");

            float common = commonProp != null ? commonProp.floatValue : 0f;
            float uncommon = uncommonProp != null ? uncommonProp.floatValue : 0f;
            float rare = rareProp != null ? rareProp.floatValue : 0f;
            float epic = epicProp != null ? epicProp.floatValue : 0f;
            float legendary = legendaryProp != null ? legendaryProp.floatValue : 0f;

            float total = common + uncommon + rare + epic + legendary;

            var rarityNames = new string[] { "Common", "Uncommon", "Rare", "Epic", "Legendary" };
            var chanceValues = new float[] { common, uncommon, rare, epic, legendary };

            for (int i = 0; i < rarityNames.Length; i++)
            {
                float chance = chanceValues[i];
                float percentage = total > 0 ? chance / total : 0f;
                var labelText = $"{rarityNames[i]} Chance: {chance:P1} ({percentage:P0} of total)";

                var gaugeBar = new VisualElement()
                {
                    style =
                    {
                        flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row),
                        marginBottom = 2
                    }
                };

                var background = new VisualElement()
                {
                    style =
                    {
                        backgroundColor = Color.gray,
                        width = new StyleLength(new Length(100, LengthUnit.Percent)),
                        height = new StyleLength(new Length(18))
                    }
                };

                var foreground = new VisualElement()
                {
                    style =
                    {
                        backgroundColor = Color.HSVToRGB(i / (float)rarityNames.Length, 1, 0.8f),
                        width = new StyleLength(new Length(percentage * 100, LengthUnit.Percent)),
                        height = new StyleLength(new Length(18))
                    }
                };

                background.Add(foreground);

                var label = new Label(labelText)
                {
                    style =
                    {
                        marginLeft = 4,
                        unityTextAlign = TextAnchor.MiddleLeft,
                        flexGrow = 1
                    }
                };

                gaugeBar.Add(label);
                gaugeBar.Add(background);
                _rarityChancesGauge.Add(gaugeBar);
            }

            if (Mathf.Abs(total - 1f) > 0.0001f)
            {
                var warning = new Label("Warning: Overall rarity chances is currently "+ total +"%!")
                {
                    style =
                    {
                        color = Color.red,
                        unityFontStyleAndWeight = FontStyle.Bold,
                        marginTop = 4
                    }
                };
                _rarityChancesGauge.Add(warning);
            }
        }
    }
}
#endif