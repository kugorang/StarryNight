using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class CameraController : MonoBehaviour
{
    [Tooltip("지구본과 겹치지 않게 적절한 X를 주세요.")]
    public float minimumX;              // 210 이상, 지구본과 겹치치 않게.
    private float startPosX;
    private DataController dataController;
    private DialogueManager dialogueManager;

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

    private int minimumDiff;

    public static bool FocusOnItem { get; set; }

    private void Awake()
    {
        //CheckLeftScene = true;
        FocusOnItem = false;
        minimumDiff = Screen.width / 8;
        dataController = DataController.Instance;
       
    }

    private void Start()
    {
        dialogueManager = DialogueManager.Instance;
    }

    private void OnEnable()
    {
        int sign = 1;               // 오른쪽으로
        if (CheckLeftScene)         // 왼쪽으로 가야하면
        {
            sign = -1;              // 왼쪽으로
        }
        transform.position = new Vector3(540.0f * sign, transform.position.y, transform.position.z);
    }

    public GameObject mainCamera;

    private void Update()//주석풀어서확인, 슬라이드 구현 ppt 12번 참조
    {
        if (Input.GetMouseButtonDown(0) && !FocusOnItem)
        {
            startPosX = Input.mousePosition.x;
        }
        else if (Input.GetMouseButtonUp(0) && !FocusOnItem)
        {
            float posXGap = Input.mousePosition.x - startPosX;

            if (Math.Abs(posXGap) > minimumDiff)
            {
                // 오른쪽에서 왼쪽
                if (posXGap > 0 && !CheckLeftScene)
                {
                    

                    iTween.MoveTo(gameObject, iTween.Hash("x", -540.0f, "time", 0.5f, "easetype", iTween.EaseType.easeOutQuad));
                    //DetectCamera.OnCameraMove(true);
                    CheckLeftScene = true;
                   
                    foreach (GameObject target in dataController.Observers)//관찰자들에게 이벤트 메세지 송출
                    {
                        ExecuteEvents.Execute<IEventListener>(target, null, (x, y) => x.OnSlide(true, 1));
                    }

                }
                // 왼쪽에서 오른쪽
                else if (posXGap < 0 && CheckLeftScene && startPosX > minimumX)
                {
                    iTween.MoveTo(gameObject, iTween.Hash("x", 540.0f, "time", 0.5f, "easetype", iTween.EaseType.easeOutQuad));
                    //DetectCamera.OnCameraMove(false);
                    CheckLeftScene = false;

                    
                    foreach (GameObject target in dataController.Observers)//관찰자들에게 이벤트 메세지 송출
                    {
                        ExecuteEvents.Execute<IEventListener>(target, null, (x, y) => x.OnSlide(false, 1));
                    }
                }
            }
        }
    }

    public void OnClickLeftBtn()
    {
        

        iTween.MoveTo(mainCamera, iTween.Hash("x", -540.0f, "time", 0.5f, "easetype", iTween.EaseType.easeOutQuad));
        CheckLeftScene = true;
        foreach (GameObject target in dataController.Observers)//관찰자들에게 이벤트 메세지 송출
        {
            ExecuteEvents.Execute<IEventListener>(target, null, (x, y) => x.OnObjClick<CameraController>(this, 1));
        }
    }

    public void OnClickRightBtn()
    {
        

        iTween.MoveTo(mainCamera, iTween.Hash("x", 540.0f, "time", 0.5f, "easetype", iTween.EaseType.easeOutQuad));
        CheckLeftScene = false;
        foreach (GameObject target in dataController.Observers)//관찰자들에게 이벤트 메세지 송출
        {
            ExecuteEvents.Execute<IEventListener>(target, null, (x, y) => x.OnObjClick<CameraController>(this, 0));
        }
    }

    public void OnApplicationQuit()
    {
        CheckLeftScene = true;
    }
}