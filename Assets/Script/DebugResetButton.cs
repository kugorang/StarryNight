using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DebugResetButton : MonoBehaviour
{
    public void OnClick()
    {
     
    
        DataController.Instance.ResetForDebug();
        SceneManager.LoadScene("Main");
    }
}