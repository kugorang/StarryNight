#region

using UnityEngine;

#endregion

namespace Script.Common
{
    public class AudioManager : MonoBehaviour
    {
        private static AudioManager _instance;

        // 배경음악 상태
        private int _bgmAlive;

        // 효과음 상태
        private int _effAlive;

        private AudioSource _myAudio;

        // 캐릭터 대사 상태
        private int _voiceAlive;

        [SerializeField] private AudioClip Act;
        [SerializeField] private AudioClip Click;
        [SerializeField] private AudioClip Item;
        [SerializeField] private AudioClip Mix;
        [SerializeField] private AudioClip Option;
        [SerializeField] private AudioClip QuestStar;
        [SerializeField] private AudioClip Sale;
        [SerializeField] private AudioClip[] Voice;

        public static AudioManager Instance
        {
            get
            {
                if (_instance != null)
                    return _instance;

                _instance = FindObjectOfType<AudioManager>();

                if (_instance != null)
                    return _instance;

                var container = new GameObject("AudioManager");

                _instance = container.AddComponent<AudioManager>();

                return _instance;
            }
        }

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);

            _myAudio = gameObject.GetComponent<AudioSource>();

            // 음악 설정
            _bgmAlive = PlayerPrefs.GetInt("BGM", 1);

            if (_bgmAlive == 0) BGMOff();

            _effAlive = PlayerPrefs.GetInt("Effect", 1);
            _voiceAlive = PlayerPrefs.GetInt("Voice", 1);
        }

        // alive = 1 -> ON
        // alive = 0 -> OFF

        /// <summary>
        ///     배경음악 상태 가져오기
        /// </summary>
        /// <returns></returns>
        public int GetBGMAlive()
        {
            return _bgmAlive;
        }

        /// <summary>
        ///     배경음악 상태 설정
        /// </summary>
        /// <param name="alive">ON OFF 상태</param>
        public void SetBGMAlive(int alive)
        {
            _bgmAlive = alive;
            PlayerPrefs.SetInt("BGM", _bgmAlive);
        }

        /// <summary>
        ///     효과음 상태 가져오기
        /// </summary>
        /// <returns></returns>
        public int GetEffAlive()
        {
            return _effAlive;
        }

        /// <summary>
        ///     효과음 상태 설정
        /// </summary>
        /// <param name="alive">On Off 상태</param>
        public void SetEffAlive(int alive)
        {
            _effAlive = alive;
            PlayerPrefs.SetInt("Effect", _effAlive);
        }

        /// <summary>
        ///     캐릭터 대사 상태 가져오기
        /// </summary>
        /// <returns></returns>
        public int GetVoiceAlive()
        {
            return _voiceAlive;
        }

        /// <summary>
        ///     캐릭터 대사 상태 설정
        /// </summary>
        /// <param name="alive">On Off 설정</param>
        public void SetVoiceAlive(int alive)
        {
            _voiceAlive = alive;
            PlayerPrefs.SetInt("Voice", _voiceAlive);
        }

        /// <summary>
        ///     배경음악 끄기
        /// </summary>
        public void BGMOff()
        {
            _myAudio.Stop();
        }

        /// <summary>
        ///     배경음악 켜기
        /// </summary>
        public void BGMOn()
        {
            _myAudio.Play();
        }

        /// <summary>
        ///     클릭 소리 재생
        /// </summary>
        public void ClickSound()
        {
            if (_effAlive == 1)
                _myAudio.PlayOneShot(Click);
        }

        /// <summary>
        ///     판매 소리 재생
        /// </summary>
        public void SaleSound()
        {
            if (_effAlive == 1)
                _myAudio.PlayOneShot(Sale);
        }

        /// <summary>
        ///     ACT 소리 재생
        /// </summary>
        public void ActSound()
        {
            if (_effAlive == 1)
                _myAudio.PlayOneShot(Act);
        }

        /// <summary>
        ///     설정 소리 재생
        /// </summary>
        public void OptionSound()
        {
            if (_effAlive == 1)
                _myAudio.PlayOneShot(Option);
        }

        /// <summary>
        ///     아이템 소리 재생
        /// </summary>
        public void ItemSound()
        {
            if (_effAlive == 1)
                _myAudio.PlayOneShot(Item);
        }

        /// <summary>
        ///     조합 소리 재생
        /// </summary>
        public void MixSound()
        {
            if (_effAlive == 1)
                _myAudio.PlayOneShot(Mix);
        }

        /// <summary>
        ///     퀘스트 별 소리 재생
        /// </summary>
        public void QuestStarSound()
        {
            if (_effAlive == 1)
                _myAudio.PlayOneShot(QuestStar);
        }

        /// <summary>
        ///     캐릭터 대사 재생
        /// </summary>
        public void VoiceSound()
        {
            // 캐릭터 대사 랜덤하게 재생하기
            if (_voiceAlive != 1)
                return;

            var num = Random.Range(1, 7);
            _myAudio.PlayOneShot(Voice[num]);
        }
    }
}