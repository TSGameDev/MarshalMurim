using TSGameDev.Inventories;

namespace TSGameDev.UI.Inventories.ToolTips
{
    /// <summary>
    /// Allows the `ItemTooltipSpawner` to display the right information.
    /// </summary>
    public interface IItemHolder
    {
        InventoryItem GetItem();
    }
}