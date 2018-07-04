using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Script
{
    public class GameStart : MonoBehaviour
    {
        public Text Loading;

        public Image Logo;
        public Button StartButton;

        public Image StartImg;

        private void Awake()
        {
            Color textColor = new Vector4(1, 1, 1, 0);
            Loading.color = textColor;
            Loading.enabled = false;
        }

        private void Start()
        {
            StartCoroutine(FadeOut());
        }

        // 로딩 완료 시 start 버튼 띄우기
        private IEnumerator FadeOut()
        {
            for (var i = 0f; i <= 1; i += 0.1f)
            {
                yield return new WaitForSeconds(0.1f);
                Color color = new Vector4(1, 1, 1, i);
                Logo.color = color;
            }

            yield return new WaitForSeconds(0.2f);
            Loading.text = "Loading...";
            Loading.color = new Vector4(1, 1, 1, 1);
            while (!DataController.Instance.LoadingFinish)
            {
                yield return new WaitForSeconds(0.2f);
                Loading.text = "Loading.";
                yield return new WaitForSeconds(0.2f);
                Loading.text = "Loading..";
                yield return new WaitForSeconds(0.2f);
                Loading.text = "Loading...";
            }

            Loading.text = "Start";
            Loading.enabled = true;
            StartCoroutine("FadeBtn");
        }

        private IEnumerator FadeBtn() // 버튼에 페이드 효과 주기
        {
            for (var i = -0.7f; i <= 0.7; i += 0.05f)
            {
                yield return new WaitForSeconds(0.1f);
                Color color = new Vector4(1, 1, 1, 1 - Mathf.Abs(i));
                StartImg.color = color;
            }

            StartCoroutine("FadeBtn"); // StopCoroutine 전까지 무한반복
        }

        // start 클릭 시 메인화면 이동
        public void Click()
        {
            StopCoroutine("FadeBtn");
            SceneManager.LoadScene("Main");
            SceneManager.LoadScene("Dialog", LoadSceneMode.Additive);
        }
    }
}