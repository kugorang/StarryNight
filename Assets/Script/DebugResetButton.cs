using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DebugResetButton : MonoBehaviour
{
    public void OnClick()
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

        SceneManager.LoadScene("Main");
    }
}