using System;
using System.Reflection;
using BepInEx.Configuration;
using BepInEx.Logging;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AnomalithAutoReload
{
    internal sealed class ReloadOption : IDisposable
    {
        private const BindingFlags Private = BindingFlags.Instance | BindingFlags.NonPublic;
        private const string Label = "Auto Reload";

        // Use the same native ON/OFF prefab and local parameters as the overlay settings.
        private static readonly FieldInfo Prefab = typeof(OptionItemHolder).GetField("m_SelectPrefab", Private);
        private static readonly FieldInfo Content = typeof(OptionItemHolder).GetField("m_OptionContentRoot", Private);
        private static readonly FieldInfo Current = typeof(OptionItemHolder).GetField("m_CurrentOptionItem", Private);
        private static readonly FieldInfo IsOpen = typeof(OptionItemHolder).GetField("m_IsOpen", Private);
        private static readonly FieldInfo Parameter = typeof(OptionItemBase).GetField("m_OptionParam", Private);
        private static readonly FieldInfo ItemName = typeof(OptionItemBase).GetField("m_ItemNameText", Private);

        private readonly ConfigEntry<bool> enabled;
        private readonly ManualLogSource logger;
        private OptionMenu menu;
        private OptionItemHolder holder;
        private OptionItemSelect item;
        private TextMeshProUGUI name;
        private OptionParam param;
        private int displayedValue;
        private bool failed;

        internal ReloadOption(ConfigEntry<bool> enabled, ManualLogSource logger)
        {
            this.enabled = enabled;
            this.logger = logger;
        }

        internal void Refresh()
        {
            if (failed || DialogManager.Instance == null || !DialogManager.IsOpenDialog ||
                !(DialogManager.GetTopDialog() is OptionMenu currentMenu) || !currentMenu.isActiveAndEnabled)
                return;

            try
            {
                if (menu != currentMenu || item == null || holder == null || !holder.GetOptionList().Contains(item))
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
                    logger.LogInfo($"Auto Reload set to {(enabled.Value ? "ON" : "OFF")}.");
                }
                else if (enabled.Value != (displayedValue == 0))
                {
                    displayedValue = enabled.Value ? 0 : 1;
                    param.m_Value = displayedValue;
                    item.UIUpdate();
                }
            }
            catch (Exception error)
            {
                failed = true;
                Dispose();
                logger.LogWarning($"Could not add the Auto Reload setting. {error}");
            }
        }

        private bool Attach(OptionMenu currentMenu)
        {
            holder = currentMenu.GetOptionHolderList().Find(candidate =>
                candidate != null && candidate.m_HolderGroupType == OptionGroupType.OTHERS);
            if (holder == null || holder.GetOptionList().Count == 0)
                return false;

            var prefab = Prefab?.GetValue(holder) as OptionItemSelect;
            var content = Content?.GetValue(holder) as RectTransform;
            if (prefab == null || content == null || Parameter == null || ItemName == null ||
                Current == null || IsOpen == null)
                throw new InvalidOperationException("The native Options selector is unavailable.");

            menu = currentMenu;
            var list = holder.GetOptionList();
            item = Object.Instantiate(prefab, content);
            name = (TextMeshProUGUI)ItemName.GetValue(item);
            displayedValue = enabled.Value ? 0 : 1;
            param = new OptionParam
            {
                m_Id = "LABS_AUTO_RELOAD",
                m_OptionGroupType = OptionGroupType.OTHERS,
                m_OptionParamType = OptionParamType.SELECT,
                m_OptionSelectType = OptionSelectType.ON_OFF,
                m_MaxValue = 1,
                m_Value = displayedValue
            };

            Parameter.SetValue(item, param);
            foreach (var component in name.GetComponents<Behaviour>())
            {
                if (component.GetType().Name == "nnLocalizeDataUGUI")
                {
                    component.enabled = false;
                    Object.Destroy(component);
                }
            }
            name.text = Label;
            item.SetHolder(holder);
            item.ObjectNumber = list.Count;
            item.IsInputActive = (bool)IsOpen.GetValue(holder);
            item.UIUpdate();
            list.Add(item);
            holder.MaskUpdate();
            logger.LogInfo("Added the Auto Reload toggle to Other Settings.");
            return true;
        }

        public void Dispose()
        {
            if (item != null)
            {
                if (holder != null)
                {
                    var list = holder.GetOptionList();
                    int index = list.IndexOf(item);
                    if (index >= 0)
                    {
                        list.RemoveAt(index);
                        for (int i = index; i < list.Count; i++)
                        {
                            if (list[i] != null)
                                list[i].ObjectNumber = i;
                        }

                        if (ReferenceEquals(Current.GetValue(holder), item))
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
                        holder.MaskUpdate();
                    }
                }

                item.gameObject.SetActive(false);
                Object.Destroy(item.gameObject);
            }
            item = null;
            holder = null;
            menu = null;
        }
    }
}
