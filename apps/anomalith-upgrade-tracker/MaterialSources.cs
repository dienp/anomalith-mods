using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace AnomalithUpgradeTracker
{
    internal static class MaterialSources
    {
        public static IEnumerable<string> DocumentedZones(string itemId)
        {
            string listed = CSVParams.instance == null ? "" :
                CSVParams.instance.GetCSVParamSt(itemId, "spawnMap");
            return (listed ?? "").Split('/')
                .Select(zone => zone.Trim())
                .Where(zone => zone.StartsWith("map_", System.StringComparison.Ordinal))
                .Distinct();
        }

        public static string ZoneId(string areaId)
        {
            return !string.IsNullOrEmpty(areaId) && areaId.Length >= 5 ? areaId.Substring(0, 5) : "";
        }

        private static readonly Lazy<ILookup<string, (string Area, string Enemy)>> EnemySources =
            new Lazy<ILookup<string, (string Area, string Enemy)>>(ReadEnemySources);

        public static IEnumerable<(string Area, string Enemy)> GetEncounters(string itemId) =>
            EnemySources.Value[itemId];

        private static ILookup<string, (string Area, string Enemy)> ReadEnemySources()
        {
            var rows = new List<(string Item, string Area, string Enemy)>();
            using (var stream = typeof(MaterialSources).Assembly.GetManifestResourceStream("EnemySources.tsv"))
            using (var reader = new StreamReader(stream))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var fields = line.Split('\t');
                    if (fields.Length == 3 && fields[0].StartsWith("ITM_", StringComparison.Ordinal))
                        rows.Add((fields[0], fields[1], fields[2]));
                }
            }
            return rows.ToLookup(row => row.Item, row => (row.Area, row.Enemy), StringComparer.Ordinal);
        }

        public static string EnemyDropNote(IEnumerable<string> keys)
        {
            var strings = StringManager.instance;
            var enemyKeys = keys.Distinct().ToArray();
            if (strings == null || enemyKeys.Length > 2 || enemyKeys.Any(string.IsNullOrEmpty))
                return "Possible enemy drops";
            var names = enemyKeys.Select(key => Clean(strings.GetString("CHARACTERS", key), ""))
                .Where(name => name.Length > 0 && !enemyKeys.Contains(name)).Distinct().ToArray();
            return names.Length > 0 ? "Possible drop · " + string.Join(", ", names) : "Possible enemy drops";
        }

        public static Dictionary<string, HashSet<string>> IndexAreaLoot(TableData tables)
        {
            var results = new Dictionary<string, HashSet<string>>(System.StringComparer.Ordinal);
            if (tables?.mapTableData == null || tables.itemTableMap == null)
                return results;

            foreach (var map in tables.mapTableData)
            {
                if (map?.itemDataID == null || map.itemTableID == null ||
                    !map.itemTableID.Any(variant => !string.IsNullOrEmpty(variant)) ||
                    string.IsNullOrEmpty(map.mapID))
                    continue;

                foreach (var tableId in map.itemDataID)
                {
                    if (string.IsNullOrEmpty(tableId) || !tables.itemTableMap.TryGetValue(tableId, out var table) ||
                        table?.itemList == null)
                        continue;
                    foreach (var row in table.itemList)
                    {
                        if (row == null)
                            continue;
                        var drop = row.dropItemInfo;
                        if (drop.dropRate <= 0 || string.IsNullOrEmpty(drop.itemID))
                            continue;

                        string itemId = Inventory.ConvertItemID(drop.itemID);
                        if (!results.TryGetValue(itemId, out var areas))
                        {
                            areas = new HashSet<string>(System.StringComparer.Ordinal);
                            results.Add(itemId, areas);
                        }
                        areas.Add(map.mapID);
                    }
                }
            }
            return results;
        }

        public static string CurrentSection(string mapId)
        {
            if (string.IsNullOrWhiteSpace(mapId) || StringManager.instance == null)
                return "";

            return Clean(StringManager.instance.GetMapName(mapId, true), mapId);
        }

        public static string Describe(string itemId, string currentArea)
        {
            itemId = Inventory.ConvertItemID(itemId);
            var zones = DocumentedZones(itemId).ToList();
            var notes = new List<string>();
            var encounterAreas = GetEncounters(itemId).GroupBy(entry => entry.Area)
                .OrderByDescending(area => area.Key == currentArea)
                .ThenByDescending(area => ZoneId(area.Key) == ZoneId(currentArea)).ToArray();
            foreach (var area in encounterAreas.Take(2))
            {
                notes.Add($"{CurrentSection(area.Key)} · {EnemyDropNote(area.Select(entry => entry.Enemy))}");
                zones.Remove(ZoneId(area.Key));
            }
            if (encounterAreas.Length > 2)
                notes.Add($"+{encounterAreas.Length - 2} other areas on Deploy");

            if (zones.Count > 0)
                notes.Add("Dive Report zones: " + string.Join(", ", zones.Select(ZoneName)));
            if (notes.Count == 0)
                notes.Add("No zone listed in the Dive Reports.");

            return string.Join("\n", notes);
        }

        public static string ZoneName(string id)
        {
            if (id.Length != 5)
                return CurrentSection(id);
            var strings = StringManager.instance;
            return strings == null ? id : Clean(strings.GetString("MapName", id + "00_AreaName"), id);
        }

        private static string Clean(string text, string fallback)
        {
            if (string.IsNullOrWhiteSpace(text))
                return fallback;
            var parts = Regex.Replace(text, "<[^>]*>", "").Split('\n')
                .Select(part => part.Trim()).Where(part => part.Length > 0);
            return string.Join(" / ", parts);
        }
    }
}
