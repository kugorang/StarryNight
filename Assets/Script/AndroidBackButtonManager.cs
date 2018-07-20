using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Script
{
    public class AndroidBackButtonManager : MonoBehaviour
    {
        private static AndroidBackButtonManager _instance;
        private bool _isPaused;
        private GameObject Panel;//주의: Panel은 Main에만 존재하므로 Main 이외에 씬에서 참조하지 못하게 막을 것.

        public static AndroidBackButtonManager Instance
        {
            get
            {
                if (_instance != null)
                    return _instance;

                _instance = FindObjectOfType<AndroidBackButtonManager>();

                if (_instance != null)
                    return _instance;

                var container = new GameObject("AndroidBackButttonManager");

                _instance = container.AddComponent<AndroidBackButtonManager>();

                return _instance;
            }
        }

        private void Awake()
        {
            DontDestroyOnLoad(this);
          
        }

        private void Update()
        {
            if (!Input.GetKey(KeyCode.Escape)) 
                return;

            if (SceneManager.GetActiveScene().name != "Main")
            {
                SceneManager.LoadScene("Main");
            }
            else
            {
                Panel = Panel ?? GameObject.Find("Overlay Canvas").transform.Find("Quit Panel").gameObject;
                if (!_isPaused)
                {
                // if game is not yet paused, ESC will pause it
               
               Panel.SetActive(true);
                // StartCoroutine(CheckTime());
                }
            else
                {
                    // if game is paused and ESC is pressed, it's the second press. QUIT
                    Debug.Log("Application.Quit does Not Work in Editor.");
                    Application.Quit();

                }

                _isPaused = Panel.activeSelf;//"아니오" 버튼에 의해 deactivate되어도 pause가 풀린 것으로 취급.
            }
        }

       /* private IEnumerator CheckTime()
        {
            yield return new WaitForSeconds(3.0f);

            _isPaused = false;
        }*/
    }
}