using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PlayerPrefsEditorUtility : MonoBehaviour
{
    [MenuItem("PlayerPrefs/Delete All")]
    static void DeletePlayerPrefs()
    {
        PlayerPrefs.DeleteAll();

        DataController dataController = DataController.Instance;

        if (dataController != null)
        {
            Dictionary<int, Dictionary<int, SerializableVector3>> haveDic = dataController.HaveDic;
            List<int> itemOpenList = dataController.itemOpenList;

            haveDic.Clear();
            itemOpenList.Clear();

            dataController.SaveGameData(haveDic, dataController.HaveDicPath);
            dataController.SaveGameData(itemOpenList, dataController.ItemOpenListPath);
        }
        
        Debug.Log("All PlayerPrefs deleted");
    }

    [MenuItem("PlayerPrefs/Save All")]
    static void SavePlayerPrefs()
    {
        PlayerPrefs.Save();
        Debug.Log("PlayerPrefs saved");
    }
}