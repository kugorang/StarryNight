using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FadeOut : MonoBehaviour
{
    public Image fade;
    float fades = 1.0f;
    float time = 0;

    // Update is called once per frame
    public void Update()
    {
        time += Time.deltaTime;

        if (fades > 0.0f && time >= 0.03f)
        {
            fades -= 0.05f;
            fade.color = new Color(255, 255, 255, fades);
            time = 0;
        }
        else if (fades <= 0.0f)
        {
            // 이 곳은 다음 씬으로 넘어가거나 다음 행동할 것에 대하여 적으시면 됩니다.
            time = 0;

            SceneManager.LoadScene("Quest");
        }
    }
}
