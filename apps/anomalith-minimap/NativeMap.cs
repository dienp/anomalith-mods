using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AnomalithMinimap
{
    internal sealed class NativeMap
    {
        private const BindingFlags Fields = BindingFlags.Instance | BindingFlags.NonPublic;
        private static readonly FieldInfo Definitions = typeof(PauseMenuMap).GetField("m_MapImageSO", Fields);
        private static readonly FieldInfo MapCamera = typeof(PauseMenuMap).GetField("m_MapPLCamera", Fields);
        private static readonly FieldInfo Marker = typeof(PauseMenuMap).GetField("m_PlIcon", Fields);
        private static readonly FieldInfo MapTitle = typeof(PauseMenuMap).GetField("m_MapName", Fields);

        private readonly List<MapImageSetData> maps;
        private readonly Quaternion inverseCameraRotation;
        public TextMeshProUGUI TitleStyle { get; }
        public Image PlayerIcon { get; }

        private NativeMap(List<MapImageSetData> maps, Quaternion cameraRotation, TextMeshProUGUI title, Image icon)
        {
            this.maps = maps;
            inverseCameraRotation = Quaternion.Inverse(cameraRotation);
            TitleStyle = title;
            PlayerIcon = icon;
        }

        public static NativeMap TryRead(PauseMenuMap menu)
        {
            var catalog = Definitions?.GetValue(menu) as MapImageData;
            var camera = MapCamera?.GetValue(menu) as Camera;
            var arrowRoot = Marker?.GetValue(menu) as RectTransform;
            var title = MapTitle?.GetValue(menu) as TextMeshProUGUI;
            var icon = arrowRoot == null ? null :
                arrowRoot.GetComponentsInChildren<Image>(true).FirstOrDefault(image => image.sprite != null);
            return catalog == null || catalog.m_Data == null || camera == null ||
                title == null || title.font == null || icon == null
                ? null : new NativeMap(catalog.m_Data, camera.transform.rotation, title, icon);
        }

        public MapImageSetData Find(string mapId) =>
            maps.Find(data => data != null && data.m_MapId == mapId && data.m_CameraSize > 0);

        public Vector2 Project(MapImageSetData map, Vector3 worldPosition)
        {
            Vector3 point = inverseCameraRotation * (worldPosition - map.m_CameraPos);
            return new Vector2(point.x, point.y) / (2f * map.m_CameraSize);
        }

        public static int FindFloor(List<Transform> thresholds, float height)
        {
            int floor = 0;
            if (thresholds != null)
            {
                foreach (var threshold in thresholds)
                {
                    if (threshold == null || threshold.position.y > height)
                        break;
                    floor++;
                }
            }
            return floor;
        }
    }
}
