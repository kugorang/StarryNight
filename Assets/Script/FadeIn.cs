using UnityEngine;
using UnityEngine.UI;

public class FadeIn : MonoBehaviour
{
    public Image fade;
    private float fades;
    private float time;

    // Update is called once per frame
    private void Update()
    {
        time += Time.deltaTime;

        if (fades < 1.0f && time >= 0.03f)
        {
            fades += 0.05f;
            fade.color = new Color(255, 255, 255, fades);
            time = 0;
        }
        else if (fades >= 1.0f)
        {
            // 이 곳은 다음 씬으로 넘어가거나 다음 행동할 것에 대하여 적으시면 됩니다.
            time = 0;
        }
    }
}