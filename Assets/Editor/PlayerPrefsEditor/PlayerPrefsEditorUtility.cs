using Script;
using UnityEditor;
using UnityEngine;

namespace Editor.PlayerPrefsEditor
{
    public class PlayerPrefsEditorUtility : MonoBehaviour
    {
        [MenuItem("PlayerPrefs/Delete All")]
        private static void DeletePlayerPrefs()
        {
            PlayerPrefs.DeleteAll();

            var dataController = DataController.Instance;

            if (dataController != null)
            {
                var haveDic = dataController.HaveDic;
                var itemOpenList = dataController.ItemOpenList;
                var newBookList = dataController.NewBookList;
                var newItemList = dataController.NewItemList;

                if (haveDic != null)
                {
                    haveDic.Clear();
                    DataController.SaveGameData(haveDic, dataController.HaveDicPath);
                }
                
                if (itemOpenList != null)
                {
                    itemOpenList.Clear();
                    DataController.SaveGameData(itemOpenList, dataController.ItemOpenListPath);
                }

                if (newBookList != null)
                {
                    newBookList.Clear();
                    DataController.SaveGameData(newBookList, dataController.NewBookListPath);
                }

                if (newItemList != null)
                {
                    newItemList.Clear();
                    DataController.SaveGameData(newItemList, dataController.NewItemListPath);
                }
            }

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