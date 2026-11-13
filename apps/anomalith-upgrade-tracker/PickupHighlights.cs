using System.Collections.Generic;
using UnityEngine;

namespace AnomalithUpgradeTracker
{
    internal sealed class PickupHighlights
    {
        private const float RangeSquared = 30f * 30f;
        private readonly List<Candidate> candidates = new List<Candidate>();
        private readonly List<Marker> markers = new List<Marker>(4);
        private float nextScan;

        public IReadOnlyList<Marker> Markers => markers;

        public void Refresh(IReadOnlyDictionary<string, (string Name, int Count)> needed)
        {
            markers.Clear();
            var camera = CameraCtrl.RetrieveMainCamera();
            if (camera == null || !camera.isActiveAndEnabled || needed.Count == 0)
            {
                Clear();
                return;
            }

            Vector3 origin = camera.transform.position;
            if (Time.unscaledTime >= nextScan)
            {
                nextScan = Time.unscaledTime + 0.5f;
                candidates.Clear();
                foreach (var pickup in Object.FindObjectsByType<Event_Item>(FindObjectsSortMode.None))
                {
                    float distance = (pickup.transform.position - origin).sqrMagnitude;
                    if (distance <= RangeSquared && TryMatch(pickup, needed, out _))
                        candidates.Add(new Candidate(pickup, distance));
                }
                candidates.Sort((left, right) => left.Distance.CompareTo(right.Distance));
            }

            int mask = ObjectBase.ObstructionLayerMask();
            foreach (var candidate in candidates)
            {
                var pickup = candidate.Pickup;
                if (pickup == null || !pickup.isActiveAndEnabled ||
                    !TryMatch(pickup, needed, out string name))
                    continue;

                Vector3 position = pickup.transform.position;
                if ((position - origin).sqrMagnitude > RangeSquared)
                    continue;

                Vector3 target = position + camera.transform.up * 0.28f;
                Vector3 viewport = camera.WorldToViewportPoint(target);
                if (viewport.z <= 0 || viewport.x <= 0 || viewport.x >= 1 ||
                    viewport.y <= 0 || viewport.y >= 1)
                    continue;

                if (Physics.Linecast(origin, target, out var hit,
                    mask, QueryTriggerInteraction.Ignore) && !hit.transform.IsChildOf(pickup.transform))
                    continue;

                Vector3 screen = camera.WorldToScreenPoint(target);
                markers.Add(new Marker(name, new Vector2(screen.x, Screen.height - screen.y)));
                if (markers.Count == 4)
                    break;
            }
        }

        public void Clear()
        {
            candidates.Clear();
            markers.Clear();
            nextScan = 0;
        }

        private static bool TryMatch(Event_Item pickup,
            IReadOnlyDictionary<string, (string Name, int Count)> needed, out string name)
        {
            if (pickup.IsEnable && pickup.ItemInfoList != null)
            {
                foreach (var item in pickup.ItemInfoList)
                {
                    if (item != null && !string.IsNullOrEmpty(item.itemID) &&
                        needed.TryGetValue(Inventory.ConvertItemID(item.itemID), out var material))
                    {
                        name = material.Name;
                        return true;
                    }
                }
            }
            name = null;
            return false;
        }

        private readonly struct Candidate
        {
            public Candidate(Event_Item pickup, float distance)
            {
                Pickup = pickup;
                Distance = distance;
            }

            public Event_Item Pickup { get; }
            public float Distance { get; }
        }

        public readonly struct Marker
        {
            public Marker(string name, Vector2 position)
            {
                Name = name;
                Position = position;
            }

            public string Name { get; }
            public Vector2 Position { get; }
        }
    }
}
