using System.Collections.Generic;
using TSGameDev.UI.Inventories.Dragging;
using TSGameDev.UI.Inventories;
using TSGameDev.UI.Inventories.ToolTips;
using UnityEngine;

namespace TSGameDev.Inventories.Equipment
{
    /// <summary>
    /// An slot for the players equipment.
    /// </summary>
    public class EquipmentSlotUI : MonoBehaviour, IItemHolder, IDragContainer<InventoryItem>
    {
        #region Serialized Variables

        [SerializeField] InventoryItemIcon icon = null;
        [SerializeField] EquipLocation equipLocation = EquipLocation.MainHand;
        [SerializeField] Equipment playerEquipment;

        #endregion

        #region LifeCycle Functions
        private void Awake()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            playerEquipment = player.GetComponent<Equipment>();
            playerEquipment.equipmentUpdated += RedrawUI;
        }

        private void Start()
        {
            RedrawUI();
        }

        #endregion

        #region Public Functions

        /// <summary>
        /// Function that returns a number int of the amount of the passed in item can be accepted into the slot represented by this instance.
        /// </summary>
        /// <param name="item">
        /// an item InventoryItem to see how many can be accepted into this slot.
        /// </param>
        /// <returns>
        /// a number int of the amount of said item can be accepted into this slot.
        /// </returns>
        public int MaxAcceptable(InventoryItem item)
        {
            Debug.Log("Checking Max Acceptable");
            EquipableItem equipableItem = item as EquipableItem;
            List<EquipLocation> equipableItemLocations = equipableItem.GetAllowedEquipLocations();

            if (equipableItem == null) return 0;
            if (GetItem() != null) return 0;
            foreach(EquipLocation itemLocation in equipableItemLocations)
            {
                if (itemLocation == equipLocation)
                    return 1;
            }

            return 0;
        }

        /// <summary>
        /// A function that adds the passed in item to the slot that is represented by this instance.
        /// </summary>
        /// <param name="item">
        /// The item InventoryItem to be placed in this slot.
        /// </param>
        /// <param name="number">
        /// A number Int of the amount of items to be placed in this slot.
        /// </param>
        public void AddItems(InventoryItem item, int number)
        {
            Debug.Log($"Attempting to add {item.GetDisplayName()} to player equipment");
            playerEquipment.AddItem(equipLocation, (EquipableItem)item);
        }

        /// <summary>
        /// A function that returns the item InventoryItem in the slot represented by this instance
        /// </summary>
        /// <returns>
        /// An item InventoryItem in this slot.
        /// </returns>
        public InventoryItem GetItem()
        {
            return playerEquipment.GetItemInSlot(equipLocation);
        }

        /// <summary>
        /// Function that returns the amount of items in the slot represented by this instance.
        /// </summary>
        /// <returns>
        /// a number int of the amount of items in this slot.
        /// </returns>
        public int GetNumber()
        {
            if (GetItem() != null)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Function to remove the item from the slot represented by this instance.
        /// </summary>
        /// <param name="number">
        /// A number int of the amount of items to be removed from this slot.
        /// </param>
        public void RemoveItems(int number)
        {
            playerEquipment.RemoveItem(equipLocation);
        }

        #endregion

        #region Private Functions

        /// <summary>
        /// Function that is assiend to a delegate that is called on every change to the equipment slots to update equipment slot visuals.
        /// </summary>
        void RedrawUI()
        {
            icon.SetItem(playerEquipment.GetItemInSlot(equipLocation));
        }

        #endregion
    }
}
