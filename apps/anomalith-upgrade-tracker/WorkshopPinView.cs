using System;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AnomalithUpgradeTracker
{
    internal sealed class WorkshopPinView
    {
        private const BindingFlags Fields = BindingFlags.Instance | BindingFlags.NonPublic;
        private static readonly FieldInfo Materials = typeof(DevMenuBase).GetField("m_RequiredMaterials", Fields);
        private static readonly FieldInfo Info = typeof(DevMenuBase).GetField("m_Info", Fields);
        private static readonly FieldInfo MaterialRows = typeof(RequiredMaterials).GetField("m_NeedMatTextInsList", Fields);
        private static readonly FieldInfo LeftMaterialColumn = typeof(RequiredMaterials).GetField("m_NeedMatTextsRootL", Fields);
        private static readonly FieldInfo ButtonBackground = typeof(DevelopmetInfo).GetField("m_DevButtonBG", Fields);
        private static readonly FieldInfo ButtonText = typeof(DevelopmetInfo).GetField("m_DevButtonText", Fields);

        private readonly DevMenuBase menu;
        private readonly RequiredMaterials materials;
        private readonly RectTransform materialsPanel;
        private readonly RectTransform rect;
        private readonly Button button;
        private readonly TextMeshProUGUI action;
        private readonly TextMeshProUGUI shortcut;
        private readonly Vector3[] corners = new Vector3[4];

        private WorkshopPinView(DevMenuBase menu, RequiredMaterials materials, RectTransform materialsPanel, Image background,
            TextMeshProUGUI font, Action onToggle)
        {
            this.menu = menu;
            this.materials = materials;
            this.materialsPanel = materialsPanel;
            rect = new GameObject("Workshop pin toggle", typeof(RectTransform), typeof(Image), typeof(Button))
                .GetComponent<RectTransform>();
            rect.SetParent(menu.transform, false);
            rect.anchorMin = rect.anchorMax = ((RectTransform)menu.transform).pivot;
            rect.pivot = new Vector2(1, 0);

            var target = rect.GetComponent<Image>();
            if (background != null)
            {
                target.sprite = background.sprite;
                target.type = background.type == Image.Type.Sliced ? Image.Type.Sliced : Image.Type.Simple;
            }

            button = rect.GetComponent<Button>();
            button.targetGraphic = target;
            button.navigation = new Navigation { mode = Navigation.Mode.None };
            button.onClick.AddListener(() => onToggle());
            action = CreateText("Pin action", font, 21, TextAlignmentOptions.MidlineLeft);
            action.color = new Color(0.92f, 0.98f, 0.94f);
            shortcut = CreateText("Pin shortcut", font, 17, TextAlignmentOptions.MidlineRight);
            shortcut.color = new Color(0.78f, 0.88f, 0.84f);
            rect.gameObject.SetActive(false);
        }

        public static WorkshopPinView TryCreate(DevMenuBase menu, Action onToggle)
        {
            var materials = Materials?.GetValue(menu) as RequiredMaterials;
            var info = Info?.GetValue(menu) as DevelopmetInfo;
            if (materials == null || info == null || !(menu.transform is RectTransform))
                return null;
            var column = LeftMaterialColumn?.GetValue(materials) as RectTransform;
            if (column == null || !(column.parent is RectTransform materialsPanel))
                return null;
            var background = ButtonBackground?.GetValue(info) as Image;
            var font = ButtonText?.GetValue(info) as TextMeshProUGUI;
            return font == null || font.font == null ? null :
                new WorkshopPinView(menu, materials, materialsPanel, background, font, onToggle);
        }

        public bool IsFor(DevMenuBase candidate) => rect != null && menu == candidate;

        public void Show(bool pinned, string key)
        {
            if (rect == null || materials == null || materialsPanel == null || !materials.gameObject.activeInHierarchy ||
                !(MaterialRows?.GetValue(materials) is List<GameObject> rows))
            {
                Hide();
                return;
            }

            var parent = (RectTransform)rect.parent;
            float top = float.NegativeInfinity;
            foreach (var row in rows)
            {
                if (row == null || !row.activeInHierarchy || !(row.transform is RectTransform rowRect))
                    continue;
                rowRect.GetWorldCorners(corners);
                top = Mathf.Max(top, parent.InverseTransformPoint(corners[2]).y);
            }
            if (float.IsNegativeInfinity(top))
            {
                Hide();
                return;
            }

            string label = pinned ? "Unpin recipe" : "Pin recipe";
            key = key ?? "";
            if (action.text != label || shortcut.text != key)
            {
                action.text = label;
                shortcut.text = key;
                float keyWidth = Mathf.Min(100, Mathf.Ceil(shortcut.GetPreferredValues().x));
                float actionWidth = Mathf.Ceil(action.GetPreferredValues().x);
                rect.sizeDelta = new Vector2(Mathf.Max(158, actionWidth + keyWidth + 42), 34);
                action.rectTransform.offsetMax = new Vector2(-keyWidth - 26, 0);
                shortcut.rectTransform.offsetMin = new Vector2(rect.sizeDelta.x - keyWidth - 13, 0);
                button.colors = new ColorBlock
                {
                    normalColor = pinned ? new Color(0.19f, 0.27f, 0.26f) : new Color(0.11f, 0.18f, 0.19f),
                    highlightedColor = new Color(0.23f, 0.33f, 0.31f),
                    pressedColor = new Color(0.11f, 0.18f, 0.18f),
                    selectedColor = new Color(0.20f, 0.30f, 0.28f),
                    disabledColor = new Color(0.14f, 0.20f, 0.20f, 0.5f),
                    colorMultiplier = 1,
                    fadeDuration = 0.1f
                };
            }
            float right = parent.InverseTransformPoint(materialsPanel.TransformPoint(materialsPanel.rect.max)).x;
            rect.anchoredPosition = new Vector2(right, top + 6);
            if (rect.GetSiblingIndex() != parent.childCount - 1)
                rect.SetAsLastSibling();
            rect.gameObject.SetActive(true);
        }

        public void Hide()
        {
            if (rect != null)
                rect.gameObject.SetActive(false);
        }

        public void Destroy()
        {
            if (rect != null)
            {
                Hide();
                UnityEngine.Object.Destroy(rect.gameObject);
            }
        }

        private TextMeshProUGUI CreateText(string name, TextMeshProUGUI font, float size, TextAlignmentOptions alignment)
        {
            var text = new GameObject(name, typeof(RectTransform), typeof(TextMeshProUGUI))
                .GetComponent<TextMeshProUGUI>();
            text.rectTransform.SetParent(rect, false);
            text.rectTransform.anchorMin = Vector2.zero;
            text.rectTransform.anchorMax = Vector2.one;
            text.rectTransform.offsetMin = new Vector2(12, 0);
            text.rectTransform.offsetMax = new Vector2(-12, 0);
            text.font = font.font;
            text.fontSharedMaterial = font.fontSharedMaterial;
            text.fontSize = size;
            text.alignment = alignment;
            text.color = font.color;
            text.textWrappingMode = TextWrappingModes.NoWrap;
            text.overflowMode = TextOverflowModes.Ellipsis;
            text.raycastTarget = false;
            text.richText = false;
            return text;
        }
    }
}
