using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CartoonManager : MonoBehaviour
{
    private int nowImgNum, nowPage, page1ImgNum, page2ImgNum;
    public Image[] page1ImgArr, page2ImgArr;
    public FadeOut fadeOut;

    private static CartoonManager instance;

    public static CartoonManager GetInstance()
    {
        if (instance == null)
        {
            instance = FindObjectOfType<CartoonManager>();

            if (instance == null)
            {
                GameObject container = new GameObject("CartoonManager");

                instance = container.AddComponent<CartoonManager>();
            }
        }

        return instance;
    }

    private void Awake()
    {
        nowImgNum = 0;
        nowPage = 1;
        page1ImgNum = page1ImgArr.Length;
        page2ImgNum = page2ImgArr.Length;
    }

    public void OnClick()
    {
        switch (nowPage)
        {
            case 1:
                if (nowImgNum == page1ImgNum)
                {
                    nowImgNum = 0;
                    nowPage++;

                    for (int i = 0; i < page1ImgNum; i++)
                    {
                        page1ImgArr[i].gameObject.SetActive(false);
                    }
                }
                else
                {
                    page1ImgArr[nowImgNum++].gameObject.SetActive(true);
                }
                break;
            case 2:
                if (nowImgNum == page2ImgNum)
                {
                    fadeOut.gameObject.SetActive(true);
                }
                else
                {
                    page2ImgArr[nowImgNum++].gameObject.SetActive(true);
                }
                break;
            default:
                break;
        }        
    }
}