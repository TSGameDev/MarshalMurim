using UnityEngine;
using TSGameDev.UI.Inventories.Dragging;
using TSGameDev.Inventories;
using TSGameDev.Inventories.Dropping;

namespace TSGameDev.UI.Inventories
{
    /// <summary>
    /// Handles spawning pickups when item dropped into the world.
    /// 
    /// Must be placed on the root canvas where items can be dragged. Will be
    /// called if dropped over empty space. 
    /// </summary>
    public class InventoryDropTarget : MonoBehaviour, IDragDestination<InventoryItem>
    {
        #region Public Functions

        /// <summary>
        /// Function that calls the functionality for the dropping items.
        /// </summary>
        /// <param name="item">
        /// The itme that is to be dropped
        /// </param>
        /// <param name="number">
        /// The amount of the preivously defined item that is to be dropped.
        /// </param>
        public void AddItems(InventoryItem item, int number)
        {
            var player = GameObject.FindGameObjectWithTag("Player");

            if (player == null)
                return;

            player.GetComponent<ItemDropper>().DropItem(item, number);
        }

        /// <summary>
        /// Function that returnes a max int value. There is no limit to the amount of items that can be dropped but this function is required due to the method of dragging and dropping items.
        /// </summary>
        public int MaxAcceptable(InventoryItem item)
        {
            return int.MaxValue;
        }

        #endregion

    }
}