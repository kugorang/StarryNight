using UnityEngine;
using System;

public class CameraController : MonoBehaviour
{
    //private float startPosX;
    //private bool checkLeftScene;
    //private int minimumDiff;

    //public static bool focusOnItem { get; set; }

    //private void Awake()
    //{
    //    checkLeftScene = true;
    //    focusOnItem = false;
    //    minimumDiff = Screen.width / 8;
    //}

    public GameObject mainCamera;

    //private void Update()
    //{
    //    if (Input.GetMouseButtonDown(0) && !focusOnItem)
    //    {
    //        startPosX = Input.mousePosition.x;
    //    }
    //    else if (Input.GetMouseButtonUp(0) && !focusOnItem)
    //    {
    //        float posXGap = Input.mousePosition.x - startPosX;

    //        if (Math.Abs(posXGap) > minimumDiff)
    //        {
    //            // 오른쪽에서 왼쪽
    //            if (posXGap > 0 && !checkLeftScene)
    //            {
    //                iTween.MoveTo(gameObject, iTween.Hash("x", -540.0f, "time", 0.5f, "easetype", iTween.EaseType.easeOutQuad));
    //                checkLeftScene = true;
    //            }
    //            // 왼쪽에서 오른쪽
    //            else if (posXGap < 0 && checkLeftScene)
    //            {
    //                iTween.MoveTo(gameObject, iTween.Hash("x", 540.0f, "time", 0.5f, "easetype", iTween.EaseType.easeOutQuad));
    //                checkLeftScene = false;
    //            }
    //        }
    //    }
    //}

    public void OnClickLeftBtn()
    {
        iTween.MoveTo(mainCamera, iTween.Hash("x", -540.0f, "time", 0.5f, "easetype", iTween.EaseType.easeOutQuad));
    }

    public void OnClickRightBtn()
    {
        iTween.MoveTo(mainCamera, iTween.Hash("x", 540.0f, "time", 0.5f, "easetype", iTween.EaseType.easeOutQuad));
    }
}