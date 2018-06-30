using UnityEngine;
using UnityEngine.SceneManagement;

namespace Script
{
    // DataController에 씬매니지 기능까지 넣고 싶지 않아서 만듦
    public class DebugResetButton : MonoBehaviour 
    {
        public void OnClick()
        {
            DataController.Instance.ResetForDebug(); //핵심 로직
            SceneManager.LoadScene("Main");
        }
    }
}