#region

using UnityEngine;
using UnityEngine.UI;

#endregion

namespace Script
{
    public class CartoonManager : MonoBehaviour
    {
        private static CartoonManager _instance;
        private int _nowImgNum, _nowPage, _page1ImgNum, _page2ImgNum;
        public FadeOut FadeOut;
        public Image[] Page1ImgArr, Page2ImgArr;

        public static CartoonManager GetInstance()
        {
            if (_instance != null)
                return _instance;

            _instance = FindObjectOfType<CartoonManager>();

            if (_instance != null)
                return _instance;

            var container = new GameObject("CartoonManager");

            _instance = container.AddComponent<CartoonManager>();

            return _instance;
        }

        private void Awake()
        {
            _nowImgNum = 0;
            _nowPage = 1;
            _page1ImgNum = Page1ImgArr.Length;
            _page2ImgNum = Page2ImgArr.Length;
        }

        public void OnClick()
        {
            switch (_nowPage)
            {
                case 1:
                    if (_nowImgNum == _page1ImgNum)
                    {
                        _nowImgNum = 0;
                        _nowPage++;

                        for (var i = 0; i < _page1ImgNum; i++) Page1ImgArr[i].gameObject.SetActive(false);
                    }
                    else
                    {
                        Page1ImgArr[_nowImgNum++].gameObject.SetActive(true);
                    }

                    break;
                case 2:
                    if (_nowImgNum == _page2ImgNum)
                        FadeOut.gameObject.SetActive(true);
                    else
                        Page2ImgArr[_nowImgNum++].gameObject.SetActive(true);
                    break;
            }
        }
    }
}