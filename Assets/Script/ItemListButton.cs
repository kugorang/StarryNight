using UnityEngine;
using UnityEngine.SceneManagement;

public class ItemListButton : MonoBehaviour
{
    public void OnClick()
    {
        SceneManager.LoadScene("ItemList");
    }
}