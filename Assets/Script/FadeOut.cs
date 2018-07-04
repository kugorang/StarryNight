using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Script
{
    public class FadeOut : MonoBehaviour
    {
        public Image Fade;
        private float _fades = 1.0f;
        private float _time;

        // Update is called once per frame
        public void Update()
        {
            _time += Time.deltaTime;

            if (_fades > 0.0f && _time >= 0.03f)
            {
                _fades -= 0.05f;
                Fade.color = new Color(255, 255, 255, _fades);
                _time = 0;
            }
            else if (_fades <= 0.0f)
            {
                // 이 곳은 다음 씬으로 넘어가거나 다음 행동할 것에 대하여 적으시면 됩니다.
                _time = 0;

                SceneManager.LoadScene("Quest");
            }
        }
    }
}