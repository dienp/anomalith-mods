using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AnomalithMinimap
{
    internal sealed class MinimapView
    {
        private const float ReferenceHealthWidth = 261f;
        private const float ImageSize = 840f;
        private readonly GameObject root;
        private readonly Canvas canvas;
        private readonly RectTransform panel;
        private readonly RectTransform mapMotion;
        private readonly RawImage map;
        private readonly RectTransform marker;
        private readonly RectTransform landmarks;
        private readonly Vector3[] healthCorners = new Vector3[4];
        private readonly List<Image> pointMarkers = new List<Image>();
        private readonly List<Vector2> pointPositions = new List<Vector2>();
        private readonly TextMeshProUGUI title;
        private readonly TextMeshProUGUI floor;
        private Quaternion mapRotation;
        private float mapAngle;
        private float lastFacing = float.NaN;

        public bool IsVisible => canvas.enabled;

        public MinimapView(Transform owner, NativeMap native)
        {
            var canvasRoot = CreateRect("ANOMALITH Minimap HUD", owner);
            root = canvasRoot.gameObject;
            canvas = root.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 65;
            var scaler = root.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 1;

            panel = CreateRect("Minimap", canvasRoot);
            panel.anchorMin = panel.anchorMax = panel.pivot = Vector2.zero;
            panel.anchoredPosition = new Vector2(27, 27);
            panel.sizeDelta = Vector2.one * ReferenceHealthWidth;

            var viewport = CreateRect("Map viewport", panel);
            viewport.anchorMin = Vector2.zero;
            viewport.anchorMax = Vector2.one;
            viewport.offsetMin = Vector2.one;
            viewport.offsetMax = new Vector2(-1, -37);
            viewport.gameObject.AddComponent<RectMask2D>();
            var backing = viewport.gameObject.AddComponent<Image>();
            backing.color = new Color(0.035f, 0.07f, 0.074f, 0.48f);
            backing.raycastTarget = false;

            mapMotion = CreateRect("Map movement", viewport);
            map = CreateRect("Native floor image", mapMotion).gameObject.AddComponent<RawImage>();
            map.raycastTarget = false;
            map.rectTransform.sizeDelta = new Vector2(ImageSize, ImageSize);
            landmarks = CreateRect("Points of interest", mapMotion);

            var arrow = CreateRect("Native player arrow", viewport).gameObject.AddComponent<Image>();
            arrow.sprite = native.PlayerIcon.sprite;
            arrow.preserveAspect = true;
            arrow.raycastTarget = false;
            arrow.color = new Color(1f, 0.69f, 0.43f);
            marker = arrow.rectTransform;
            marker.sizeDelta = new Vector2(22, 24);

            var header = CreateRect("Map header", panel);
            header.anchorMin = new Vector2(0, 1);
            header.anchorMax = new Vector2(1, 1);
            header.pivot = new Vector2(0.5f, 1);
            header.offsetMin = new Vector2(1, -36);
            header.offsetMax = new Vector2(-1, -1);
            var headerBackground = header.gameObject.AddComponent<Image>();
            headerBackground.color = new Color(0.048f, 0.084f, 0.088f, 0.68f);
            headerBackground.raycastTarget = false;

            title = CreateText("Current area", header, native.TitleStyle, 22, TextAlignmentOptions.Left);
            title.color = new Color(0.91f, 0.97f, 0.94f);
            title.rectTransform.offsetMin = new Vector2(12, 0);
            floor = CreateText("Current floor", header, native.TitleStyle, 16, TextAlignmentOptions.Right);
            floor.color = new Color(0.72f, 0.85f, 0.80f);
            floor.rectTransform.offsetMax = new Vector2(-12, 0);

            AddBorderEdge(panel, new Vector2(0, 0), new Vector2(1, 0), Vector2.zero, new Vector2(0, 1));
            AddBorderEdge(panel, new Vector2(0, 1), new Vector2(1, 1), new Vector2(0, -1), Vector2.zero);
            AddBorderEdge(panel, new Vector2(0, 0), new Vector2(0, 1), Vector2.zero, new Vector2(1, 0));
            AddBorderEdge(panel, new Vector2(1, 0), new Vector2(1, 1), new Vector2(-1, 0), Vector2.zero);
            Hide();
        }

        public void MatchHealthBarWidth(Image health)
        {
            if (health == null || health.canvas == null)
                return;

            Canvas source = health.canvas.rootCanvas;
            Camera camera = source.renderMode == RenderMode.ScreenSpaceOverlay ? null : source.worldCamera;
            health.rectTransform.GetWorldCorners(healthCorners);
            float left = RectTransformUtility.WorldToScreenPoint(camera, healthCorners[0]).x;
            float right = RectTransformUtility.WorldToScreenPoint(camera, healthCorners[3]).x;
            float size = (right - left) / canvas.scaleFactor;
            if (size <= 0)
                return;
            panel.sizeDelta = Vector2.one * size;
        }

        public void SetFloor(Texture2D texture, string area, string floorName, float rotation)
        {
            map.texture = texture;
            float scale = ImageSize / Mathf.Max(texture.width, texture.height);
            map.rectTransform.sizeDelta = new Vector2(texture.width * scale, texture.height * scale);
            mapAngle = rotation;
            mapRotation = Quaternion.Euler(0, 0, rotation);
            map.rectTransform.localRotation = mapRotation;
            title.text = area;
            floor.text = floorName;
            float floorWidth = floorName.Length == 0 ? 0 : floor.GetPreferredValues().x + 10;
            title.rectTransform.offsetMax = new Vector2(-12 - floorWidth, 0);
        }

        public void UpdateLocation(Vector2 projected, float facing)
        {
            Vector2 offset = -(Vector2)(mapRotation * (projected * ImageSize));
            if ((offset - mapMotion.anchoredPosition).sqrMagnitude >= 0.25f * 0.25f)
                mapMotion.anchoredPosition = offset;
            if (float.IsNaN(lastFacing) || Mathf.Abs(Mathf.DeltaAngle(lastFacing, facing)) >= 0.5f)
            {
                lastFacing = facing;
                marker.localRotation = Quaternion.Euler(0, 0, facing);
            }
            if (!canvas.enabled)
                canvas.enabled = true;
        }

        public void SetPoints(List<NativeMapPoints.Point> points)
        {
            pointPositions.Clear();
            int visible = 0;
            foreach (var point in points)
            {
                if (Mathf.Abs(point.Position.x) > 0.5f || Mathf.Abs(point.Position.y) > 0.5f)
                    continue;
                Vector2 position = mapRotation * (point.Position * ImageSize);

                bool overlaps = false;
                foreach (var previous in pointPositions)
                {
                    if ((position - previous).sqrMagnitude >= 12f * 12f)
                        continue;
                    overlaps = true;
                    break;
                }
                if (overlaps)
                    continue;

                pointPositions.Add(position);
                if (visible == pointMarkers.Count)
                {
                    var image = CreateRect("Map point", landmarks).gameObject.AddComponent<Image>();
                    image.raycastTarget = false;
                    image.preserveAspect = true;
                    pointMarkers.Add(image);
                }
                var icon = pointMarkers[visible++];
                icon.name = point.Appearance.Name;
                icon.sprite = point.Appearance.Sprite;
                icon.color = point.Appearance.Color;
                icon.rectTransform.sizeDelta = Vector2.one * point.Appearance.Size;
                icon.rectTransform.anchoredPosition = position;
                icon.rectTransform.localRotation = point.RotatesWithMap
                    ? Quaternion.Euler(0, 0, point.Heading + mapAngle) : Quaternion.identity;
                if (!icon.gameObject.activeSelf)
                    icon.gameObject.SetActive(true);
            }
            for (int index = visible; index < pointMarkers.Count; index++)
                if (pointMarkers[index].gameObject.activeSelf)
                    pointMarkers[index].gameObject.SetActive(false);
        }

        public void Hide()
        {
            if (canvas.enabled)
                canvas.enabled = false;
        }

        public void ClearFloor()
        {
            Hide();
            map.texture = null;
        }

        public void Destroy()
        {
            ClearFloor();
            Object.Destroy(root);
        }

        private static RectTransform CreateRect(string name, Transform parent)
        {
            var rect = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(0.5f, 0.5f);
            return rect;
        }

        private static void AddBorderEdge(Transform parent, Vector2 min, Vector2 max, Vector2 low, Vector2 high)
        {
            var rect = CreateRect("Minimap border", parent);
            rect.anchorMin = min;
            rect.anchorMax = max;
            rect.offsetMin = low;
            rect.offsetMax = high;
            var edge = rect.gameObject.AddComponent<Image>();
            edge.color = new Color(0.49f, 0.67f, 0.64f, 0.52f);
            edge.raycastTarget = false;
        }

        private static TextMeshProUGUI CreateText(string name, Transform parent, TextMeshProUGUI template,
            float size, TextAlignmentOptions alignment)
        {
            var rect = CreateRect(name, parent);
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = rect.offsetMax = Vector2.zero;
            var text = rect.gameObject.AddComponent<TextMeshProUGUI>();
            text.font = template.font;
            text.fontSharedMaterial = template.fontSharedMaterial;
            text.fontSize = size;
            text.alignment = alignment;
            text.richText = false;
            text.textWrappingMode = TextWrappingModes.NoWrap;
            text.overflowMode = TextOverflowModes.Ellipsis;
            text.raycastTarget = false;
            return text;
        }
    }
}
