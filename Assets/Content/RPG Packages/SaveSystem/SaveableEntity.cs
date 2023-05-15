using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace TSGameDev.SavingSystem
{
    /// <summary>
    /// To be placed on any GameObject that has ISaveable components that
    /// require saving.
    ///
    /// This class gives the GameObject a unique ID in the scene file. The ID is
    /// used for saving and restoring the state related to this GameObject. This
    /// ID can be manually override to link GameObjects between scenes (such as
    /// recurring characters, the player or a score board). Take care not to set
    /// this in a prefab unless you want to link all instances between scenes.
    /// </summary>
    [ExecuteAlways]
    public class SaveableEntity : MonoBehaviour
    {
        [Tooltip("The unique ID is automatically generated in a scene file if " +
        "left empty. Do not set in a prefab unless you want all instances to " + 
        "be linked.")]
        [SerializeField] string uniqueIdentifier = "";

        //A single instance Dictionary of all SaveableEntities to make sure the unqiueIdentifier is actually unique.
        static Dictionary<string, SaveableEntity> globalLookup = new Dictionary<string, SaveableEntity>();

        /// <summary>
        /// Returns the unique indentifier for this SaveableEntity
        /// </summary>
        /// <returns>
        /// String being the unique indentifier
        /// </returns>
        public string GetUniqueIdentifier()
        {
            return uniqueIdentifier;
        }

        /// <summary>
        /// Will capture the state of all `ISaveables` on this component and
        /// return a `System.Serializable` object that can restore this state
        /// later.
        /// </summary>
        public object CaptureState()
        {
            Dictionary<string, object> state = new Dictionary<string, object>();
            foreach (ISaveable saveable in GetComponents<ISaveable>())
            {
                state[saveable.GetType().ToString()] = saveable.CaptureState();
            }
            return state;
        }

        /// <summary>
        /// Will restore the state that was captured by `CaptureState`.
        /// </summary>
        /// <param name="state">
        /// The same object that was returned by `CaptureState`.
        /// </param>
        public void RestoreState(object state)
        {
            Dictionary<string, object> stateDict = (Dictionary<string, object>)state;
            foreach (ISaveable saveable in GetComponents<ISaveable>())
            {
                string typeString = saveable.GetType().ToString();
                if (stateDict.ContainsKey(typeString))
                {
                    saveable.RestoreState(stateDict[typeString]);
                }
            }
        }


#if UNITY_EDITOR

        //Update function running during the editor that checks to make sure the unique indentifier is unqiue and/or isn't blank.
        //Adds this instance of SaveableEntity to the global dictionary to be compared to the rest.
        private void Update() {
            if (Application.IsPlaying(gameObject)) return;
            if (string.IsNullOrEmpty(gameObject.scene.path)) return;

            SerializedObject serializedObject = new SerializedObject(this);
            SerializedProperty property = serializedObject.FindProperty("uniqueIdentifier");
            
            if (string.IsNullOrEmpty(property.stringValue) || !IsUnique(property.stringValue))
            {
                property.stringValue = System.Guid.NewGuid().ToString();
                serializedObject.ApplyModifiedProperties();
            }

            globalLookup[property.stringValue] = this;
        }

#endif

        /// <summary>
        /// Function that checks if the passed in SaveableEntity, via the unique indentifer, is unique.
        /// </summary>
        /// <param name="candidate">
        /// String unqiue refernece to a SaveableEntity
        /// </param>
        /// <returns>
        /// Bool of true if the passed in unqiue reference is unqiue and actually references an instance of SaveableEntity via the global cache. If there is no reference or it conflicts with other returns false.
        /// </returns>
        private bool IsUnique(string candidate)
        {
            if (!globalLookup.ContainsKey(candidate)) return true;

            if (globalLookup[candidate] == this) return true;

            if (globalLookup[candidate] == null)
            {
                globalLookup.Remove(candidate);
                return true;
            }

            if (globalLookup[candidate].GetUniqueIdentifier() != candidate)
            {
                globalLookup.Remove(candidate);
                return true;
            }

            return false;
        }
    }
}