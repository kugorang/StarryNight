using UnityEngine;
using UnityEngine.UI;

public class SwitchSunMoon : MonoBehaviour {

    public Button SunMoonbtn;
    private bool state;
    public Sprite sun;
    public Sprite moon;

    private DataController dataController;
    public DialogueManager dialogueManager;

    private static SwitchSunMoon instance;

    public static SwitchSunMoon Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<SwitchSunMoon>();

                if (instance == null)
                {
                    GameObject container = new GameObject("SwitchSunMoon");

                    instance = container.AddComponent<SwitchSunMoon>();
                }
            }
            return instance;
        }
    }

    void Start()
    {
        if (SunMoonbtn == null)
        {
            SunMoonbtn = gameObject.GetComponent<Button>();
        }

        dataController = DataController.Instance;
    }

    
// 현재 스위치 상태 가져오기
/// <summary>
/// true이면 별, false이면 다른 재료
/// </summary>
public bool State
    {
        get
        {
            return state;
        }

        private set
        {
            if (value)
            {
                gameObject.GetComponent<Image>().sprite = sun;
            }
            else
            {
                gameObject.GetComponent<Image>().sprite = moon;
            }
            state = value;
        }
    }
    // 버튼 스위치
    public void CheckButton()
    {
        
        if (dataController.IsTutorialEnd == 0 && dataController.NowIndex == 300212)
        {
            dialogueManager.ContinueDialogue();
        }
        State = !state; // true가 sun false가 moon
    }
}
