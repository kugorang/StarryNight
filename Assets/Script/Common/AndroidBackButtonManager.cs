#region

using UnityEngine;
using UnityEngine.SceneManagement;

#endregion

namespace Script.Common
{
    public class AndroidBackButtonManager : MonoBehaviour, IResetables
    {
        private static AndroidBackButtonManager _instance;
        private DataController _dataController;
        private float _deltaTime;
        private bool _isPaused;
        private GameObject _panel; //주의: _panel은 Main에만 존재하므로 Main 이외에 씬에서 참조하지 못하게 막을 것.

        public float TimeBetweenUpdate = 0.2f;

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

        public void OnReset()
        {
            _panel = null;
        }

        private void Awake()
        {
            DontDestroyOnLoad(this);
            _deltaTime = 0;
        }

        private void Start()
        {
            _dataController = DataController.Instance;
            if (!_dataController.ResetList.Contains(gameObject))
                _dataController.ResetList.Add(gameObject);
        }

        private void Update()
        {
            _deltaTime += Time.deltaTime;
            if (_deltaTime < TimeBetweenUpdate) //초당 프레임 수가 너무 높으면 뒤로가기 키를 1번만 눌렀어도 여러 번 누른 것으로 인식함. 그래서 업데이트 횟수를 줄임.
                return;
            _deltaTime = 0;
            if (SceneManager.GetActiveScene().name == "Main")
            {
                _panel = _panel ? _panel : GameObject.Find("Overlay Canvas").transform.Find("Quit Panel").gameObject;


                _isPaused = _panel.activeSelf; //"아니오" 버튼에 의해 deactivate되어도 pause가 풀린 것으로 취급.
                if (!Input.GetKey(KeyCode.Escape)) return;
                if (_isPaused)
                {
                    // if game is paused and ESC is pressed, it's the second press. QUIT
                    Debug.Log("Application.Quit does Not Work in Editor.");
                    Application.Quit();
                }
                else
                {
                    // if game is not yet paused, ESC will pause it
                    _panel.SetActive(true);
                }
            }
            else if (Input.GetKey(KeyCode.Escape)) //Main 아닌 데서 뒤로 가기
            {
                _panel = null;
                SceneManager.LoadScene("Main");
            }
            else
            {
                _panel = null; //_panel은 main에만 존재하므로 null넣어줌.
            }
        }

        private void OnDisable()
        {
            _dataController.ResetList.Remove(gameObject);
        }
    }
}