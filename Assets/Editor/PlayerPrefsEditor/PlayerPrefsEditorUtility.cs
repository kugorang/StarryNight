using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PlayerPrefsEditorUtility : MonoBehaviour
{
    [MenuItem("PlayerPrefs/Delete All")]
    static void DeletePlayerPrefs()
    {
        PlayerPrefs.DeleteAll();

        DataController dataController = DataController.GetInstance();

        if (dataController != null)
        {
            Dictionary<int, int> haveDic = dataController.haveDic;
            List<int> itemOpenList = dataController.itemOpenList;

            haveDic.Clear();
            itemOpenList.Clear();

            dataController.SaveGameData(haveDic, dataController.haveDicPath);
            dataController.SaveGameData(itemOpenList, dataController.itemOpenListPath);
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