using UnityEngine;

public class DetectCamera : MonoBehaviour
{
    
    private Vector3 uiPos;
    private static GameObject Panel;
    private bool CheckLeftScene
    {
        get
        {
            return PlayerPrefs.GetInt("isLeftScene", 1) > 0;
        }
        set
        {

            int v = 0;

            if (value)
            {
                v = 1;
            }

            PlayerPrefs.SetInt("isLeftScene", v);
        }
    }

    // Use this for initialization
    void Start()
    {
        // uiPos = new Vector3(mainCamera.transform.position.x, 0, 0);
        if (Panel == null)
        {
            Panel = gameObject;
        }
    }
    /*
    // Update is called once per frame
    void Update()
    {
        uiPos.x = mainCamera.transform.position.x + 540;
        GetComponent<RectTransform>().localPosition = uiPos;
    }*/

        /// <summary>
        /// 카메라가 움직임였음을 알려주는 함수
        /// </summary>
        /// <param name="isLeft">왼쪽으로 움직였는자를 나타내는 부울값</param>
    public static void OnCameraMove(bool isLeft)
    {
        if (Panel == null)
        {
            Debug.LogError("Panel is null object");
            return;
        }
        if (isLeft)
        {
            iTween.MoveTo(Panel, iTween.Hash("x", -540.0f, "time", 0.5f, "easetype", iTween.EaseType.easeOutQuad));
        }
        else
        {
            iTween.MoveTo(Panel, iTween.Hash("x", 540.0f, "time", 0.5f, "easetype", iTween.EaseType.easeOutQuad));
        }
    }

    private void OnDisable()
    {
        Panel = null;
    }
}