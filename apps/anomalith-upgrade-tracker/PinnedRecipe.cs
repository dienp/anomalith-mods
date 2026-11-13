using System.Linq;
using Newtonsoft.Json;

namespace AnomalithUpgradeTracker
{
    [JsonObject(MemberSerialization.Fields)]
    internal sealed class PinnedRecipe
    {
        public const string ReadyToBuild = "Ready to build at the Workshop";
        public const string Built = "Built";
        public Kind kind;
        public string recipeId;
        public string weaponId;
        public int recipeLevel;
        public string name;
        public Material[] materials;

        public string DisplayName
        {
            get
            {
                if (kind != Kind.Reinforcement || string.IsNullOrEmpty(weaponId))
                    return name;
                string weaponName = Inventory.GetItemName(weaponId);
                return string.IsNullOrWhiteSpace(weaponName) || weaponName == weaponId || weaponName == name
                    ? name : $"{weaponName} · {name}";
            }
        }

        public bool Matches(DevelopmetData recipe, Kind selectionKind, bool exactStep)
        {
            return kind != Kind.Unknown && kind == selectionKind && recipe != null &&
                !string.IsNullOrEmpty(recipeId) && recipeId == recipe.m_ItemId &&
                weaponId == (recipe as WeaponParamReinforceData)?.m_WeaponId &&
                (!exactStep || recipeLevel == recipe.GetDevLevel());
        }

        public bool IsValid =>
            !string.IsNullOrWhiteSpace(name) &&
            materials != null && materials.Length > 0 &&
            materials.All(material => material != null &&
                !string.IsNullOrWhiteSpace(material.id) && material.required > 0);

        public static PinnedRecipe ReadSelection(DevelopItemSelector selector, Kind kind)
        {
            var recipe = selector == null ? null : selector.GetDevData();
            if (!CanPin(recipe))
                return null;

            var required = recipe.GetLevelNeedMatData();
            if (required == null || required.Count == 0)
                return null;

            var pin = new PinnedRecipe
            {
                kind = kind,
                recipeId = recipe.m_ItemId,
                weaponId = (recipe as WeaponParamReinforceData)?.m_WeaponId,
                recipeLevel = recipe.GetDevLevel(),
                name = selector.GetItemName(),
                materials = required.Where(material => material != null &&
                        !string.IsNullOrWhiteSpace(material.m_MatId) && material.m_Num > 0)
                    .GroupBy(material => material.m_MatId)
                    .Select(group => new Material
                    {
                        id = group.Key,
                        required = group.Sum(material => material.m_Num)
                    })
                    .ToArray()
            };

            return pin.IsValid ? pin : null;
        }

        public static bool CanPin(DevelopmetData recipe)
        {
            return recipe != null && !recipe.IsSecletInfo() && !recipe.IsMaxDevLevel() &&
                recipe.GetLevelNeedMatData()?.Any(material => material != null &&
                    !string.IsNullOrWhiteSpace(material.m_MatId) && material.m_Num > 0) == true;
        }

        public string GetCompletionStatus(Inventory inventory, bool materialsReady)
        {
            if (kind == Kind.Unknown || string.IsNullOrEmpty(recipeId))
                return materialsReady ? "Materials ready. Pin again at the Workshop." : "";

            var catalog = GetCatalog();
            var recipe = catalog == null ? null : catalog.GetData(recipeId);
            if (recipe == null)
                return materialsReady ? "Materials ready. Check the Workshop." : "";

            int currentLevel = recipe.GetDevLevel();
            if (currentLevel > recipeLevel ||
                ((kind == Kind.Weapon || kind == Kind.Attachment) && catalog.IsDeveloped(recipeId)))
                return Built;

            if (currentLevel != recipeLevel || recipe.IsMaxDevLevel())
                return "Recipe changed. Pin again at the Workshop.";

            if (!materialsReady)
                return "";

            if (recipe.IsDevelopment())
                return ReadyToBuild;

            int missingMoney = recipe.GetLevelNeedMoney() - inventory.GetMoney;
            if (missingMoney > 0)
                return "Materials ready. Need more OV.";
            return "Materials ready. Check Workshop requirements.";
        }

        public (DialogType Menu, int Index) GetWorkshopCategory()
        {
            if (kind == Kind.Reinforcement && !string.IsNullOrEmpty(weaponId))
                return (DialogType.WeaonDevMenu, (int)(WeaponDevDatas.instance?.GetWeaponType(weaponId) ?? WeaponType.NONE));
            var recipe = string.IsNullOrEmpty(recipeId) ? null : GetCatalog()?.GetData(recipeId);
            switch (recipe)
            {
                case ArmorDevData armor when armor.m_ArmorType != ArmorType.NONE:
                    return (DialogType.ArmorDevMenu, (int)armor.m_ArmorType);
                case WeaponDevData weapon:
                    return (DialogType.WeaonDevMenu, (int)weapon.m_WeaponType);
                case AttachmentDevData attachment:
                    return (DialogType.AttachmentDevMenu, (int)attachment.m_AttachmentType);
                default:
                    return (default, -1);
            }
        }

        private DevelopmentDatasBace GetCatalog()
        {
            switch (kind)
            {
                case Kind.Weapon:
                    return WeaponDevDatas.instance;
                case Kind.Armor:
                    return ArmorDevDatas.instance;
                case Kind.Attachment:
                    return AttachmentDevDatas.instance;
                case Kind.Reinforcement:
                    var reinforcement = WeaponReinforceDatas.instance;
                    return reinforcement == null || string.IsNullOrEmpty(weaponId)
                        ? null : reinforcement.GetWeaponReinforceData(weaponId);
                default:
                    return null;
            }
        }

        public enum Kind
        {
            Unknown = 0,
            Weapon = 1,
            Armor = 2,
            Attachment = 3,
            Reinforcement = 4
        }

        internal sealed class Material
        {
            public string id;
            public int required;
        }
    }
}
