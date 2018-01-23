﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DebugResetButton : MonoBehaviour
{
    public void OnClick()
    {
        PlayerPrefs.DeleteAll();

        DataController dataController = DataController.Instance;

        if (dataController != null)
        {
            Dictionary<int, int> haveDic = dataController.haveDic;
            List<int> itemOpenList = dataController.itemOpenList;

            haveDic.Clear();
            itemOpenList.Clear();

            dataController.SaveGameData(haveDic, dataController.HaveDicPath);
            dataController.SaveGameData(itemOpenList, dataController.ItemOpenListPath);
        }

        SceneManager.LoadScene("Main");
    }
}