using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AnomalithUpgradeTracker
{
    internal sealed class WorkshopChecklistView
    {
        private const float Width = 618f;
        private const float RowHeight = 44f;
        private const float RowsTop = 68f;
        private const BindingFlags Private = BindingFlags.Instance | BindingFlags.NonPublic;
        private static readonly FieldInfo DialogPrefabs = typeof(DialogManager).GetField("m_DialogPrefabList", Private);
        private static readonly FieldInfo Materials = typeof(DevMenuBase).GetField("m_RequiredMaterials", Private);
        private static readonly FieldInfo MaterialPrefab = typeof(RequiredMaterials).GetField("m_NeedMatTextPrefab", Private);
        private static readonly FieldInfo SortTabs = typeof(DevMenuBase).GetField("m_SortTab", Private);
        private static readonly FieldInfo TabSprites = typeof(PauseMenuTabGroup).GetField("pageSpriteList", Private);
        private static readonly FieldInfo NormalColor = typeof(RequiredMaterials).GetField("m_NormalTextColor", Private);
        private static readonly FieldInfo ShortageColor = typeof(RequiredMaterials).GetField("m_ShortageTextColor", Private);
        private static readonly FieldInfo EnemyDrops = typeof(RequiredMaterials).GetField("EnemyDropItems", Private);
        private static readonly FieldInfo IconMap = typeof(DocumentIcon).GetField("cachedIconMap",
            BindingFlags.Static | BindingFlags.NonPublic);

        private readonly GameObject root;
        private readonly RectTransform panel;
        private readonly TextMeshProUGUI title;
        private readonly Image categoryIcon;
        private readonly TextMeshProUGUI caption;
        private readonly TextMeshProUGUI sourceText;
        private readonly TextMeshProUGUI nativeName;
        private readonly TextMeshProUGUI nativeCount;
        private readonly Sprite materialIcon;
        private readonly Color normalColor;
        private readonly Color shortageColor;
        private readonly EnemyDropItemList enemyDrops;
        private readonly Texture2D rowTexture;
        private readonly List<Row> rows = new List<Row>();
        private readonly List<DialogBase> prefabs;
        private readonly bool embedded;
        private PinnedRecipe displayedRecipe;
        private float displayedWidth;

        private WorkshopChecklistView(Transform owner, RequiredMaterials materials, NeedMatText template,
            List<DialogBase> prefabs, bool embedded)
        {
            this.prefabs = prefabs;
            this.embedded = embedded;
            nativeName = template.m_MatName;
            nativeCount = template.m_MatNum;
            materialIcon = template.m_Icon?.sprite;
            normalColor = NormalColor?.GetValue(materials) is Color32 normal ? (Color)normal : nativeName.color;
            shortageColor = ShortageColor?.GetValue(materials) is Color32 shortage
                ? (Color)shortage : new Color(1f, 0.63f, 0.46f);
            enemyDrops = EnemyDrops?.GetValue(materials) as EnemyDropItemList;
            rowTexture = new Texture2D(1, 4, TextureFormat.RGBA32, false)
            {
                name = "Workshop checklist row",
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Repeat
            };
            rowTexture.SetPixels(new[]
            {
                new Color(0.09f, 0.11f, 0.11f, 0.7f), new Color(0.17f, 0.19f, 0.19f, 0.7f),
                new Color(0.17f, 0.19f, 0.19f, 0.7f), new Color(0.17f, 0.19f, 0.19f, 0.7f)
            });
            rowTexture.Apply(false, true);

            root = new GameObject("Pinned Workshop Checklist", typeof(RectTransform));
            root.transform.SetParent(owner, false);
            if (embedded)
            {
                var frame = (RectTransform)root.transform;
                frame.anchorMin = Vector2.zero;
                frame.anchorMax = Vector2.one;
                frame.pivot = ((RectTransform)owner).pivot;
                frame.offsetMin = frame.offsetMax = Vector2.zero;
            }
            else
            {
                var canvas = root.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvas.sortingOrder = 70;
                var scaler = root.AddComponent<CanvasScaler>();
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1920f, 1080f);
                scaler.matchWidthOrHeight = 1f;
            }

            panel = CreateRect("Checklist", root.transform);
            panel.anchorMin = panel.anchorMax = new Vector2(1f, 1f);
            panel.pivot = new Vector2(1f, 1f);
            panel.anchoredPosition = new Vector2(-26f, -39f);
            var backdrop = panel.gameObject.AddComponent<Image>();
            backdrop.raycastTarget = false;
            backdrop.color = new Color(0.04f, 0.07f, 0.07f, 0.90f);

            title = AddText("Pinned upgrade", nativeCount, 26f, TextAlignmentOptions.MidlineLeft);
            var category = CreateRect("Workshop category icon", panel);
            Place(category, 17f, 7f, 34f, 34f);
            categoryIcon = category.gameObject.AddComponent<Image>();
            categoryIcon.preserveAspect = true;
            categoryIcon.raycastTarget = false;
            categoryIcon.color = normalColor;
            caption = AddText("Recipe status", nativeName, 20f, TextAlignmentOptions.MidlineLeft);
            caption.text = "Still needed";
            caption.color = new Color(0.68f, 0.82f, 0.79f);
            Place(caption.rectTransform, 8f, 42f, Width - 16f, 26f);

            sourceText = AddText("Material sources", nativeName, 18f, TextAlignmentOptions.TopLeft);
            sourceText.textWrappingMode = TextWrappingModes.Normal;
            root.SetActive(false);
        }

        public GameObject Root => root;

        public static WorkshopChecklistView TryCreate(Transform owner, bool embedded = false)
        {
            var manager = DialogManager.Instance;
            if (manager == null || !(DialogPrefabs?.GetValue(manager) is List<DialogBase> prefabs))
                return null;
            var workshop = prefabs.OfType<WeaponDevMenu>().FirstOrDefault();
            if (workshop == null || !(Materials?.GetValue(workshop) is RequiredMaterials materials) ||
                materials == null || !(MaterialPrefab?.GetValue(materials) is GameObject rowPrefab) ||
                rowPrefab == null || !rowPrefab.TryGetComponent<NeedMatText>(out var row) ||
                row.m_MatName == null || row.m_MatNum == null)
                return null;
            return new WorkshopChecklistView(owner, materials, row, prefabs, embedded);
        }

        public void Show(PinnedRecipe recipe, string[] materials, int[] held, string sources, string completionStatus)
        {
            SetHeading(recipe, Width);
            bool complete = !string.IsNullOrEmpty(completionStatus);
            caption.text = complete ? completionStatus : "Still needed";
            caption.color = completionStatus == PinnedRecipe.ReadyToBuild || completionStatus == PinnedRecipe.Built
                ? new Color(0.59f, 0.91f, 0.75f) : new Color(0.68f, 0.82f, 0.79f);
            int visibleRows = complete ? 0 : materials.Length;
            SetRowCount(visibleRows);
            for (int index = 0; index < visibleRows; index++)
            {
                var material = recipe.materials[index];
                SetRow(rows[index], RowsTop + RowHeight * index, Width, material.id, materials[index],
                    held[index], material.required);
            }

            float height = RowsTop + RowHeight * visibleRows + 6f;
            bool showSources = !complete && !string.IsNullOrEmpty(sources);
            sourceText.gameObject.SetActive(showSources);
            if (showSources)
            {
                sourceText.text = sources;
                float textWidth = Width - 36f;
                float textHeight = sourceText.GetPreferredValues(sources, textWidth, 0).y;
                Place(sourceText.rectTransform, 18f, height + 3f, textWidth, textHeight + 3f);
                height += textHeight + 14f;
            }
            panel.sizeDelta = new Vector2(Width, height);
            root.SetActive(true);
        }

        public float LayoutDeploy(PinnedRecipe recipe, IReadOnlyList<DeployMaterial> materials, float width)
        {
            root.SetActive(true);
            SetHeading(recipe, width);
            caption.gameObject.SetActive(false);
            sourceText.gameObject.SetActive(false);
            SetRowCount(materials.Count);

            float y = 48f;
            for (int index = 0; index < materials.Count; index++)
            {
                var material = materials[index];
                var row = rows[index];
                SetRow(row, y, width, material.Id, material.Name, material.Held, material.Required);
                y += RowHeight;
                row.Source.text = material.Source;
                const float inset = 16f;
                float detailWidth = width - 2 * inset;
                float detailHeight = Mathf.Max(22f,
                    Mathf.Ceil(row.Source.GetPreferredValues(material.Source, detailWidth, Mathf.Infinity).y));
                Place(row.Source.rectTransform, inset, y + 3f, detailWidth, detailHeight + 2f);
                y += detailHeight + 10f;
            }
            return y;
        }

        public void ShowDeploy(Rect bounds)
        {
            var parent = (RectTransform)panel.parent;
            panel.anchorMin = panel.anchorMax = parent.pivot;
            panel.anchoredPosition = new Vector2(bounds.xMin, bounds.yMax);
            panel.pivot = new Vector2(0f, 1f);
            panel.sizeDelta = bounds.size;
            root.SetActive(true);
        }

        private void SetHeading(PinnedRecipe recipe, float width)
        {
            title.text = recipe.DisplayName;
            if (displayedRecipe != recipe || displayedWidth != width || categoryIcon.sprite == null)
            {
                displayedRecipe = recipe;
                displayedWidth = width;
                var (menu, index) = recipe.GetWorkshopCategory();
                int menuIndex = (int)menu;
                Sprite sprite = null;
                if (index >= 0 && menuIndex >= 0 && menuIndex < prefabs.Count &&
                    prefabs[menuIndex] is DevMenuBase prefab &&
                    SortTabs?.GetValue(prefab) is PauseMenuTabGroup tabs && tabs != null &&
                    TabSprites?.GetValue(tabs) is List<Sprite> sprites && index < sprites.Count)
                    sprite = sprites[index];
                categoryIcon.sprite = sprite;
                categoryIcon.enabled = sprite != null;
                float titleLeft = categoryIcon.enabled ? 60f : 16f;
                Place(title.rectTransform, titleLeft, 4f, width - titleLeft - 16f, 40f);
            }
        }

        private void SetRowCount(int count)
        {
            while (rows.Count < count)
                rows.Add(CreateRow());
            for (int index = 0; index < rows.Count; index++)
            {
                rows[index].Rect.gameObject.SetActive(index < count);
                if (embedded)
                    rows[index].Source.gameObject.SetActive(index < count);
            }
        }

        public void Hide()
        {
            if (root != null)
                root.SetActive(false);
        }

        public void Destroy()
        {
            Hide();
            Object.Destroy(root);
            Object.Destroy(rowTexture);
        }

        private Row CreateRow()
        {
            var row = CreateRect("Material row", panel);
            var background = row.gameObject.AddComponent<RawImage>();
            background.texture = rowTexture;
            background.uvRect = new Rect(0, 0, 1, RowHeight / rowTexture.height);
            background.raycastTarget = false;

            var name = AddText("Name", nativeName, 24f, TextAlignmentOptions.MidlineLeft, row);
            var count = AddText("Count", nativeCount, 24f, TextAlignmentOptions.MidlineRight, row);

            var iconRect = CreateRect("Material icon", row);
            Place(iconRect, 24f, 6f, 32f, 32f);
            var icon = iconRect.gameObject.AddComponent<Image>();
            icon.preserveAspect = true;
            icon.raycastTarget = false;
            TextMeshProUGUI source = null;
            if (embedded)
            {
                source = AddText("Material source", nativeName, 17f, TextAlignmentOptions.TopLeft);
                source.textWrappingMode = TextWrappingModes.Normal;
                source.color = caption.color;
                name.enableAutoSizing = true;
                name.fontSizeMin = 17f;
                name.fontSizeMax = 24f;
            }
            return new Row(row, name, count, icon, source);
        }

        private void SetRow(Row row, float top, float width, string id, string name, int held, int required)
        {
            Place(row.Rect, 0f, top, width, RowHeight);
            float countWidth = embedded ? 86f : 123f;
            Place(row.Name.rectTransform, 60f, 0f, width - countWidth - 81f, RowHeight);
            Place(row.Count.rectTransform, width - countWidth - 21f, 0f, countWidth, RowHeight);
            row.Name.text = name;
            row.Count.text = $"{held}/{required}";
            row.Name.color = row.Count.color = held < required ? shortageColor : normalColor;
            if (row.MaterialId != id)
            {
                row.MaterialId = id;
                row.EnemyId = enemyDrops?.GetEnemiesWhoDropItem(id).FirstOrDefault();
            }
            Sprite icon = materialIcon;
            if (!string.IsNullOrEmpty(row.EnemyId) &&
                SaveRootGame.Achivement?.GetObjectCount(row.EnemyId) > 0 &&
                IconMap?.GetValue(null) is DocumentIconMap map && map != null)
                icon = map.GetIcon(row.EnemyId) ?? icon;
            row.Icon.sprite = icon;
            row.Icon.enabled = icon != null;
        }

        private TextMeshProUGUI AddText(string name, TextMeshProUGUI template,
            float size, TextAlignmentOptions alignment, Transform parent = null)
        {
            var target = CreateRect(name, parent == null ? panel : parent);
            var label = target.gameObject.AddComponent<TextMeshProUGUI>();
            label.raycastTarget = false;
            label.richText = false;
            label.font = template.font;
            label.fontSharedMaterial = template.fontSharedMaterial;
            label.fontStyle = template.fontStyle;
            label.color = template.color;
            label.fontSize = size;
            label.alignment = alignment;
            label.overflowMode = TextOverflowModes.Ellipsis;
            label.textWrappingMode = TextWrappingModes.NoWrap;
            return label;
        }

        private static RectTransform CreateRect(string name, Transform parent)
        {
            var rect = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            return rect;
        }

        private static void Place(RectTransform rect, float x, float y, float width, float height)
        {
            rect.anchorMin = rect.anchorMax = new Vector2(0, 1);
            rect.pivot = new Vector2(0, 1);
            rect.anchoredPosition = new Vector2(x, -y);
            rect.sizeDelta = new Vector2(width, height);
        }

        private sealed class Row
        {
            public Row(RectTransform rect, TextMeshProUGUI name, TextMeshProUGUI count, Image icon, TextMeshProUGUI source)
            {
                Rect = rect;
                Name = name;
                Count = count;
                Icon = icon;
                Source = source;
            }

            public RectTransform Rect { get; }
            public TextMeshProUGUI Name { get; }
            public TextMeshProUGUI Count { get; }
            public Image Icon { get; }
            public TextMeshProUGUI Source { get; }
            public string MaterialId { get; set; }
            public string EnemyId { get; set; }
        }

        internal readonly struct DeployMaterial
        {
            public DeployMaterial(string id, string name, int held, int required, string source)
            {
                Id = id;
                Name = name;
                Held = held;
                Required = required;
                Source = source;
            }

            public string Id { get; }
            public string Name { get; }
            public int Held { get; }
            public int Required { get; }
            public string Source { get; }
        }
    }
}
