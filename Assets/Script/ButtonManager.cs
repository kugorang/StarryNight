using UnityEngine;
using UnityEngine.SceneManagement;

namespace Script
{
    public class ButtonManager : MonoBehaviour
    {
        private DataController _dataController;
        /*private AudioManager _audioManager;*/

        private void Start()
        {
            _dataController = DataController.Instance;
            /*_audioManager = AudioManager.GetInstance();*/
        }

        public void OnQuestBtnClick()
        {
            SceneManager.LoadScene("QuestList");

            //현재 퀘스트로 바로 이동
            var process = _dataController.QuestProcess;

            if (process > 90104) 
                return;
            
            if (!_dataController.IsTutorialEnd 
                && (_dataController.NowIndex == 300134 || _dataController.NowIndex == 300217))
                _dataController.NowIndex++;
        }

        // 망원경
        public void OnTelescopeBtnClick()
        {
            SceneManager.LoadScene("Quest");
        }
        
        // 도감 (일반 아이템)
        public void OnItemListBtnClick()
        {
            SceneManager.LoadScene("ItemList");

        }

        // 서적 (세트 아이템)
        public void OnBookListBtnClick()
        {
            if (!_dataController.IsTutorialEnd 
                && (_dataController.NowIndex == 300609 || _dataController.NowIndex == 300622))
                _dataController.NowIndex++;

            SceneManager.LoadScene("BookList");
        }

        // 메인 화면으로 돌아가는 버튼
        public void OnMainBackBtnClick()
        {
            if (!_dataController.IsTutorialEnd && _dataController.NowIndex == 300204) 
                _dataController.NowIndex++;

            SceneManager.LoadScene("Main");
        }

        /*// 양자리 퀘스트 버튼
        public void OnAriesBtnClick()
        {
            _audioManager.ActSound();
            SceneManager.LoadScene("Cartoon");
        }

        // 황소자리 퀘스트 버튼
        public void OnTaurusBtnClick()
        {
            // 퀘스트 인덱스 확인
            if (90104 >= _dataController.QuestProcess) 
                return;
            
            _audioManager.ActSound();
            SceneManager.LoadScene("Cartoon");
        }*/
    }
}