﻿using System.Collections.Generic;
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
            Dictionary<int, Dictionary<int, SerializableVector3>> haveDic = dataController.HaveDic;
            List<int> itemOpenList = dataController.itemOpenList;

            haveDic.Clear();
            itemOpenList.Clear();

            dataController.SaveGameData(haveDic, dataController.HaveDicPath);
            dataController.SaveGameData(itemOpenList, dataController.ItemOpenListPath);
        }

        SceneManager.LoadScene("Main");
    }
}