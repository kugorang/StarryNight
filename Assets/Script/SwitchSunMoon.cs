using UnityEngine;
using UnityEngine.UI;

public class SwitchSunMoon : MonoBehaviour
{
    public Button SunMoonbtn;
    public int State { get; set; }
    public Sprite sun, moon;
    private DataController dataController;
    public DialogueManager dialogueManager;

    private static SwitchSunMoon instance;

    public static SwitchSunMoon GetInstance()
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

    void Start()
    {
        if (SunMoonbtn == null)
        {
            SunMoonbtn = gameObject.GetComponent<Button>();
        }

        dataController = DataController.Instance;
    }

    void Update()
    {
        GetComponent<Image>().sprite = State == 1 ? sun : moon;
    }

    // 버튼 스위치
    public void CheckButton()
    {
        if (dataController.IsTutorialEnd == 0 && dataController.NowIndex == 300212)
        {
            dialogueManager.ContinueDialogue();
        }

        // 1이 sun, 2가 moon
        State = State == 1 ? 0 : 1;
    }
}
