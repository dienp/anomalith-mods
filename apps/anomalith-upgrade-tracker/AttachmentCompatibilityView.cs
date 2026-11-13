using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace AnomalithUpgradeTracker
{
    internal sealed class AttachmentCompatibilityView
    {
        private const BindingFlags Fields = BindingFlags.Instance | BindingFlags.NonPublic;
        private static readonly FieldInfo Info = typeof(DevMenuBase).GetField("m_Info", Fields);
        private static readonly FieldInfo Materials = typeof(DevMenuBase).GetField("m_RequiredMaterials", Fields);
        private static readonly FieldInfo Description = typeof(DevelopmetInfo).GetField("m_InfoText", Fields);
        private static readonly FieldInfo MaterialColumn = typeof(RequiredMaterials).GetField("m_NeedMatTextsRootL", Fields);
        private static readonly Color Foreground = new Color(0.91f, 0.98f, 0.95f);
        private static readonly Color Secondary = new Color(0.69f, 0.82f, 0.79f);

        private readonly AttachmentDevMenu menu;
        private readonly RectTransform materialsPanel;
        private readonly RectTransform root;
        private readonly TextMeshProUGUI template;
        private readonly List<TextMeshProUGUI> rows = new List<TextMeshProUGUI>();
        private string attachmentId;
        private int rowCount;

        private AttachmentCompatibilityView(AttachmentDevMenu menu, RectTransform materialsPanel, TextMeshProUGUI template)
        {
            this.menu = menu;
            this.materialsPanel = materialsPanel;
            this.template = template;
            root = new GameObject("Attachment compatibility", typeof(RectTransform), typeof(Image))
                .GetComponent<RectTransform>();
            root.SetParent(menu.transform, false);
            root.anchorMin = root.anchorMax = ((RectTransform)menu.transform).pivot;
            root.pivot = new Vector2(1f, 0f);
            var background = root.GetComponent<Image>();
            background.color = new Color(0.05f, 0.09f, 0.09f, 0.94f);
            background.raycastTarget = false;
            var heading = Text(19f);
            heading.text = "Compatible weapons";
            heading.color = Secondary;
            Place(heading.rectTransform, 8f, 26f);
            Hide();
        }

        public static AttachmentCompatibilityView TryCreate(AttachmentDevMenu menu)
        {
            var info = Info?.GetValue(menu) as DevelopmetInfo;
            var materials = Materials?.GetValue(menu) as RequiredMaterials;
            var font = info == null ? null : Description?.GetValue(info) as TextMeshProUGUI;
            var column = materials == null ? null : MaterialColumn?.GetValue(materials) as RectTransform;
            return font != null && font.font != null && column != null && column.parent is RectTransform panel
                ? new AttachmentCompatibilityView(menu, panel, font) : null;
        }

        public bool IsFor(AttachmentDevMenu candidate) => root != null && menu == candidate;

        public void Show(AttachmentDevData attachment)
        {
            var catalog = WeaponDevDatas.instance;
            var parameters = CSVParams.instance;
            if (catalog == null || parameters == null || materialsPanel == null)
            {
                Hide();
                return;
            }

            if (attachmentId != attachment.m_ItemId)
            {
                attachmentId = attachment.m_ItemId;
                var fits = CompatibleWeapons(attachment, catalog, parameters);
                UpdateRows(fits);
            }

            var parent = (RectTransform)root.parent;
            Vector3 right = parent.InverseTransformPoint(materialsPanel.TransformPoint(materialsPanel.rect.max));
            Vector3 left = parent.InverseTransformPoint(materialsPanel.TransformPoint(materialsPanel.rect.min));
            float width = Mathf.Min(490f, right.x - left.x);
            float bottom = right.y + 49f;
            float availableHeight = parent.rect.yMax - 134f - bottom;
            float rowHeight = Mathf.Clamp((availableHeight - 46f) / rowCount, 14f, 26f);
            for (int index = 0; index < rowCount; index++)
            {
                var row = rows[index];
                row.fontSizeMax = Mathf.Min(20f, rowHeight - 4f);
                row.fontSizeMin = Mathf.Min(16f, row.fontSizeMax);
                Place(row.rectTransform, 36f + index * rowHeight, rowHeight);
            }
            root.sizeDelta = new Vector2(width, 46f + rowCount * rowHeight);
            root.anchoredPosition = new Vector2(right.x, bottom);
            root.gameObject.SetActive(true);
        }

        private void UpdateRows((string Name, bool Developed)[] fits)
        {
            rowCount = 0;
            if (fits.Length == 0)
                AddRow("No known weapons fit", Secondary);
            bool addedHeading = false;
            foreach (var weapon in fits)
            {
                if (!weapon.Developed && !addedHeading)
                {
                    AddRow("Not developed", Secondary);
                    addedHeading = true;
                }
                AddRow(weapon.Name, Foreground);
            }
            for (int index = rowCount; index < rows.Count; index++)
                rows[index].gameObject.SetActive(false);
        }

        private void AddRow(string name, Color color)
        {
            if (rowCount == rows.Count)
            {
                var label = Text(20f);
                label.enableAutoSizing = true;
                label.textWrappingMode = TextWrappingModes.NoWrap;
                label.overflowMode = TextOverflowModes.Ellipsis;
                rows.Add(label);
            }
            var row = rows[rowCount++];
            row.text = name;
            row.color = color;
            row.gameObject.SetActive(true);
        }

        internal static (string Name, bool Developed)[] CompatibleWeapons(
            AttachmentDevData attachment, WeaponDevDatas catalog, CSVParams parameters)
        {
            return catalog.GetAllWeaponDevList().OfType<WeaponDevData>()
                .Where(weapon => !weapon.IsSecletInfo() && weapon.m_AvailableAttachmentScale != null &&
                    weapon.m_AvailableAttachments != null && weapon.IsCanBeWorn(attachment.m_AttachmentType) &&
                    weapon.GetAttEquipScale(attachment.m_ItemId) > 0f &&
                    parameters.GetCSVParamsSt(weapon.m_ItemId, "ATT_CATEGORY")?.Any(category =>
                        Enum.TryParse(category, out AttachmentType slot) && slot == attachment.m_AttachmentType) == true)
                .Select(weapon => (Name: Inventory.GetItemName(weapon.m_ItemId), Developed: catalog.IsDeveloped(weapon.m_ItemId)))
                .Where(weapon => !string.IsNullOrWhiteSpace(weapon.Name))
                .Distinct()
                .OrderByDescending(weapon => weapon.Developed)
                .ThenBy(weapon => weapon.Name, StringComparer.CurrentCulture)
                .ToArray();
        }

        public void Hide()
        {
            attachmentId = null;
            if (root != null)
                root.gameObject.SetActive(false);
        }

        public void Destroy()
        {
            if (root != null)
            {
                Hide();
                Object.Destroy(root.gameObject);
            }
        }

        private static void Place(RectTransform rect, float top, float height)
        {
            rect.offsetMin = new Vector2(14f, -top - height);
            rect.offsetMax = new Vector2(-14f, -top);
        }

        private TextMeshProUGUI Text(float size)
        {
            var label = new GameObject("Compatibility text", typeof(RectTransform), typeof(TextMeshProUGUI))
                .GetComponent<TextMeshProUGUI>();
            label.rectTransform.SetParent(root, false);
            label.rectTransform.anchorMin = new Vector2(0f, 1f);
            label.rectTransform.anchorMax = Vector2.one;
            label.font = template.font;
            label.fontSharedMaterial = template.fontSharedMaterial;
            label.fontSize = size;
            label.alignment = TextAlignmentOptions.MidlineLeft;
            label.raycastTarget = false;
            label.richText = false;
            return label;
        }
    }
}
