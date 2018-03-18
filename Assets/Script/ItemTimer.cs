using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class ItemTimer : MonoBehaviour
{
    public GameObject prefab;
    public Text timeDisplayer;  // 남은 시간 표시
    public Image img;
    public Button btn;
    public int index;
    float cooltime = 300.0f;    // 쿨타임 -> 타이머 쿨타임 업그레이드 추가 시 datacontroller에서 가져오는 걸로 수정 필요. itemtimer 2,3도 마찬가지
    public bool disableOnStart = false;

    private int sec, sec_1, sec_10, min;


    public Button combineButton;

    private DataController LeftTimer;
    private DialogueManager dialogueManager;
   

    private void Awake()
    {
        

        LeftTimer = DataController.Instance;
    }

    void Start()
    {
        dialogueManager = DialogueManager.Instance;
        if (img == null)
            img = gameObject.GetComponent<Image>();

        if (btn == null)
            btn = gameObject.GetComponent<Button>();
    }

    // 시간당 게이지 채우기, 남은 시간 표시
    void Update()
    {
        if (LeftTimer[index] > 0)
        {
            btn.enabled = false;
            sec = (int)LeftTimer[index] % 60;
            sec_10 = sec / 10;
            sec_1 = sec % 10;
            min = (int)LeftTimer[index] / 60;
            timeDisplayer.text = min + ":" + sec_10 + sec_1;

           
            float ratio = 1.0f - (LeftTimer[index] / cooltime);

            if (img)
                img.fillAmount = ratio;
        }
        else
        {
            timeDisplayer.text = "0:00";
            img.fillAmount = 1.0f;

            LeftTimer[index] = 0;

            if (btn)
            {
                btn.enabled = true;
            }
        }
    }

    // 쿨타임 시간 지났는지 확인
    public bool CheckCooltime()
    {
        if (LeftTimer[index] > 0)
            return false;
        else
            return true;
    }

    public void ResetCooltime()
    {
        

        foreach (GameObject target in LeftTimer.Observers)//관찰자들에게 Click 이벤트 메세지 송출
        {
            ExecuteEvents.Execute<IEventListener>(target, null, (x, y) => x.OnObjClick<ItemTimer>(this));
        }


        if (btn) // 버튼 활성화 시
        {
            
            // 세트 아이템 랜덤 생성
            int id = Random.Range(4001, 4059);

            while (id % 5 == 0)
            {
                id = Random.Range(4001, 4059);
            }

            LeftTimer.InsertNewItem(id,1); //도감에 등록만 되면 됨
            AudioManager.GetInstance().ItemSound();
            PopUpWindow.Alert("[서적] "+DataDictionary.Instance.FindItemDic[id].Name+" 획득",this);

            LeftTimer[index] = cooltime;
            btn.enabled = false;
        }
    }


}