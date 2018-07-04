using UnityEngine;
using UnityEngine.UI;

namespace Script
{
    public class OptionManager : MonoBehaviour
    {
        public Text BgmDisplayer;
        public Text EffDisplayer;
        public Text VoiceDisplayer;
        public Button Bgm;
        public Button Effect;
        public Button Voice;
        private AudioManager _audioManager;

        private void Awake()
        {
            _audioManager = AudioManager.GetInstance();
            
            // 배경음악 On Off 버튼 이미지와 텍스트 설정
            if (_audioManager.GetBGMAlive() == 1)
            {
                Bgm.GetComponent<Image>().sprite = Resources.Load<Sprite>("optionImg/push");
                BgmDisplayer.text = "ON";
            }
            else
            {
                _audioManager.BGMOff();
                Bgm.GetComponent<Image>().sprite = Resources.Load<Sprite>("optionImg/pull");
                BgmDisplayer.text = "OFF";
            }

            // 효과음 On Off 버튼 이미지와 텍스트 설정
            if (_audioManager.GetEffAlive() == 1)
            {
                Effect.GetComponent<Image>().sprite = Resources.Load<Sprite>("optionImg/push");
                EffDisplayer.text = "ON";
            }
            else
            {
                Effect.GetComponent<Image>().sprite = Resources.Load<Sprite>("optionImg/pull");
                EffDisplayer.text = "OFF";
            }

            // 캐릭터 대사 On Off 버튼 이미지와 텍스트 설정
            if (_audioManager.GetVoiceAlive() == 1)
            {
                Voice.GetComponent<Image>().sprite = Resources.Load<Sprite>("optionImg/push");
                VoiceDisplayer.text = "ON";
            }
            else
            {
                Voice.GetComponent<Image>().sprite = Resources.Load<Sprite>("optionImg/pull");
                VoiceDisplayer.text = "OFF";
            }
        }

        // 배경음악 버튼 선택 시
        public void BGMButton()
        {
            if (_audioManager.GetBGMAlive() == 1) // 음악이 켜져 있다면
            {
                _audioManager.BGMOff();
                _audioManager.SetBGMAlive(0);
                Bgm.GetComponent<Image>().sprite = Resources.Load<Sprite>("optionImg/pull");
                BgmDisplayer.text = "OFF";
            }
            else // 음악이 꺼져 있다면
            {
                _audioManager.BGMOn();
                _audioManager.SetBGMAlive(1);
                Bgm.GetComponent<Image>().sprite = Resources.Load<Sprite>("optionImg/push");
                BgmDisplayer.text = "ON";
            }
        }

        // 효과음 버튼 선택 시
        public void EffectButton()
        {
            if (_audioManager.GetEffAlive() == 1) // 음악이 켜져 있다면
            {
                _audioManager.SetEffAlive(0);
                Effect.GetComponent<Image>().sprite = Resources.Load<Sprite>("optionImg/pull");
                EffDisplayer.text = "OFF";
            }
            else // 음악이 꺼져 있다면
            {
                _audioManager.SetEffAlive(1);
                Effect.GetComponent<Image>().sprite = Resources.Load<Sprite>("optionImg/push");
                EffDisplayer.text = "ON";
            }
        }

        // 캐릭터 대사 버튼 선택 시
        public void VoiceButton()
        {
            if (_audioManager.GetVoiceAlive() == 1) // 켜져 있다면
            {
                _audioManager.SetVoiceAlive(0);
                Voice.GetComponent<Image>().sprite = Resources.Load<Sprite>("optionImg/pull");
                VoiceDisplayer.text = "OFF";
            }
            else // 꺼져 있다면
            {
                _audioManager.SetVoiceAlive(1);
                Voice.GetComponent<Image>().sprite = Resources.Load<Sprite>("optionImg/push");
                VoiceDisplayer.text = "ON";
            }
        }
    }
}