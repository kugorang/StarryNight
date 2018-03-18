using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DebugResetButton : MonoBehaviour //DataController에 씬매니지 기능까지 넣고 싶지 않아서 만듦
{
    public void OnClick()
    {
        DataController.Instance.ResetForDebug();//핵심 로직
        SceneManager.LoadScene("Main");
    }
}