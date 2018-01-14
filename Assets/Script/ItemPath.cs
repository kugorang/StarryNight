using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPath : MonoBehaviour {

    void Start()
    {
        // 아이템 생성 시 인벤토리 위치로 랜덤하게 이동
        iTween.MoveTo(this.gameObject, iTween.Hash("x", Random.Range(175, 953), "y", Random.Range(616, -227), "time", 1.0f, "easeType", iTween.EaseType.easeInExpo));
    }
}
