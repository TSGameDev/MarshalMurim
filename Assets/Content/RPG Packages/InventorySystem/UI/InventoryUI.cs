using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TSGameDev.Inventories;

namespace TSGameDev.UI.Inventories
{
    /// <summary>
    /// To be placed on the root of the inventory UI. Handles spawning all the
    /// inventory slot prefabs.
    /// </summary>
    public class InventoryUI : MonoBehaviour
    {
        #region Serialized Variables

        // Prefab of Item slots that represent positions in a inventory
        [SerializeField] InventorySlotUI InventoryItemPrefab;

        // reference to the player inventory
        [SerializeField] Inventory playerInventory;

        #endregion

        #region Life Cycle Functions

        private void Awake() 
        {
            playerInventory.InventoryUpdated += Redraw;
        }

        private void Start()
        {
            Redraw();
        }

        #endregion

        #region Private Functions

        /// <summary>
        /// Function reposible for updating the inventory UI whenever there is a change. Simply destorys all the UI and re-instantiates the gameobjects calling Setup on each one
        /// This makes the UI update as the changes to the inventory have already happened meaning the position of an item has changed or a new item has already been added therefor will be acounted for in the UI.
        /// </summary>
        private void Redraw()
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }

            for (int i = 0; i < playerInventory.GetSize(); i++)
            {
                var itemUI = Instantiate(InventoryItemPrefab, transform);
                itemUI.Setup(playerInventory, i);
            }
        }

        #endregion

    }
}