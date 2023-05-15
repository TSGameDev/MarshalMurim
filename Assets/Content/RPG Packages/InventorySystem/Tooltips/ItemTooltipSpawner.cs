using UnityEngine;
using TSGameDev.Inventories.ToolTips;

namespace TSGameDev.UI.Inventories.ToolTips
{
    /// <summary>
    /// To be placed on a UI slot to spawn and show the correct item tooltip.
    /// </summary>
    [RequireComponent(typeof(IItemHolder))]
    public class ItemTooltipSpawner : TooltipSpawner
    {
        #region Public Functions

        public override bool CanCreateTooltip()
        {
            var item = GetComponent<IItemHolder>().GetItem();
            if (!item) return false;

            return true;
        }

        public override void UpdateTooltip(GameObject tooltip)
        {
            var itemTooltip = tooltip.GetComponent<ItemTooltip>();
            if (!itemTooltip) return;

            var item = GetComponent<IItemHolder>().GetItem();

            itemTooltip.Setup(item);
        }

        #endregion
    }
}