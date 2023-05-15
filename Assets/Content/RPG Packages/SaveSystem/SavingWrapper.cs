using System.Collections;
using UnityEngine;

namespace TSGameDev.SavingSystem
{
    /// <summary>
    /// Componants that connects with controls to perform saving, loading and deleting of save files.
    /// </summary>
    public class SavingWrapper : MonoBehaviour
    {
        //The name of the save file
        const string defaultSaveFile = "Save";
        
        //Calls LoadLastScene on start up, loading the lasts saved scene/data
        private void Awake() 
        {
            StartCoroutine(LoadLastScene());
        }

        /// <summary>
        /// Gets the SavingSystem componant from the object this script resides on and calls its LoadLastScene function.
        /// </summary>
        private IEnumerator LoadLastScene() {
            yield return GetComponent<SavingSystem>().LoadLastScene(defaultSaveFile);
        }

        /// <summary>
        /// Function to call for performing a LoadLastScene.
        /// </summary>
        public void Load()
        {
            // If there are multiple saves, this is the locate to define what save is loaded.
            StartCoroutine(GetComponent<SavingSystem>().LoadLastScene(defaultSaveFile));
        }

        /// <summary>
        /// Function to call for performing a save.
        /// </summary>
        public void Save()
        {
            // If the project is to have multiple saves, this is the locate to define the save name via player input or auto generation.
            GetComponent<SavingSystem>().Save(defaultSaveFile);
        }

        /// <summary>
        /// Function to call for deleting a save file.
        /// </summary>
        public void Delete()
        {
            //If there are multiple saves, this is the locate to define what save file to delete.
            GetComponent<SavingSystem>().Delete(defaultSaveFile);
        }
    }
}