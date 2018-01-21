using UnityEngine;
using System;

public class CameraController : MonoBehaviour
{
    [Tooltip("지구본과 겹치지 않게 적절한 X를 주세요.")]
    public float minimumX;//210 이상, 지구본과 겹치치 않게.
    private float startPosX;
    private bool checkLeftScene;
    private int minimumDiff;

    public static bool FocusOnItem { get; set; }

    private void Awake()
    {
        checkLeftScene = true;
        FocusOnItem = false;
        minimumDiff = Screen.width / 8;
    }

    public GameObject mainCamera;

    private void Update()//주석풀어서확인, 슬라이드 구현 ppt 12번 참조
    {
        if (Input.GetMouseButtonDown(0) && !FocusOnItem)
        {
            startPosX = Input.mousePosition.x;
            //Debug.Log(startPosX);
        }
        else if (Input.GetMouseButtonUp(0) && !FocusOnItem)
        {
            float posXGap = Input.mousePosition.x - startPosX;

            if (Math.Abs(posXGap) > minimumDiff)
            {
                // 오른쪽에서 왼쪽
                if (posXGap > 0 && !checkLeftScene)
                {
                    iTween.MoveTo(gameObject, iTween.Hash("x", -540.0f, "time", 0.5f, "easetype", iTween.EaseType.easeOutQuad));
                    checkLeftScene = true;
                }
                // 왼쪽에서 오른쪽
                else if (posXGap < 0 && checkLeftScene && startPosX > minimumX)
                {
                    iTween.MoveTo(gameObject, iTween.Hash("x", 540.0f, "time", 0.5f, "easetype", iTween.EaseType.easeOutQuad));
                    checkLeftScene = false;
                }
            }
        }
    }

    public void OnClickLeftBtn()
    {
        iTween.MoveTo(mainCamera, iTween.Hash("x", -540.0f, "time", 0.5f, "easetype", iTween.EaseType.easeOutQuad));
    }

    public void OnClickRightBtn()
    {
        iTween.MoveTo(mainCamera, iTween.Hash("x", 540.0f, "time", 0.5f, "easetype", iTween.EaseType.easeOutQuad));
    }
}