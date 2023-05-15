using UnityEngine;
using TMPro;
using TSGameDev.Inventories;

namespace TSGameDev.Inventories.ToolTips
{
    /// <summary>
    /// Root of the tooltip prefab to expose properties to other classes.
    /// </summary>
    public class ItemTooltip : MonoBehaviour
    {
        #region Serialized Variables

        [SerializeField] TextMeshProUGUI titleText = null;
        [SerializeField] TextMeshProUGUI bodyText = null;
        [SerializeField] TextMeshProUGUI itemMinPriceText = null;
        [SerializeField] TextMeshProUGUI itemMaxPriceText = null;
        [SerializeField] TextMeshProUGUI itemTierText = null;

        #endregion

        #region Public Functions

        public void Setup(InventoryItem item)
        {
            titleText.text = item.GetDisplayName();
            bodyText.text = item.GetDescription();
            CurrencySet itemMinPrice = item.GetMinPrice();
            CurrencySet itemMaxPrice = item.GetMaxPrice();

            itemMinPriceText.text = $"Bronze: {itemMinPrice.bronze} {System.Environment.NewLine}" +
                $"Silver: {itemMinPrice.silver} {System.Environment.NewLine}" +
                $"Gold: {itemMinPrice.gold} {System.Environment.NewLine}";

            itemMaxPriceText.text = $"Bronze: {itemMaxPrice.bronze} {System.Environment.NewLine}" +
                $"Silver: {itemMaxPrice.silver} {System.Environment.NewLine}" +
                $"Gold: {itemMaxPrice.gold} {System.Environment.NewLine}";

            itemTierText.text = $"Tier: {item.GetTier()}";
        }

        #endregion
    }
}
