using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace AnomalithMinimap
{
    internal sealed class NativeMapPoints
    {
        private const BindingFlags Fields = BindingFlags.Instance | BindingFlags.NonPublic;
        private static readonly FieldInfo MainIcon = typeof(PauseMenuMap).GetField("m_QuestIconPrefab", Fields);
        private static readonly FieldInfo SideIcon = typeof(PauseMenuMap).GetField("m_SubQuestIconPrefab", Fields);
        private static readonly FieldInfo DoorIcon = typeof(PauseMenuMap).GetField("m_DoorIconPrefab", Fields);
        private static readonly FieldInfo WarpIcon = typeof(PauseMenuMap).GetField("m_WarpIconPrefab", Fields);
        private static readonly FieldInfo PortalIcon = typeof(PauseMenuMap).GetField("m_PortalIconPrefab", Fields);
        private static readonly FieldInfo TravelIcon = typeof(PauseMenuMap).GetField("m_WarpPointIconPrefab", Fields);

        private readonly Icon main;
        private readonly Icon side;
        private readonly Icon door;
        private readonly Icon warp;
        private readonly Icon portal;
        private readonly Icon travel;

        public NativeMapPoints(PauseMenuMap menu)
        {
            main = new Icon(MainIcon, menu, "Main objective", 20);
            side = new Icon(SideIcon, menu, "Side objective", 18);
            door = new Icon(DoorIcon, menu, "Door", 16);
            warp = new Icon(WarpIcon, menu, "Area connection", 16);
            portal = new Icon(PortalIcon, menu, "Portal", 18);
            travel = new Icon(TravelIcon, menu, "Fast travel", 15);
        }

        public void Collect(NativeMap native, MapImageSetData map, MapManager manager, int floor, List<Point> points)
        {
            points.Clear();
            var thresholds = manager.m_GetFloorPoint;

            void Add(Transform transform, Icon icon, bool rotate = false)
            {
                if (transform == null || icon.Sprite == null)
                    return;
                Vector3 position = transform.position;
                if (NativeMap.FindFloor(thresholds, position.y) == floor)
                    points.Add(new Point(native.Project(map, position), icon, rotate ? -transform.eulerAngles.y : 0, rotate));
            }

            void AddObjectives(List<GameObject> objectives, Icon icon)
            {
                if (objectives == null)
                    return;
                foreach (var objective in objectives)
                    if (objective != null)
                        Add(objective.transform, icon);
            }

            var quests = QuestManager.Instance;
            if (quests != null)
            {
                AddObjectives(quests.GetCurrentMainQuestTriggerList(), main);
                AddObjectives(quests.GetCurrentSubQuestTriggerList(), side);
            }

            var portals = manager.GetAllPortal();
            if (portals != null)
                foreach (var entry in portals)
                    if (entry != null)
                        Add(entry.transform, portal, true);

            var travelPoints = manager.GetAllMapWarpPoints();
            var questSave = SaveRootGame.Quest;
            if (questSave != null && !questSave.fastTravelLock && travelPoints != null)
                foreach (var entry in travelPoints)
                    if (entry != null)
                        Add(entry.transform, travel);

            var doors = manager.GetAllConnectors();
            if (doors != null)
                foreach (var entry in doors)
                    if (entry != null && entry.gameObject.activeInHierarchy && !entry.isChallenge && entry.NextEntranceNum != default)
                        Add(entry.transform, door, true);

            var warps = manager.GetAllWarps();
            if (warps != null)
                foreach (var entry in warps)
                    if (entry != null && entry.NextEntranceNum != default)
                        Add(entry.transform, warp, true);
        }

        internal readonly struct Icon
        {
            public readonly Sprite Sprite;
            public readonly Color Color;
            public readonly string Name;
            public readonly float Size;

            public Icon(FieldInfo field, PauseMenuMap menu, string name, float size)
            {
                var prefab = field?.GetValue(menu) as Image;
                Sprite = prefab == null ? null : prefab.sprite;
                Color = prefab == null ? UnityEngine.Color.white : prefab.color;
                Name = name;
                Size = size;
            }
        }

        internal readonly struct Point
        {
            public readonly Vector2 Position;
            public readonly Icon Appearance;
            public readonly float Heading;
            public readonly bool RotatesWithMap;

            public Point(Vector2 position, Icon appearance, float heading, bool rotatesWithMap)
            {
                Position = position;
                Appearance = appearance;
                Heading = heading;
                RotatesWithMap = rotatesWithMap;
            }
        }
    }
}
