using System;
using System.Reflection;
using BepInEx.Configuration;
using BepInEx.Logging;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AnomalithQuickSwitch
{
    internal sealed class SwitchOption : IDisposable
    {
        private const BindingFlags Private = BindingFlags.Instance | BindingFlags.NonPublic;
        private const string Label = "Quick Weapon Switch";
        private const string SpeedLabel = "Switch Speed";

        // Use the same native ON/OFF prefab and local parameters as the overlay settings.
        private static readonly FieldInfo Prefab = typeof(OptionItemHolder).GetField("m_SelectPrefab", Private);
        private static readonly FieldInfo SliderPrefab = typeof(OptionItemHolder).GetField("m_SliderPrefab", Private);
        private static readonly FieldInfo SliderValueText = typeof(OptionItemSlider).GetField("m_ValueText", Private);
        private static readonly FieldInfo Content = typeof(OptionItemHolder).GetField("m_OptionContentRoot", Private);
        private static readonly FieldInfo Current = typeof(OptionItemHolder).GetField("m_CurrentOptionItem", Private);
        private static readonly FieldInfo IsOpen = typeof(OptionItemHolder).GetField("m_IsOpen", Private);
        private static readonly FieldInfo Parameter = typeof(OptionItemBase).GetField("m_OptionParam", Private);
        private static readonly FieldInfo ItemName = typeof(OptionItemBase).GetField("m_ItemNameText", Private);

        private readonly ConfigEntry<bool> enabled;
        private readonly ConfigEntry<int> speed;
        private readonly ManualLogSource logger;
        private OptionMenu menu;
        private OptionItemHolder holder;
        private OptionItemSelect item;
        private OptionItemSlider slider;
        private TextMeshProUGUI name;
        private TextMeshProUGUI sliderName;
        private TextMeshProUGUI sliderValue;
        private OptionParam param;
        private int displayedValue;
        private int displayedSpeed;
        private string speedText;
        private bool failed;

        internal SwitchOption(ConfigEntry<bool> enabled, ConfigEntry<int> speed, ManualLogSource logger)
        {
            this.enabled = enabled;
            this.speed = speed;
            this.logger = logger;
        }

        internal void Refresh()
        {
            if (failed || DialogManager.Instance == null || !DialogManager.IsOpenDialog ||
                !(DialogManager.GetTopDialog() is OptionMenu currentMenu) || !currentMenu.isActiveAndEnabled)
                return;

            try
            {
                if (menu != currentMenu || item == null || slider == null || holder == null ||
                    !holder.GetOptionList().Contains(item) || !holder.GetOptionList().Contains(slider))
                {
                    Dispose();
                    if (!Attach(currentMenu))
                        return;
                }

                if (name.text != Label)
                    name.text = Label;

                if (param.m_Value != displayedValue)
                {
                    displayedValue = param.m_Value;
                    enabled.Value = displayedValue == 0;
                    logger.LogInfo($"Quick Weapon Switch set to {(enabled.Value ? "ON" : "OFF")}.");
                }
                else if (enabled.Value != (displayedValue == 0))
                {
                    displayedValue = enabled.Value ? 0 : 1;
                    param.m_Value = displayedValue;
                    item.UIUpdate();
                }

                if (sliderName.text != SpeedLabel)
                    sliderName.text = SpeedLabel;

                var sliderParam = slider.GetOptionParam();
                if (sliderParam.m_Value != displayedSpeed)
                {
                    speed.Value = sliderParam.m_Value;
                    UpdateDisplayedSpeed();
                    logger.LogInfo($"Switch Speed set to {speedText}.");
                }
                else if (speed.Value != displayedSpeed)
                {
                    UpdateDisplayedSpeed();
                    sliderParam.m_Value = displayedSpeed;
                    slider.UIUpdate();
                }
                if (sliderValue.text != speedText)
                    sliderValue.text = speedText;
            }
            catch (Exception error)
            {
                failed = true;
                Dispose();
                logger.LogWarning($"Could not add the Quick Weapon Switch settings. {error}");
            }
        }

        private bool Attach(OptionMenu currentMenu)
        {
            var holders = currentMenu.GetOptionHolderList();
            holder = holders.Find(candidate =>
                candidate != null && candidate.m_HolderGroupType == OptionGroupType.OTHERS);
            if (holder == null || holder.GetOptionList().Count == 0)
                return false;

            var prefab = Prefab?.GetValue(holder) as OptionItemSelect;
            var audioHolder = holders.Find(candidate =>
                candidate != null && candidate.m_HolderGroupType == OptionGroupType.SOUND);
            var sliderPrefab = SliderPrefab?.GetValue(audioHolder ?? holder) as OptionItemSlider;
            var content = Content?.GetValue(holder) as RectTransform;
            if (prefab == null || content == null || Parameter == null || ItemName == null ||
                Current == null || IsOpen == null || sliderPrefab == null || SliderValueText == null)
                throw new InvalidOperationException("The native Options selector or slider is unavailable.");

            menu = currentMenu;
            item = Object.Instantiate(prefab, content);
            displayedValue = enabled.Value ? 0 : 1;
            param = new OptionParam
            {
                m_Id = "LABS_QUICK_WEAPON_SWITCH",
                m_OptionGroupType = OptionGroupType.OTHERS,
                m_OptionParamType = OptionParamType.SELECT,
                m_OptionSelectType = OptionSelectType.ON_OFF,
                m_MaxValue = 1,
                m_Value = displayedValue
            };

            name = AddRow(item, param, Label);

            slider = Object.Instantiate(sliderPrefab, content);
            sliderValue = (TextMeshProUGUI)SliderValueText.GetValue(slider);
            UpdateDisplayedSpeed();
            sliderName = AddRow(slider, new OptionParam
            {
                m_Id = "LABS_WEAPON_SWITCH_SPEED",
                m_OptionGroupType = OptionGroupType.OTHERS,
                m_OptionParamType = OptionParamType.SLIDER,
                m_MinValue = QuickSwitchPlugin.MinimumSpeed,
                m_DefaultValue = QuickSwitchPlugin.DefaultSpeed,
                m_MaxValue = QuickSwitchPlugin.MaximumSpeed,
                m_Value = displayedSpeed
            }, SpeedLabel);
            sliderValue.text = speedText;
            holder.MaskUpdate();
            logger.LogInfo("Added the Quick Weapon Switch toggle and Switch Speed slider to Other Settings.");
            return true;
        }

        private void UpdateDisplayedSpeed()
        {
            displayedSpeed = speed.Value;
            speedText = $"{displayedSpeed / 10f:0.0}×";
        }

        private TextMeshProUGUI AddRow(OptionItemBase row, OptionParam parameter, string label)
        {
            var list = holder.GetOptionList();
            var name = (TextMeshProUGUI)ItemName.GetValue(row);
            Parameter.SetValue(row, parameter);
            foreach (var component in name.GetComponents<Behaviour>())
            {
                if (component.GetType().Name == "nnLocalizeDataUGUI")
                {
                    component.enabled = false;
                    Object.Destroy(component);
                }
            }
            name.text = label;
            row.SetHolder(holder);
            row.ObjectNumber = list.Count;
            row.IsInputActive = (bool)IsOpen.GetValue(holder);
            row.UIUpdate();
            list.Add(row);
            return name;
        }

        public void Dispose()
        {
            if (holder != null)
            {
                var list = holder.GetOptionList();
                var selected = Current?.GetValue(holder);
                bool removed = list.Remove(item) | list.Remove(slider);
                if (removed)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        if (list[i] != null)
                            list[i].ObjectNumber = i;
                    }
                    if (ReferenceEquals(selected, item) || ReferenceEquals(selected, slider))
                    {
                        var replacement = list.FindLast(row => row != null && row.gameObject.activeSelf);
                        Current.SetValue(holder, replacement);
                        if (replacement != null && (bool)IsOpen.GetValue(holder))
                        {
                            replacement.UseSelect();
                            if (menu != null)
                                menu.GuideUpdate(replacement.GetOptionParam());
                        }
                    }
                }
            }
            DestroyRow(item);
            DestroyRow(slider);
            if (holder != null)
                holder.MaskUpdate();
            item = null;
            slider = null;
            holder = null;
            menu = null;
        }

        private static void DestroyRow(OptionItemBase row)
        {
            if (row == null)
                return;

            row.gameObject.SetActive(false);
            Object.Destroy(row.gameObject);
        }
    }
}
