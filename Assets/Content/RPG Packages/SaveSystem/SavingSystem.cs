using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TSGameDev.SavingSystem
{
    /// <summary>
    /// This component provides the interface to the saving system. It provides
    /// methods to save and restore a scene.
    ///
    /// This component should be created once and shared between all subsequent scenes.
    /// </summary>
    public class SavingSystem : MonoBehaviour
    {
        /// <summary>
        /// Will load the last scene that was saved and restore the state. This
        /// must be run as a coroutine.
        /// </summary>
        /// <param name="saveFile">The save file to consult for loading.</param>
        public IEnumerator LoadLastScene(string saveFile)
        {
            Dictionary<string, object> state = LoadFile(saveFile);
            int buildIndex = SceneManager.GetActiveScene().buildIndex;
            if (state.ContainsKey("lastSceneBuildIndex"))
            {
                buildIndex = (int)state["lastSceneBuildIndex"];
            }
            yield return SceneManager.LoadSceneAsync(buildIndex);
            RestoreState(state);
        }

        /// <summary>
        /// Save the current scene to the provided save file.
        /// </summary>
        public void Save(string saveFile)
        {
            Dictionary<string, object> state = LoadFile(saveFile);
            CaptureState(state);
            SaveFile(saveFile, state);
        }

        /// <summary>
        /// Delete the state in the given save file.
        /// </summary>
        public void Delete(string saveFile)
        {
            File.Delete(GetPathFromSaveFile(saveFile));
        }

        /// <summary>
        /// Loads the specified savefile. Useful if there are multiple save files.
        /// </summary>
        /// <param name="saveFile">
        /// The save file you wish to load.
        /// </param>
        private void Load(string saveFile)
        {
            RestoreState(LoadFile(saveFile));
        }

        /// <summary>
        /// Finds, Opens and Deserialises a specified save file (loads a save file).
        /// </summary>
        /// <param name="saveFile">
        /// The save file you wish to load.
        /// </param>
        /// <returns>
        /// Returns a dictionary of formate <string, object>. The string references the saveable entity key and the object is a dictionary of save formate stating each componant and their save data.
        /// </returns>
        private Dictionary<string, object> LoadFile(string saveFile)
        {
            string path = GetPathFromSaveFile(saveFile);
            if (!File.Exists(path))
            {
                return new Dictionary<string, object>();
            }
            using (FileStream stream = File.Open(path, FileMode.Open))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                return (Dictionary<string, object>)formatter.Deserialize(stream);
            }
        }

        /// <summary>
        /// Creates and Opens a file at the specified path and Serialises the passed on savedata.
        /// </summary>
        /// <param name="saveFile">
        /// The path to create the save file at.
        /// </param>
        /// <param name="state">
        /// The games save data as an object.
        /// </param>
        private void SaveFile(string saveFile, object state)
        {
            string path = GetPathFromSaveFile(saveFile);
            print("Saving to " + path);
            using (FileStream stream = File.Open(path, FileMode.Create))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, state);
            }
        }

        /// <summary>
        /// Finds all saveable entites and collects all their componant save data adding in the last scene that was active.
        /// </summary>
        /// <param name="state">
        /// A dictionary of <string, object> that will contain all the save data. String is the saveable entity key and the object is a dictionary of componant references and relivant save data.
        /// It is either empty due to there being no save data avaliable from LoadFile or contains the previous savedata to be overwritten.
        /// </param>
        private void CaptureState(Dictionary<string, object> state)
        {
            foreach (SaveableEntity saveable in FindObjectsOfType<SaveableEntity>())
            {
                state[saveable.GetUniqueIdentifier()] = saveable.CaptureState();
            }

            state["lastSceneBuildIndex"] = SceneManager.GetActiveScene().buildIndex;
        }

        /// <summary>
        /// Loads all the saved data by collecting all saveable entities, comparing their ID to the stored IDs and if there is a match, running that states RestroeState function passing in the <String, Object> dictionary.
        /// </summary>
        /// <param name="state">
        /// A dictionary of <String, Object> that contains all the saved data in the Object and all previously saved SaveableEntites IDs in the String.
        /// </param>
        private void RestoreState(Dictionary<string, object> state)
        {
            foreach (SaveableEntity saveable in FindObjectsOfType<SaveableEntity>())
            {
                string id = saveable.GetUniqueIdentifier();
                if (state.ContainsKey(id))
                {
                    saveable.RestoreState(state[id]);
                }
            }
        }

        /// <summary>
        /// Function that get the file path for the save file. Currently uses a persistentDataPath which is a special file that very very rarly get overwritten.
        /// Also adds on .sav to the file for better acknowledgment of game save files.
        /// </summary>
        /// <param name="saveFile">
        /// A string to become the name of the save file
        /// </param>
        /// <returns>
        /// A string of the file path.
        /// </returns>
        private string GetPathFromSaveFile(string saveFile)
        {
            return Path.Combine(Application.persistentDataPath, saveFile + ".sav");
        }
    }
}