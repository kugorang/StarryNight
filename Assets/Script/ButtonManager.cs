using UnityEngine;
using UnityEngine.EventSystems;
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

            // 현재 퀘스트로 바로 이동

            if (Quest.Progress > 90104) 
                return;
            
            // 관찰자들에게 Click 이벤트 메세지 송출
            foreach (var target in _dataController.Observers) 
                ExecuteEvents.Execute<IEventListener>(target, null, (x, y) => x.OnObjClick(this));
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
    }
}