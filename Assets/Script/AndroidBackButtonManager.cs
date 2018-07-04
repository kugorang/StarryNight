using System.Collections;
using UnityEngine;

namespace Script
{
    public class AndroidBackButtonManager : MonoBehaviour
    {
        private static AndroidBackButtonManager _instance;
        private bool _isPaused;

        public static AndroidBackButtonManager GetInstance()
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

        private void Awake()
        {
            DontDestroyOnLoad(this);
        }

        private void Update()
        {
            if (!Input.GetKey(KeyCode.Escape)) 
                return;
            
            if (!_isPaused)
            {
                // if game is not yet paused, ESC will pause it
                _isPaused = true;

                StartCoroutine(CheckTime());
            }
            else
            {
                // if game is paused and ESC is pressed, it's the second press. QUIT
                Application.Quit();
            }
        }

        private IEnumerator CheckTime()
        {
            yield return new WaitForSeconds(3.0f);

            _isPaused = false;
        }
    }
}