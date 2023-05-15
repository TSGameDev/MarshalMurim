using System;
using System.Collections.Generic;
using UnityEngine;
using TSGameDev.Inventories.Pickups;
using Sirenix.OdinInspector;

namespace TSGameDev.Inventories
{
    [Serializable]
    public struct CurrencySet
    {
        public CurrencySet(int newBronze = 0, int newSilver = 0, int newGold = 0)
        {
            bronze = newBronze;
            silver = newSilver;
            gold = newGold;
        }

        public int bronze;
        public int silver;
        public int gold;
    }

    /// <summary>
    /// A ScriptableObject that represents any item that can be put in an
    /// inventory.
    /// </summary>
    /// <remarks>
    /// In practice, you are likely to use a subclass such as `ActionItem` or
    /// `EquipableItem`.
    /// </remarks>
    public abstract class InventoryItem : ScriptableObject, ISerializationCallbackReceiver
    {
        #region Serialized Variables

        [TabGroup("Tab1", "General Information")]
        [HorizontalGroup("Tab1/General Information/Split", width: 0.21f)]
        [VerticalGroup("Tab1/General Information/Split/Left")]
        [Tooltip("The UI icon to represent this item in the inventory.")]
        [PreviewField(100, ObjectFieldAlignment.Left)]
        [HideLabel]
        [SerializeField] Sprite icon = null;

        [VerticalGroup("Tab1/General Information/Split/Left")]
        [Tooltip("Item Tier")]
        [LabelWidth(53)]
        [SerializeField] int itemTier = 1;

        [VerticalGroup("Tab1/General Information/Split/Right")]
        [Tooltip("Auto-generated UUID for saving/loading. Clear this field if you want to generate a new one.")]
        [LabelWidth(48)]
        [SerializeField] string itemID = null;

        [VerticalGroup("Tab1/General Information/Split/Right")]
        [Tooltip("Item name to be displayed in UI.")]
        [LabelWidth(85)]
        [SerializeField] string displayName = null;

        [VerticalGroup("Tab1/General Information/Split/Right")]
        [Tooltip("If true, multiple items of this type can be stacked in the same inventory slot.")]
        [LabelWidth(100)]
        [SerializeField] bool stackable = false;

        [VerticalGroup("Tab1/General Information/Split/Right")]
        [Tooltip("Item description to be displayed in UI.")]
        [SerializeField][TextArea] string description = null;

        [TabGroup("Tab1", "Pickup Information")]
        [Tooltip("The prefab that should be spawned when this item is dropped.")]
        [SerializeField] Pickup pickup = null;

        [TabGroup("Tab1", "Price & Shop Information")]
        [PropertyTooltip("Bool for if this item can be sold.")]
        [SerializeField] bool sellable = true;

        [TabGroup("Tab1", "Price & Shop Information")]
        [HideIf("@!sellable")]
        [PropertyTooltip("The min Buy/Sell price of the item")]
        [SerializeField] CurrencySet minItemPrice = new();

        [TabGroup("Tab1", "Price & Shop Information")]
        [HideIf("@!sellable")]
        [PropertyTooltip("The max Buy/Sell price of the item")]
        [SerializeField] CurrencySet maxItemPrice = new();

        #endregion

        static Dictionary<string, InventoryItem> itemLookupCache;

        #region Public Functions

        /// <summary>
        /// Get the inventory item instance from its UUID.
        /// </summary>
        /// <param name="itemID">
        /// String UUID that persists between game instances.
        /// </param>
        /// <returns>
        /// Inventory item instance corresponding to the ID.
        /// </returns>
        public static InventoryItem GetFromID(string itemID)
        {
            if (itemLookupCache == null)
            {
                itemLookupCache = new Dictionary<string, InventoryItem>();
                var itemList = Resources.LoadAll<InventoryItem>("");
                foreach (var item in itemList)
                {
                    if (itemLookupCache.ContainsKey(item.itemID))
                    {
                        Debug.LogError(string.Format("Looks like there's a duplicate GameDevTV.UI.InventorySystem ID for objects: {0} and {1}", itemLookupCache[item.itemID], item));
                        continue;
                    }

                    itemLookupCache[item.itemID] = item;
                }
            }

            if (itemID == null || !itemLookupCache.ContainsKey(itemID)) return null;
            return itemLookupCache[itemID];
        }
        
        /// <summary>
        /// Spawn the pickup gameobject into the world.
        /// </summary>
        /// <param name="position">Where to spawn the pickup.</param>
        /// <param name="number">How many instances of the item does the pickup represent.</param>
        /// <returns>Reference to the pickup object spawned.</returns>
        public Pickup SpawnPickup(Vector3 position, int number)
        {
            if (this.pickup == null)
                return null;

            var pickup = Instantiate(this.pickup);
            pickup.transform.position = position;
            pickup.Setup(this, number);
            return pickup;
        }

        /// <summary>
        /// Function that returns the Icon of this item
        /// </summary>
        /// <returns>
        /// a icon Sprite of this item
        /// </returns>
        public Sprite GetIcon()
        {
            return icon;
        }

        /// <summary>
        /// Fucntion that returns the ID of this item
        /// </summary>
        /// <returns>
        /// the id string of this item
        /// </returns>
        public string GetItemID()
        {
            return itemID;
        }

        /// <summary>
        /// Function that returns if this item is stackable or not.
        /// </summary>
        /// <returns>
        /// a bool for if this item is stackable.
        /// </returns>
        public bool IsStackable()
        {
            return stackable;
        }
        
        /// <summary>
        /// function that returns the display name of this item.
        /// </summary>
        /// <returns>
        /// a name string of this item.
        /// </returns>
        public string GetDisplayName()
        {
            return displayName;
        }

        /// <summary>
        /// Function that returns the description of this item
        /// </summary>
        /// <returns>
        /// a description string of this item.
        /// </returns>
        public string GetDescription()
        {
            return description;
        }

        /// <summary>
        /// Function that returns if the item is sellable.
        /// </summary>
        /// <returns>
        /// Bool value for if the item is sellable.
        /// </returns>
        public bool GetSellable()
        {
            return sellable;
        }

        /// <summary>
        /// function that returns the min price of this item.
        /// </summary>
        /// <returns>
        /// a number int of the price of this item.
        /// </returns>
        public CurrencySet GetMinPrice()
        {
            return minItemPrice;
        }

        /// <summary>
        /// function that returns the max price of this item.
        /// </summary>
        /// <returns>
        /// a number int of the price of this item.
        /// </returns>
        public CurrencySet GetMaxPrice()
        {
            return maxItemPrice;
        }

        /// <summary>
        /// function that returns the tier of this item.
        /// </summary>
        /// <returns>
        /// a number int of the tier of this item.
        /// </returns>
        public int GetTier()
        {
            return itemTier;
        }

        #endregion

        #region Private Functions

        /// <summary>
        /// Function that is called when variable changes and the script is Serialized by Unity. Checks if the ID is unique and if it needs to generate a new guild ID
        /// </summary>
        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            // Generate and save a new UUID if this is blank.
            if (string.IsNullOrWhiteSpace(itemID))
            {
                itemID = System.Guid.NewGuid().ToString();
            }
        }

        //Function require by the interface but isn't used.
        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            // Require by the ISerializationCallbackReceiver but we don't need
            // to do anything with it.
        }

        #endregion

    }

}
