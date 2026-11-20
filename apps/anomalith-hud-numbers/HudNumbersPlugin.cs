using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AnomalithHudNumbers
{
    [BepInPlugin("labs.anomalith.hudnumbers", "ANOMALITH HUD Numbers", "1.0.2")]
    public sealed class HudNumbersPlugin : BaseUnityPlugin
    {
        private const BindingFlags Private = BindingFlags.Instance | BindingFlags.NonPublic;
        private static readonly FieldInfo ManaGauge = typeof(UIManager).GetField("m_MpGauge", Private);
        private static readonly FieldInfo MainGauge = typeof(HudGauge).GetField("m_MainGauge", Private);
        private ConfigEntry<bool> showNumbers;
        private HudNumbersOption option;
        private UIManager currentHud;
        private Image healthBar;
        private Image manaBar;
        private TextMeshProUGUI healthText;
        private TextMeshProUGUI manaText;
        private string healthValue;
        private string manaValue;
        private float nextRefresh;
        private readonly Vector3[] barCorners = new Vector3[4];

        private void Awake()
        {
            showNumbers = Config.Bind("General", "Enabled", true, "Show current and maximum HP and MP beside the HUD bars.");
            option = new HudNumbersOption(showNumbers, Logger);
            if (ManaGauge == null || MainGauge == null)
            {
                Logger.LogWarning("Could not find the native MP gauge in this game build.");
                enabled = false;
                return;
            }
            Logger.LogInfo("HUD Numbers loaded.");
        }

        private void LateUpdate()
        {
            option?.Refresh();
            if (Time.unscaledTime < nextRefresh)
                return;
            nextRefresh = Time.unscaledTime + 0.1f;

            var hud = UIManager.Instance;
            var player = BattleManager.Instance?.GetCurrentPlayer();
            if (!showNumbers.Value || hud == null || !hud.GetShowUI() ||
                hud.HUDCanvas == null || !hud.HUDCanvas.isActiveAndEnabled ||
                player == null || player.MyStatus?.FinalStatus == null)
            {
                Hide();
                return;
            }

            if (currentHud != hud || healthBar == null || manaBar == null || healthText == null || manaText == null)
            {
                Clear();
                var gauge = ManaGauge.GetValue(hud) as HudGauge;
                var mana = gauge == null ? null : MainGauge.GetValue(gauge) as Image;
                var style = hud.RemainingBullet;
                if (hud.Health == null || mana == null || style == null || style.font == null)
                    return;

                currentHud = hud;
                healthBar = hud.Health;
                manaBar = mana;
                healthText = CreateText("HP numbers", hud.HUDCanvas.transform, style);
                manaText = CreateText("MP numbers", hud.HUDCanvas.transform, style);
            }

            string hp = FormatValue("HP", player.GetRemaingHealth(), player.GetHealth(), true);
            string mp = FormatValue("MP", player.GetRemainMana(), player.GetMana(), false);
            if (healthValue != hp)
            {
                healthValue = hp;
                healthText.text = hp;
            }
            if (manaValue != mp)
            {
                manaValue = mp;
                manaText.text = mp;
            }
            healthText.enabled = healthBar.isActiveAndEnabled && hp.Length > 0;
            manaText.enabled = manaBar.isActiveAndEnabled && mp.Length > 0;
            healthText.color = healthBar.color;
            PositionText(healthText, healthBar);
            PositionText(manaText, manaBar);
        }

        private static string FormatValue(string label, float remaining, float maximum, bool roundUp)
        {
            if (float.IsNaN(remaining) || float.IsInfinity(remaining) ||
                float.IsNaN(maximum) || float.IsInfinity(maximum) || maximum <= 0)
                return "";

            float current = Mathf.Clamp(remaining, 0, maximum);
            int value = roundUp ? Mathf.CeilToInt(current) : Mathf.FloorToInt(current);
            return $"{label} {value:N0} / {Mathf.CeilToInt(maximum):N0}";
        }

        private static TextMeshProUGUI CreateText(string name, Transform parent, TextMeshProUGUI style)
        {
            var label = new GameObject(name, typeof(RectTransform)).AddComponent<TextMeshProUGUI>();
            var rect = label.rectTransform;
            rect.SetParent(parent, false);
            label.gameObject.layer = parent.gameObject.layer;
            rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(1, 0.5f);
            rect.sizeDelta = new Vector2(210, 28);
            label.font = style.font;
            label.fontSharedMaterial = style.fontSharedMaterial;
            label.fontSize = 20;
            label.enableAutoSizing = true;
            label.fontSizeMin = 14;
            label.fontSizeMax = 20;
            label.alignment = TextAlignmentOptions.Right;
            label.textWrappingMode = TextWrappingModes.NoWrap;
            label.richText = false;
            label.color = new Color(0.94f, 0.99f, 0.97f);
            label.outlineColor = new Color32(0, 0, 0, 230);
            label.outlineWidth = 0.2f;
            label.raycastTarget = false;
            label.enabled = false;
            return label;
        }

        private void PositionText(TextMeshProUGUI text, Image bar)
        {
            // The native gauge can mirror its image. Position in the HUD canvas
            // so the numbers remain upright on either side of that transform.
            bar.rectTransform.GetWorldCorners(barCorners);
            Transform parent = text.transform.parent;
            Vector3 first = parent.InverseTransformPoint(barCorners[0]);
            Vector3 opposite = parent.InverseTransformPoint(barCorners[2]);
            text.rectTransform.localPosition = new Vector3(
                Mathf.Min(first.x, opposite.x) - 32, (first.y + opposite.y) * 0.5f, 0);
            Color color = text.color;
            color.a = bar.canvasRenderer.GetInheritedAlpha();
            text.color = color;
        }

        private void Hide()
        {
            if (healthText != null)
                healthText.enabled = false;
            if (manaText != null)
                manaText.enabled = false;
        }

        private void Clear()
        {
            Hide();
            if (healthText != null)
                Destroy(healthText.gameObject);
            if (manaText != null)
                Destroy(manaText.gameObject);
            healthText = null;
            manaText = null;
            currentHud = null;
            healthBar = null;
            manaBar = null;
            healthValue = null;
            manaValue = null;
        }

        private void OnDestroy()
        {
            option?.Dispose();
            Clear();
        }
    }
}
