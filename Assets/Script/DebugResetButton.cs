using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DebugResetButton : MonoBehaviour
{
    public void OnClick()
    {
        /*PlayerPrefs.DeleteAll();

        DataController dataController = DataController.Instance;

        if (dataController != null)
        {
            Dictionary<int, Dictionary<int, SerializableVector3>> haveDic = dataController.HaveDic;
            List<int> itemOpenList = dataController.itemOpenList;

            haveDic.Clear();
            itemOpenList.Clear();
            dataController.newBookList.Clear();
            dataController.newItemList.Clear();

           for(int i=0; i<3; i++)
            {
                dataController[i] = 0;
            }


            dataController.SaveGameData(haveDic, dataController.HaveDicPath);
            dataController.SaveGameData(itemOpenList, dataController.ItemOpenListPath);
            dataController.SaveGameData(dataController.newBookList, dataController.NewBookListPath);
            dataController.SaveGameData(dataController.newItemList, dataController.NewItemListPath);
    }*/
        DataController.Instance.ResetForDebug();
        SceneManager.LoadScene("Main");
    }
}