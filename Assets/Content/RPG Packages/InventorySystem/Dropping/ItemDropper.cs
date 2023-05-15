using System.Collections.Generic;
using UnityEngine;
using TSGameDev.SavingSystem;
using TSGameDev.Inventories.Pickups;

namespace TSGameDev.Inventories.Dropping
{
    /// <summary>
    /// To be placed on anything that wishes to drop pickups into the world.
    /// Tracks the drops for saving and restoring.
    /// </summary>
    public class ItemDropper : MonoBehaviour, ISaveable
    {
        #region Private Variabels

        private List<Pickup> droppedItems = new List<Pickup>();

        #endregion

        #region Public Functions

        /// <summary>
        /// Create a pickup at the current position.
        /// </summary>
        /// <param name="item">The item type for the pickup.</param>
        /// <param name="number">
        /// The number of items contained in the pickup. Only used if the item
        /// is stackable.
        /// </param>
        public void DropItem(InventoryItem item, int number)
        {
            SpawnPickup(item, GetDropLocation(), number);
        }

        /// <summary>
        /// Create a pickup at the current position.
        /// </summary>
        /// <param name="item">The item type for the pickup.</param>
        public void DropItem(InventoryItem item)
        {
            SpawnPickup(item, GetDropLocation(), 1);
        }

        /// <summary>
        /// Override to set a custom method for locating a drop.
        /// </summary>
        /// <returns>The location the drop should be spawned.</returns>
        protected virtual Vector3 GetDropLocation()
        {
            return transform.position;
        }

        /// <summary>
        /// Funnction that calls the items spawn pickup function and stores that pickup into a dropped items array.
        /// </summary>
        /// <param name="item">
        /// The item to drop
        /// </param>
        /// <param name="spawnLocation">
        /// The spawn location of the pickup gameobject
        /// </param>
        /// <param name="number">
        /// The number of items to be dropped
        /// </param>
        public void SpawnPickup(InventoryItem item, Vector3 spawnLocation, int number)
        {
            var pickup = item.SpawnPickup(spawnLocation, number);
            droppedItems.Add(pickup);
        }

        #endregion

        #region Private Functions

        //A struct that defines what DropRecord is. A saveable record of all the dropped pickup gameobjects to be saved and loaded
        [System.Serializable]
        private struct DropRecord
        {
            public string itemID;
            public SerializableVector3 position;
            public int number;
        }

        /// <summary>
        /// Function implimented by ISavable that impliments the functionaliy for saving this scripts data
        /// </summary>
        /// <returns>
        /// A object of array fo DropRecords which is an array of dropped items using Item Id, number and position which is custom wrapper for serializable vector3s.
        /// </returns>
        object ISaveable.CaptureState()
        {
            RemoveDestroyedDrops();
            var droppedItemsList = new DropRecord[droppedItems.Count];
            for (int i = 0; i < droppedItemsList.Length; i++)
            {
                droppedItemsList[i].itemID = droppedItems[i].GetItem().GetItemID();
                droppedItemsList[i].position = new SerializableVector3(droppedItems[i].transform.position);
                droppedItemsList[i].number = droppedItems[i].GetNumber();
            }
            return droppedItemsList;
        }

        /// <summary>
        /// Function implimented by ISavable that impliments the functionlaity for loading the passed in save data.
        /// </summary>
        /// <param name="state">
        /// An object that is an array of DropRecords.
        /// </param>
        void ISaveable.RestoreState(object state)
        {
            var droppedItemsList = (DropRecord[])state;
            foreach (var item in droppedItemsList)
            {
                var pickupItem = InventoryItem.GetFromID(item.itemID);
                Vector3 position = item.position.ToVector();
                int number = item.number;
                SpawnPickup(pickupItem, position, number);
            }
        }

        /// <summary>
        /// Remove any drops in the world that have subsequently been picked up.
        /// </summary>
        private void RemoveDestroyedDrops()
        {
            var newList = new List<Pickup>();
            foreach (var item in droppedItems)
            {
                if (item != null)
                {
                    newList.Add(item);
                }
            }
            droppedItems = newList;
        }

        #endregion
    }
}