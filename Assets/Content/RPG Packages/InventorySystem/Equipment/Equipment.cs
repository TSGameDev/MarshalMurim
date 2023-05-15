using System;
using System.Collections.Generic;
using UnityEngine;
using TSGameDev.SavingSystem;

namespace TSGameDev.Inventories.Equipment
{
    /// <summary>
    /// Provides a store for the items equipped to a player. Items are stored by
    /// their equip locations.
    /// 
    /// This component should be placed on the GameObject tagged "Player".
    /// </summary>
    public class Equipment : MonoBehaviour, ISaveable
    {
        #region Private Variables

        Dictionary<EquipLocation, EquipableItem> equippedItems = new Dictionary<EquipLocation, EquipableItem>();

        #endregion

        #region Public Functions

        /// <summary>
        /// Broadcasts when the items in the slots are added/removed.
        /// </summary>
        public event Action equipmentUpdated;

        /// <summary>
        /// Return the item in the given equip location.
        /// </summary>
        public EquipableItem GetItemInSlot(EquipLocation equipLocation)
        {
            if (!equippedItems.ContainsKey(equipLocation))
            {
                return null;
            }

            return equippedItems[equipLocation];
        }

        /// <summary>
        /// Add an item to the given equip location. Do not attempt to equip to
        /// an incompatible slot.
        /// </summary>
        public void AddItem(EquipLocation slot, EquipableItem item)
        {
            List<EquipLocation> itemEquipLocations = item.GetAllowedEquipLocations();

            foreach(EquipLocation itemSlot in itemEquipLocations)
            {
                if(itemSlot == slot)
                {
                    Debug.Log($"added {item.GetDisplayName()} to plater equipment");
                    equippedItems.Add(slot, item);
                }
            }

            if (equipmentUpdated != null)
            {
                equipmentUpdated();
            }
        }

        /// <summary>
        /// Remove the item for the given slot.
        /// </summary>
        public void RemoveItem(EquipLocation slot)
        {
            equippedItems.Remove(slot);
            if (equipmentUpdated != null)
            {
                equipmentUpdated();
            }
        }

        #endregion

        #region Private Functions

        /// <summary>
        /// Function implimented by ISavable Interface which collects the dictionary of equipped items and saves them as an Object.
        /// The dictionary format is the equip location as a key with a string reference of the items ID as the value.
        /// </summary>
        /// <returns>
        /// Object of Dictionary<EquipLocation, String>, string being the item id.
        /// </returns>
        object ISaveable.CaptureState()
        {
            var equippedItemsForSerialization = new Dictionary<EquipLocation, string>();
            foreach (var pair in equippedItems)
            {
                equippedItemsForSerialization[pair.Key] = pair.Value.GetItemID();
            }
            return equippedItemsForSerialization;
        }

        /// <summary>
        /// Resets the equipped dictionary and takes in the object casting it to the correct dictionary format. It then loops through the dictionary and adds the items via GetFromID to the correct location.
        /// </summary>
        /// <param name="state">
        /// Object of Dictionary<Equiplocation, string>, string being item id.
        /// </param>
        void ISaveable.RestoreState(object state)
        {
            equippedItems = new Dictionary<EquipLocation, EquipableItem>();

            var equippedItemsForSerialization = (Dictionary<EquipLocation, string>)state;

            foreach (var pair in equippedItemsForSerialization)
            {
                var item = (EquipableItem)InventoryItem.GetFromID(pair.Value);
                if (item != null)
                {
                    equippedItems[pair.Key] = item;
                }
            }
        }

        #endregion
    }
}
