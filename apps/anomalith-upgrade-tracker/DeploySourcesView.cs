using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AnomalithUpgradeTracker
{
    internal sealed class DeploySourcesView
    {
        private const BindingFlags Fields = BindingFlags.Instance | BindingFlags.NonPublic;
        private static readonly FieldInfo Groups = typeof(PortalWarpDialog).GetField("mapGroupList", Fields);
        private static readonly FieldInfo SelectedGroup = typeof(PortalWarpDialog).GetField("groupCursorIdx", Fields);
        private static readonly FieldInfo Graph = typeof(PortalWarpDialog).GetField("m_MapIconUI", Fields);
        private static readonly FieldInfo TileText = typeof(MapIcon).GetField("m_MapName", Fields);
        private static readonly FieldInfo ActiveArm = typeof(MapIconArm).GetField("m_OnObj", Fields);
        private static readonly Color Accent = new Color(0.49f, 0.86f, 0.79f);
        private static readonly Color Foreground = new Color(0.91f, 0.98f, 0.95f);
        private static readonly Color Secondary = new Color(0.69f, 0.82f, 0.79f);
        private static readonly Color Surface = new Color(0.035f, 0.067f, 0.068f, 0.96f);

        private readonly PortalWarpDialog dialog;
        private readonly RectTransform root;
        private readonly MapIconUI graph;
        private readonly List<PortalWarpDialog.MapList> groups;
        private readonly Vector3[] corners = new Vector3[4];
        private TableData indexedTables;
        private Dictionary<string, HashSet<string>> lootAreas;
        private WorkshopChecklistView checklist;
        private string shown;

        public DeploySourcesView(PortalWarpDialog dialog)
        {
            this.dialog = dialog;
            groups = Groups?.GetValue(dialog) as List<PortalWarpDialog.MapList>;
            graph = Graph?.GetValue(dialog) as MapIconUI;
            root = new GameObject("Upgrade Tracker Deploy Sources", typeof(RectTransform))
                .GetComponent<RectTransform>();
            root.SetParent(dialog.transform, false);
            root.anchorMin = Vector2.zero;
            root.anchorMax = Vector2.one;
            root.offsetMin = root.offsetMax = Vector2.zero;
        }

        public bool IsFor(PortalWarpDialog candidate) => root != null && dialog == candidate;
        public void Hide()
        {
            if (root != null)
                root.gameObject.SetActive(false);
        }

        public void Destroy()
        {
            checklist?.Destroy();
            if (root != null)
            {
                Hide();
                UnityEngine.Object.Destroy(root.gameObject);
            }
        }

        public void Refresh(PinnedRecipe recipe, string[] names, int[] held)
        {
            if (graph == null || groups == null ||
                !(SelectedGroup?.GetValue(dialog) is int selected) || selected < 0 || selected >= groups.Count)
            {
                Hide();
                return;
            }

            var rows = groups.Where(group => group.buttonElement != null &&
                group.buttonElement.gameObject.activeInHierarchy).ToArray();
            var selectedButton = groups[selected].buttonElement;
            var font = selectedButton == null ? null : selectedButton.GetText;
            if (font == null || font.font == null)
            {
                Hide();
                return;
            }

            string zone = GroupId(groups[selected]);
            if (checklist == null)
            {
                checklist = WorkshopChecklistView.TryCreate(root, embedded: true);
                if (checklist != null)
                    shown = null;
            }
            var otherButtons = dialog.GetComponentsInChildren<PortalWarpMenuButton>()
                .Where(button => button.isActiveAndEnabled && !rows.Any(group => group.buttonElement == button))
                .ToArray();
            var tiles = graph.GetComponentsInChildren<MapIcon>()
                .Where(icon => icon.isActiveAndEnabled && !string.IsNullOrEmpty(icon.GetMapId))
                .Select(icon => new
                {
                    Icon = icon,
                    Rect = icon.transform as RectTransform,
                    Name = (TileText?.GetValue(icon) as TextMeshProUGUI)?.text
                }).Where(tile => tile.Rect != null).ToArray();
            var tables = MapManager.Instance == null ? null : MapManager.Instance.GetTableData;
            bool tablesReady = tables?.itemTableMap?.Count > 0;
            if (lootAreas == null || (tablesReady && !ReferenceEquals(tables, indexedTables)))
            {
                lootAreas = MaterialSources.IndexAreaLoot(tables);
                indexedTables = tablesReady ? tables : null;
                shown = null;
            }

            string signature = recipe.kind + "|" + recipe.recipeId + "|" + recipe.weaponId + "|" +
                recipe.DisplayName + "|" + zone + "|" + root.rect.size + "|" + Screen.width + "x" + Screen.height + "|" +
                string.Join("/", rows.Select(GroupId)) + "|" +
                string.Join("/", otherButtons.Select(button => button.GetInstanceID())) + "|" +
                string.Join("/", tiles.Select(tile => tile.Icon.GetMapId + "=" + tile.Name)) + "|" +
                string.Join("/", recipe.materials.Select((item, index) => item.id + "=" + held[index] + "/" + item.required));
            root.gameObject.SetActive(true);
            if (signature == shown)
                return;
            shown = signature;
            root.SetAsLastSibling();
            for (int i = root.childCount - 1; i >= 0; i--)
            {
                GameObject previous = root.GetChild(i).gameObject;
                if (checklist != null && previous == checklist.Root)
                    continue;
                previous.SetActive(false);
                UnityEngine.Object.Destroy(previous);
            }

            var items = recipe.materials.Select((material, index) => (material, index))
                .Where(item => held[item.index] < item.material.required)
                .Select(item => new Source(item.material.id, names[item.index], held[item.index],
                    item.material.required, lootAreas)).ToArray();
            var discovered = new List<(string Id, string Name, Rect Rect)>();
            var obstacles = new List<Rect>();
            foreach (var tile in tiles)
            {
                Rect bounds = Bounds(tile.Rect);
                obstacles.Add(Expand(bounds, 5));
                string id = tile.Icon.GetMapId;
                if (!string.IsNullOrWhiteSpace(tile.Name) && tile.Name != "?" && MaterialSources.ZoneId(id) == zone)
                    discovered.Add((id, tile.Name, bounds));
            }
            foreach (var arm in graph.GetComponentsInChildren<MapIconArm>())
                if (ActiveArm?.GetValue(arm) is GameObject line && line.activeInHierarchy &&
                    line.transform is RectTransform rect)
                    obstacles.Add(Bounds(rect));

            Rect selectedRow = Bounds(selectedButton.GetRectTransform);
            float left = selectedRow.xMin;
            float right = selectedRow.xMax;
            float bottom = selectedRow.yMin;
            foreach (var group in rows)
            {
                Rect row = Bounds(group.buttonElement.GetRectTransform);
                left = Mathf.Min(left, row.xMin);
                right = Mathf.Max(right, row.xMax);
                bottom = Mathf.Min(bottom, row.yMin);
                obstacles.Add(Expand(row, 15));
                if (items.Any(item => item.Zones.Contains(GroupId(group))))
                    Paint(new Rect(row.xMax + 10, row.center.y - 4, 8, 8), Accent);
            }

            foreach (var button in otherButtons)
            {
                Rect row = Bounds(button.GetRectTransform);
                obstacles.Add(Expand(row, 6));
                if (row.xMin < right && row.xMax > left)
                    bottom = Mathf.Min(bottom, row.yMin);
            }

            DrawSummary(recipe, items, discovered, groups.Select(GroupId).ToArray(), zone,
                left, right, bottom, obstacles);

            var labeled = new HashSet<string>(StringComparer.Ordinal);
            foreach (var tile in discovered.OrderByDescending(tile => tile.Rect.yMax).ThenBy(tile => tile.Rect.xMin))
            {
                string label = string.Join("\n", items.Where(item => item.HasArea(tile.Id)).Select(item => item.Name));
                if (label.Length == 0)
                    continue;

                DrawOutline(tile.Rect);
                if (!labeled.Contains(label) && TryDrawTileLabel(tile.Rect, font, label, obstacles))
                    labeled.Add(label);
            }
        }

        private void DrawSummary(PinnedRecipe recipe, Source[] items,
            List<(string Id, string Name, Rect Rect)> discovered, string[] available, string zone,
            float left, float right, float bottom, List<Rect> obstacles)
        {
            float width = Mathf.Min(right - left, 480f);
            if (checklist == null || width < 250)
            {
                checklist?.Hide();
                return;
            }

            var materials = new List<WorkshopChecklistView.DeployMaterial>(items.Length);
            foreach (var item in items)
            {
                var lines = new List<string>();
                var encounters = discovered.Where(tile => item.Encounters.Contains(tile.Id))
                    .OrderBy(tile => tile.Id, StringComparer.Ordinal).ToArray();
                var possible = discovered.Where(tile => item.Loot.Contains(tile.Id) && !item.Encounters.Contains(tile.Id))
                    .Select(tile => tile.Name).Distinct().OrderBy(name => name).ToArray();
                if (encounters.Length == 1)
                {
                    var encounter = encounters[0];
                    string section = MaterialSources.CurrentSection(encounter.Id).Split('/').Last().Trim();
                    lines.Add(encounter.Name + " · " + section);
                    lines.Add(MaterialSources.EnemyDropNote(item.Encounters[encounter.Id]));
                }
                else if (encounters.Length > 1)
                {
                    lines.Add("Possible enemy drops · " +
                        SummarizeAreas(encounters.Select(tile => tile.Name).Distinct().ToArray(), discovered.Count));
                }
                if (possible.Length > 0)
                    lines.Add("Possible loot · " + SummarizeAreas(possible, discovered.Count));
                if (lines.Count == 0)
                {
                    if (item.Zones.Contains(zone))
                    {
                        lines.Add(item.HasAreaIn(zone) ? "Source in an undiscovered area" : "Area unlisted in Dive Reports");
                    }
                    else
                    {
                        var elsewhere = item.Zones.Where(id => available.Contains(id) || item.Documented.Contains(id))
                            .OrderByDescending(item.HasAreaIn)
                            .ThenBy(id => id)
                            .Select(id => MaterialSources.ZoneName(id) +
                                (available.Contains(id) ? "" : " (unavailable)")).ToArray();
                        lines.Add(elsewhere.Length > 0 ? string.Join("\n", elsewhere) : "No location listed");
                    }
                }

                materials.Add(new WorkshopChecklistView.DeployMaterial(item.Id, item.Name, item.Held,
                    item.Required, string.Join("\n", lines)));
            }

            float height = checklist.LayoutDeploy(recipe, materials, width);
            const float footerGap = 68;
            float footer = root.rect.yMin + footerGap;
            var candidates = new[]
            {
                new Rect(left, bottom - 26 - height, width, height),
                new Rect(root.rect.xMax - width - 38, footer, width, height),
                new Rect(root.rect.center.x - width / 2, footer, width, height)
            };
            Rect panel = candidates.FirstOrDefault(candidate => candidate.yMin >= footer &&
                root.rect.Contains(candidate.min) && root.rect.Contains(candidate.max) &&
                !obstacles.Any(obstacle => obstacle.Overlaps(candidate)));
            if (panel.width == 0)
            {
                checklist.Hide();
                return;
            }

            checklist.ShowDeploy(panel);
            obstacles.Add(Expand(panel, 8));
        }

        private bool TryDrawTileLabel(Rect tile, TextMeshProUGUI font, string content, List<Rect> obstacles)
        {
            var label = Text(font, content, 20, Foreground);
            float width = Mathf.Min(275, label.GetPreferredValues().x + 18);
            float height = LineHeight(label, width - 18) + 10;
            const float gap = 8;
            var candidates = new[]
            {
                new Rect(tile.xMax + gap, tile.yMax - height, width, height),
                new Rect(tile.xMin - gap - width, tile.yMax - height, width, height),
                new Rect(tile.center.x - width / 2, tile.yMax + gap, width, height),
                new Rect(tile.xMin, tile.yMax + gap, width, height),
                new Rect(tile.xMax - width, tile.yMax + gap, width, height),
                new Rect(tile.center.x - width / 2, tile.yMin - gap - height, width, height),
                new Rect(tile.xMin, tile.yMin - gap - height, width, height),
                new Rect(tile.xMax - width, tile.yMin - gap - height, width, height)
            };
            foreach (Rect candidate in candidates)
            {
                if (!root.rect.Contains(candidate.min) || !root.rect.Contains(candidate.max) ||
                    obstacles.Any(obstacle => obstacle.Overlaps(candidate)))
                    continue;

                Paint(candidate, Surface);
                Paint(new Rect(candidate.xMin, candidate.yMax - 1, candidate.width, 1), Accent);
                if (candidate.yMin >= tile.yMax && candidate.xMin < tile.center.x && candidate.xMax > tile.center.x)
                    Paint(new Rect(tile.center.x - 1, tile.yMax, 2, gap), Accent);
                if (candidate.yMax <= tile.yMin && candidate.xMin < tile.center.x && candidate.xMax > tile.center.x)
                    Paint(new Rect(tile.center.x - 1, candidate.yMax, 2, gap), Accent);
                Place(label.rectTransform, new Rect(candidate.xMin + 9, candidate.yMin + 5, width - 18, height - 10));
                label.rectTransform.SetAsLastSibling();
                obstacles.Add(Expand(candidate, 5));
                return true;
            }
            label.gameObject.SetActive(false);
            return false;
        }

        private void DrawOutline(Rect tile)
        {
            const float line = 3;
            Paint(new Rect(tile.xMin, tile.yMin, tile.width, line), Accent);
            Paint(new Rect(tile.xMin, tile.yMax - line, tile.width, line), Accent);
            Paint(new Rect(tile.xMin, tile.yMin + line, line, tile.height - 2 * line), Accent);
            Paint(new Rect(tile.xMax - line, tile.yMin + line, line, tile.height - 2 * line), Accent);
        }

        private Rect Bounds(RectTransform target)
        {
            target.GetWorldCorners(corners);
            Vector2 min = root.InverseTransformPoint(corners[0]);
            Vector2 max = root.InverseTransformPoint(corners[2]);
            return Rect.MinMaxRect(min.x, min.y, max.x, max.y);
        }

        private static Rect Expand(Rect rect, float amount) =>
            Rect.MinMaxRect(rect.xMin - amount, rect.yMin - amount, rect.xMax + amount, rect.yMax + amount);

        private static string GroupId(PortalWarpDialog.MapList group) => "map_" + group.mapGroupID;

        private static float LineHeight(TextMeshProUGUI text, float width) =>
            Mathf.Ceil(text.GetPreferredValues(text.text, width, 0).y) + 2;

        private static void Place(RectTransform transform, Rect rect)
        {
            var parent = (RectTransform)transform.parent;
            transform.anchorMin = transform.anchorMax = parent.pivot;
            transform.pivot = Vector2.zero;
            transform.anchoredPosition = rect.position;
            transform.sizeDelta = rect.size;
        }

        private RectTransform Paint(Rect rect, Color color)
        {
            var image = new GameObject("Source accent", typeof(RectTransform), typeof(Image)).GetComponent<Image>();
            image.transform.SetParent(root, false);
            image.raycastTarget = false;
            image.color = color;
            Place(image.rectTransform, rect);
            return image.rectTransform;
        }

        private TextMeshProUGUI Text(TextMeshProUGUI font, string content, float size, Color color)
        {
            var text = new GameObject("Source text", typeof(RectTransform), typeof(TextMeshProUGUI))
                .GetComponent<TextMeshProUGUI>();
            text.transform.SetParent(root, false);
            text.font = font.font;
            text.fontSharedMaterial = font.fontSharedMaterial;
            text.fontSize = size;
            text.alignment = TextAlignmentOptions.TopLeft;
            text.textWrappingMode = TextWrappingModes.Normal;
            text.color = color;
            text.richText = false;
            text.raycastTarget = false;
            text.text = content;
            return text;
        }

        private sealed class Source
        {
            public readonly string Name;
            public readonly string Id;
            public readonly int Held;
            public readonly int Required;
            public readonly ILookup<string, string> Encounters;
            public readonly HashSet<string> Loot;
            public readonly HashSet<string> Documented;
            public readonly HashSet<string> Zones;

            public Source(string id, string name, int held, int required, Dictionary<string, HashSet<string>> lootAreas)
            {
                Name = name;
                Id = id;
                Held = held;
                Required = required;
                id = Inventory.ConvertItemID(id);
                Encounters = MaterialSources.GetEncounters(id).ToLookup(entry => entry.Area, entry => entry.Enemy,
                    StringComparer.Ordinal);
                Loot = lootAreas.TryGetValue(id, out var areas) ? areas : new HashSet<string>(StringComparer.Ordinal);
                Documented = new HashSet<string>(MaterialSources.DocumentedZones(id).Select(MaterialSources.ZoneId),
                    StringComparer.Ordinal);
                Zones = new HashSet<string>(Documented, StringComparer.Ordinal);
                Zones.UnionWith(Loot.Select(MaterialSources.ZoneId));
                Zones.UnionWith(Encounters.Select(area => MaterialSources.ZoneId(area.Key)));
            }

            public bool HasArea(string id) => Encounters.Contains(id) || Loot.Contains(id);

            public bool HasAreaIn(string zone) =>
                Encounters.Any(area => MaterialSources.ZoneId(area.Key) == zone) ||
                Loot.Any(id => MaterialSources.ZoneId(id) == zone);
        }

        private static string SummarizeAreas(string[] names, int discoveredCount)
        {
            if (names.Length == discoveredCount && names.Length > 3)
                return "All discovered areas";
            string first = string.Join(", ", names.Take(3));
            return names.Length > 3 ? $"{first} +{names.Length - 3} more" : first;
        }
    }
}
