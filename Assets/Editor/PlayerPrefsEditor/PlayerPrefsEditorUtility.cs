#region

using Script.Common;
using UnityEditor;
using UnityEngine;

#endregion

namespace Editor.PlayerPrefsEditor
{
    public class PlayerPrefsEditorUtility : MonoBehaviour
    {
        [MenuItem("PlayerPrefs/Delete All")]
        private static void DeletePlayerPrefs()
        {
            var dataController = DataController.Instance;

            if (dataController != null) dataController.ResetData();

            Debug.Log("All PlayerPrefs deleted");
        }

        [MenuItem("PlayerPrefs/Save All")]
        private static void SavePlayerPrefs()
        {
            PlayerPrefs.Save();
            Debug.Log("PlayerPrefs saved");
        }
    }
}