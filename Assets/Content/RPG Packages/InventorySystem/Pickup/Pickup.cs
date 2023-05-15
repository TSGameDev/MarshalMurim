using UnityEngine;

namespace TSGameDev.Inventories.Pickups
{
    /// <summary>
    /// To be placed at the root of a Pickup prefab. Contains the data about the
    /// pickup such as the type of item and the number.
    /// </summary>
    public class Pickup : MonoBehaviour
    {
        #region Private Variables

        InventoryItem item;
        int number = 1;

        #endregion

        #region Serialized Variables

        [SerializeField] Inventory inventory;

        #endregion

        #region Life Cycle Functions

        private void Awake()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            inventory = player.GetComponent<Inventory>();
        }

        #endregion

        #region Public Functions

        /// <summary>
        /// Set the vital data after creating the prefab.
        /// </summary>
        /// <param name="item">The type of item this prefab represents.</param>
        /// <param name="number">The number of items represented.</param>
        public void Setup(InventoryItem item, int number)
        {
            this.item = item;
            if (!item.IsStackable())
            {
                number = 1;
            }
            this.number = number;
        }

        /// <summary>
        /// function that returns the item InventoryItem in this pickup
        /// </summary>
        /// <returns>
        /// the InventoryItem that this pickup contains.
        /// </returns>
        public InventoryItem GetItem()
        {
            return item;
        }

        /// <summary>
        /// Function that returns the number of items in the pickup.
        /// </summary>
        /// <returns>
        /// a number int of the amount of items in this pickup.
        /// </returns>
        public int GetNumber()
        {
            return number;
        }

        /// <summary>
        /// Function for picking up the item and adding it to assigned inventory
        /// </summary>
        public void PickupItem()
        {
            bool foundSlot = inventory.AddToFirstEmptySlot(item, number);
            if (foundSlot)
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Function that checks if the inventory has space for the pickup item
        /// </summary>
        /// <returns>
        /// Bool for if the item can be accepted by inventory
        /// </returns>
        public bool CanBePickedUp()
        {
            return inventory.HasSpaceFor(item);
        }

        #endregion

    }
}