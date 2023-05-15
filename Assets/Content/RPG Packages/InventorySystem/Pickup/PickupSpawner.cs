using UnityEngine;
using TSGameDev.SavingSystem;

namespace TSGameDev.Inventories.Pickups
{
    /// <summary>
    /// Spawns pickups that should exist on first load in a level. This
    /// automatically spawns the correct prefab for a given inventory item.
    /// </summary>
    public class PickupSpawner : MonoBehaviour, ISaveable
    {
        #region Serialized Variables

        [SerializeField] InventoryItem item = null;
        [SerializeField] int number = 1;

        #endregion

        #region Life Cycle Functions

        private void Awake()
        {
            // Spawn in Awake so can be destroyed by save system after.
            SpawnPickup();
        }

        #endregion

        #region Public Functions

        /// <summary>
        /// Returns the pickup spawned by this class if it exists.
        /// </summary>
        /// <returns>Returns null if the pickup has been collected.</returns>
        public Pickup GetPickup() 
        { 
            return GetComponentInChildren<Pickup>();
        }

        /// <summary>
        /// True if the pickup was collected.
        /// </summary>
        public bool isCollected() 
        { 
            return GetPickup() == null;
        }

        #endregion

        #region Private Functions

        /// <summary>
        /// Function that collects the pickup prefab from the item variable and spawns it as a child to this spawner and this spawners location.
        /// </summary>
        private void SpawnPickup()
        {
            var spawnedPickup = item.SpawnPickup(transform.position, number);
            spawnedPickup.transform.SetParent(transform);
        }

        /// <summary>
        /// Destorys the child spawned pickup
        /// </summary>
        private void DestroyPickup()
        {
            if (GetPickup())
            {
                Destroy(GetPickup().gameObject);
            }
        }
        
        /// <summary>
        /// Function implimented by ISavable allows for this script to define saving functionlaity
        /// </summary>
        /// <returns>
        /// An object which is the spawned pickup gameobject if it is there.
        /// </returns>
        object ISaveable.CaptureState()
        {
            return isCollected();
        }

        /// <summary>
        /// Function implimented by ISavable allows for this script to define the functionlaity of loading save data.
        /// </summary>
        /// <param name="state">
        /// A bool for if this pickup was collected or not. If so then it destorys the awake spawned pickup.
        /// </param>
        void ISaveable.RestoreState(object state)
        {
            bool shouldBeCollected = (bool)state;

            if (shouldBeCollected && !isCollected())
            {
                DestroyPickup();
            }

            if (!shouldBeCollected && isCollected())
            {
                SpawnPickup();
            }
        }

        #endregion

    }
}