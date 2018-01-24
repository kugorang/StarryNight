using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour
{

    public void OnQuestBtnClick()
    {
        //현재 퀘스트로 바로 이동
        int process = DataController.Instance.QuestProcess;
        if (process <= 90104)
        {
            AudioManager.GetInstance().ActSound();
            SceneManager.LoadScene("Aries");
        }
        else if (90104 < process && process <= 90123)
        {
            AudioManager.GetInstance().ActSound();
            SceneManager.LoadScene("Taurus");
        }
        /*  else
          {
              SceneManager.LoadScene("Quest");
          }*/
    }

    // 망원경
    public void OnTelescopeBtnClick()
    {
        SceneManager.LoadScene("Quest");
    }

    // 서적 (세트 아이템)
    public void OnBookListBtnClick()
    {
        SceneManager.LoadScene("BookList");
    }

    public void OnMainBackBtnClick()
    {
        SceneManager.LoadScene("Main");
    }

    //양자리 퀘스트 버튼
    public void OnAriesBtnClick()
    {
        AudioManager.GetInstance().ActSound();
        SceneManager.LoadScene("Cartoon");
    }

    // 황소자리 퀘스트 버튼
    public void OnTaurusBtnClick()
    {
        // 퀘스트 인덱스 확인
        if (90104 < DataController.Instance.QuestProcess)
        {
            AudioManager.GetInstance().ActSound();
            SceneManager.LoadScene("Cartoon");
        }
    }
}