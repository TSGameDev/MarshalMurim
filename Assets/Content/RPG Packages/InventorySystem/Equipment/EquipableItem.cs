using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace TSGameDev.Inventories.Equipment
{
    /// <summary>
    /// A list of locations that equipable items can be attached to.
    /// </summary>
    public enum EquipLocation
    {
        Helm,
        Chest,
        Cape,
        Legs,
        Feet,
        Glove,
        Shoulders,
        Necklace,
        LeftRing,
        RightRing,
        MainHand,
        OffHand,
    }

    /// <summary>
    /// A scriptable object that inherits from the base of Inventory Item providing a baseline amount of functionlaity to then add on the specific functionlaity for equipment (weapons, armour and jewlery)
    /// </summary>
    [CreateAssetMenu(menuName = ("TSGameDev/Inventory/EquipableItem"))]
    public class EquipableItem : InventoryItem
    {
        #region Serialised Variables

        [TabGroup("Tab1", "Equipment Information")]
        [Tooltip("List of all the locations this equipment can be attached to.")]
        [SerializeField]
        List<EquipLocation> allowedEquipLocation = new List<EquipLocation>
        {
            EquipLocation.MainHand
        };

        #endregion

        #region Public Functions

        /// <summary>
        /// Function that returns the list of locations this equipment can be attached to.
        /// </summary>
        /// <returns>A list of equipment locations</returns>
        public List<EquipLocation> GetAllowedEquipLocations()
        {
            return allowedEquipLocation;
        }

        #endregion

    }
}
