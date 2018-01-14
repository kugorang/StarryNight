using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {

    private static AudioManager instance;

    AudioSource myAudio;

    // 배경음악 상태
    private int bgmAlive;
    // 효과음 상태
    private int effAlive;
    //캐릭터 대사 상태
    private int voiceAlive;

    public AudioClip click;
    public AudioClip sale;
    public AudioClip act;
    public AudioClip option;
    public AudioClip item;
    public AudioClip mix;
    public AudioClip questStar;
    private AudioClip voice;

    public static AudioManager GetInstance()
    {
        if (instance == null)
        {
            instance = FindObjectOfType<AudioManager>();

            if (instance == null)
            {
                GameObject container = new GameObject("AudioManager");
                instance = container.AddComponent<AudioManager>();
            }
        }

        return instance;
    }

    
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        // 음악 설정
        bgmAlive = PlayerPrefs.GetInt("BGM", 1);
        effAlive = PlayerPrefs.GetInt("Effect", 1);
        voiceAlive = PlayerPrefs.GetInt("Voice", 1);
        myAudio = gameObject.GetComponent<AudioSource>();
    }

    // alive = 1 -> ON
    // alive = 0 -> OFF

    /// <summary>
    /// 배경음악 상태 가져오기
    /// </summary>
    /// <returns></returns>
    public int GetBGMAlive()
    {
        return bgmAlive;
    }

    /// <summary>
    /// 배경음악 상태 설정
    /// </summary>
    /// <param name="alive">ON OFF 상태</param>
    public void SetBGMAlive(int alive)
    {
        bgmAlive = alive;
        PlayerPrefs.SetInt("BGM", bgmAlive);
    }

    /// <summary>
    /// 효과음 상태 가져오기
    /// </summary>
    /// <returns></returns>
    public int GetEffAlive()
    {
        return effAlive;
    }

    /// <summary>
    /// 효과음 상태 설정
    /// </summary>
    /// <param name="alive">On Off 상태</param>
    public void SetEffAlive(int alive)
    {
        effAlive = alive;
        PlayerPrefs.SetInt("Effect", effAlive);
    }

    /// <summary>
    /// 캐릭터 대사 상태 가져오기
    /// </summary>
    /// <returns></returns>
    public int GetVoiceAlive()
    {
        return voiceAlive;
    }

    /// <summary>
    /// 캐릭터 대사 상태 설정
    /// </summary>
    /// <param name="alive">On Off 설정</param>
    public void SetVoiceAlive(int alive)
    {
        voiceAlive = alive;
        PlayerPrefs.SetInt("Voice", voiceAlive);
    }

    /// <summary>
    /// 배경음악 끄기
    /// </summary>
    public void BGMOff()
    {
        myAudio.Stop();
    }

    /// <summary>
    /// 배경음악 켜기
    /// </summary>
    public void BGMOn()
    {
        myAudio.Play();
    }

    /// <summary>
    /// 클릭 소리 재생
    /// </summary>
    public void ClickSound()
    {
        if (effAlive == 1)
        {
            myAudio.PlayOneShot(click);
        }
    }

    /// <summary>
    /// 판매 소리 재생
    /// </summary>
    public void SaleSound()
    {
        if (effAlive == 1)
        {
            myAudio.PlayOneShot(sale);
        }
    }

    /// <summary>
    /// ACT 소리 재생
    /// </summary>
    public void ActSound()
    {
        if (effAlive == 1)
        {
            myAudio.PlayOneShot(act);
        }
    }

    /// <summary>
    /// 설정 소리 재생
    /// </summary>
    public void OptionSound()
    {
        if (effAlive == 1)
        {
            myAudio.PlayOneShot(option);
        }
    }

    /// <summary>
    /// 아이템 소리 재생
    /// </summary>
    public void ItemSound()
    {
        if (effAlive == 1)
        {
            myAudio.PlayOneShot(item);
        }
    }

    /// <summary>
    /// 조합 소리 재생
    /// </summary>
    public void MixSound()
    {
        if (effAlive == 1)
        {
            myAudio.PlayOneShot(mix);
        }
    }

    /// <summary>
    /// 퀘스트 별 소리 재생
    /// </summary>
    public void QuestStarSound()
    {
        if (effAlive == 1)
        {
            myAudio.PlayOneShot(questStar);
        }
    }

    /// <summary>
    /// 캐릭터 대사 재생
    /// </summary>
    public void VoiceSound()
    {
        // 캐릭터 대사 랜덤하게 재생하기
        if (voiceAlive == 1)
        {
            int num = Random.Range(1,7);
            AudioClip voice = Resources.Load<AudioClip>("audio/voice_" + num);
            myAudio.PlayOneShot(voice);
        }
    }

}
