using Sirenix.OdinInspector;
using UnityEngine;

namespace TSGameDev.Inventories.Actions
{
    /// <summary>
    /// An inventory item that can be placed in the action bar and "Used".
    /// </summary>
    /// <remarks>
    /// This class should be used as a base. Subclasses must implement the `Use`
    /// method.
    /// </remarks>
    [CreateAssetMenu(menuName = ("TSGameDev/Inventory/Action Item"))]
    public class ActionItem : InventoryItem
    {
        [TabGroup("Tab1", "Action Information")]
        [Tooltip("Does an instance of this item get consumed every time it's used.")]
        [SerializeField] bool consumable = false;

        /// <summary>
        /// Trigger the use of this item. Override to provide functionality.
        /// </summary>
        /// <param name="user">The character that is using this action.</param>
        public virtual void Use(GameObject user)
        {
            Debug.Log("Using action: " + this);
        }

        /// <summary>
        /// Function that returns if the action item is consumable or not.
        /// </summary>
        /// <returns>Bool of if the item is consumable</returns>
        public bool isConsumable()
        {
            return consumable;
        }
    }
}
