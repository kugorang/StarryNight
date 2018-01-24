using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckQuestClear : MonoBehaviour {

    private GameObject ariesClear;
    private GameObject taurusClear;

    private void Awake()
    {
        ariesClear = GameObject.Find("Aries Clear");
        taurusClear = GameObject.Find("Taurus Clear");

        ariesClear.SetActive(false);
        taurusClear.SetActive(false);

        // 양자리 퀘스트 클리어 시 클리어 이미지 띄우기
        if (DataController.Instance.QuestProcess> 90104)
        {
            ariesClear.SetActive(true);
        }

        // 황소자리 퀘스트 클리어 시 클리어 이미지 띄우기
        if (DataController.Instance.QuestProcess> 90123)
        {
            taurusClear.SetActive(true);
        }
    }

}
