using UnityEngine;
using TSGameDev.Inventories;
using TSGameDev.UI.Inventories.Dragging;
using TSGameDev.UI.Inventories.ToolTips;

namespace TSGameDev.UI.Inventories
{
    /// <summary>
    /// Script that belongs on the Inventory slot that is spawned within the scroll context of the Inventory pannel. Inventory Slot holds a gameobject that is resonsible for showcasing the inventory icon.
    /// </summary>
    public class InventorySlotUI : MonoBehaviour, IItemHolder, IDragContainer<InventoryItem>
    {
        #region Serialized Variables

        //Reference to the child InventoryItemIcon that is set from within the prefab.
        [SerializeField] InventoryItemIcon icon = null;

        #endregion

        #region Private Variables

        //Cache of the inventory slot this UI represents.
        int index;
        //Reference to the inventory this UI represents.
        Inventory inventory;

        #endregion

        #region Public Functions

        /// <summary>
        /// Function used to set up this specific slot UI caching the inventory and index this slot represents and then set ups the child gameobject icon.
        /// </summary>
        /// <param name="inventory">
        /// The inventory this slot is to represent.
        /// </param>
        /// <param name="index">
        /// The specific inventory index this inventory slot is to represent within the inventory passed in.
        /// </param>
        public void Setup(Inventory inventory, int index)
        {
            this.inventory = inventory;
            this.index = index;
            icon.SetItem(inventory.GetItemInSlot(index), inventory.GetNumberInSlot(index));
        }

        /// <summary>
        /// Function used to check if the represented inventory can hold a specific item
        /// </summary>
        /// <param name="item">
        /// The item to check the inventory for avaliability.
        /// </param>
        /// <returns>
        /// An int either max value if the inventory can accompodate the item or 0 if it can't
        /// </returns>
        public int MaxAcceptable(InventoryItem item)
        {
            if (inventory.HasSpaceFor(item))
            {
                return int.MaxValue;
            }
            return 0;
        }

        /// <summary>
        /// Function to add an item to the slot represented by this slot UI.
        /// </summary>
        /// <param name="item">
        /// The item that is to be added to the slot represented by the UI
        /// </param>
        /// <param name="number">
        /// The number of items to be added to the slot represented by the UI
        /// </param>
        public void AddItems(InventoryItem item, int number)
        {
            inventory.AddItemToSlot(index, item, number);
        }

        /// <summary>
        /// Function to get the item in the slot that is being represented by this slot UI.
        /// </summary>
        /// <returns>
        /// An inventory item.
        /// </returns>
        public InventoryItem GetItem()
        {
            return inventory.GetItemInSlot(index);
        }

        /// <summary>
        /// Fucntion to get the amount of items in the slot that is being represented by this slot UI.
        /// </summary>
        /// <returns>
        /// Int of the amount of items in the represented slot.
        /// </returns>
        public int GetNumber()
        {
            return inventory.GetNumberInSlot(index);
        }

        /// <summary>
        /// Function to remove the item from the slot represented by this slot UI.
        /// </summary>
        /// <param name="number">
        /// The number of items to remove from the slot represented by this slot UI.
        /// </param>
        public void RemoveItems(int number)
        {
            inventory.RemoveFromSlot(index, number);
        }

        #endregion

    }
}