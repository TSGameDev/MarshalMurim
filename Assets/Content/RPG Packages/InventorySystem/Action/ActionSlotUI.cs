using TSGameDev.UI.Inventories.Dragging;
using TSGameDev.UI.Inventories.ToolTips;
using TSGameDev.Inventories;
using TSGameDev.Inventories.Actions;
using UnityEngine;

namespace TSGameDev.UI.Inventories.Actions
{
    /// <summary>
    /// The UI slot for the player action bar.
    /// </summary>
    public class ActionSlotUI : MonoBehaviour, IItemHolder, IDragContainer<InventoryItem>
    {
        #region Serialized Variables

        [SerializeField] InventoryItemIcon icon = null;
        [SerializeField] int index = 0;

        #endregion

        #region Private Variables

        ActionStore store;

        #endregion

        #region Life Cycle Functions

        /// <summary>
        /// Sets up the UI updating via delegate
        /// </summary>
        private void Awake()
        {
            store = GameObject.FindGameObjectWithTag("Player").GetComponent<ActionStore>();
            store.storeUpdated += UpdateIcon;
        }

        #endregion

        #region Public Functions

        /// <summary>
        /// Function that adds passed in items to the action hotbar.
        /// </summary>
        /// <param name="item">
        /// InventoryItem to add to the action hotbar.
        /// </param>
        /// <param name="number">
        /// The int number of InventoryItems to add to action hotbar.
        /// </param>
        public void AddItems(InventoryItem item, int number)
        {
            store.AddAction(item, index, number);
        }

        /// <summary>
        /// Function that returns the item in the slot represented by this instance.
        /// </summary>
        /// <returns>
        /// InventoryItem that is in this slot.
        /// </returns>
        public InventoryItem GetItem()
        {
            return store.GetAction(index);
        }

        /// <summary>
        /// Function that returns the number of items in the action slot represented by this instance.
        /// </summary>
        /// <returns>
        /// Int number of items in the action slot.
        /// </returns>
        public int GetNumber()
        {
            return store.GetNumber(index);
        }

        /// <summary>
        /// Function that returns the number of acceptable items in the slot reletive to the item going to be put into the slot.
        /// </summary>
        /// <param name="item">
        /// Inventoryitem that is going to be put into the slot
        /// </param>
        /// <returns>
        /// a number int of the amount of type InventoryItem item that can be accepted into this slot.
        /// </returns>
        public int MaxAcceptable(InventoryItem item)
        {
            return store.MaxAcceptable(item, index);
        }

        /// <summary>
        /// Function that removes the item in the slot represented by this instance.
        /// </summary>
        /// <param name="number">
        /// a number int of the amount of items in the slot represented by this isntance that should be removed.
        /// </param>
        public void RemoveItems(int number)
        {
            store.RemoveItems(index, number);
        }

        #endregion

        #region Private Functions

        /// <summary>
        /// Function added to the update delegate that gets called every change to the action store to update the UI visuals.
        /// </summary>
        void UpdateIcon()
        {
            icon.SetItem(GetItem(), GetNumber());
        }

        #endregion
    }
}

