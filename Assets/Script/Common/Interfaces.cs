#region

using UnityEngine.EventSystems;

#endregion

namespace Script.Common
{
    public enum ValueType
    {
        Gold
    }

    public interface IClickables : IEventSystemHandler
    {
        /// <summary>다른 구성원이 OnClick되었을 때 처리</summary>
        void OnOtherClick();
    }

    public interface IResetables : IEventSystemHandler
    {
        /// <summary>
        ///     Reset신호를 받는 것들
        /// </summary>
        void OnReset();
    }

    public interface IEventListener : IEventSystemHandler
    {
        //void OnClick(GameObject obj);
        /// <summary>
        ///     오브젝트 클릭 메시지를 받아 처리
        /// </summary>
        /// <typeparam name="T">클릭된 오브젝트의 클래스</typeparam>
        /// <param name="obj">클릭된 오브젝트</param>
        /// <param name="options">처리에 필요한 옵션(switch로 클래스에 맞게 구현)</param>
        void OnObjClick<T>(T obj, params int[] options);

        /// <summary>
        ///     슬라이드 메시지를 받아 처리합니다.
        /// </summary>
        /// <param name="isLeft">슬라이드 방향</param>
        /// <param name="sceneIndex">슬라이드 된 씬의 씬 인덱스</param>
        void OnSlide(bool isLeft, int sceneIndex);

        /// <summary>
        ///     아이템 획득 메시지를 받아처리합니다.
        /// </summary>
        /// <param name="item">획득한 아이템 정보</param>
        void OnObtain(ItemInfo item);

        /// <summary>
        ///     아이템 판매 메시지를 받아 처리합니다.
        /// </summary>
        /// <param name="item">판매된 아이템 정보</param>
        void OnSell(ItemInfo item);

        /// <summary>
        ///     아이템 조합 메시지를 받아 처리합니다.
        /// </summary>
        /// <param name="itemA">재료아이템A 정보</param>
        /// <param name="itemB">재료아이템B 정보</param>
        /// <param name="result">결과아이템 정보</param>
        void OnCombine(ItemInfo itemA, ItemInfo itemB, ItemInfo result);

        /// <summary>
        ///     값이 변화한 후 메시지를 받아 그것을 처리합니다.
        /// </summary>
        /// <param name="vt">변화한 값의 종류</param>
        /// <param name="args">0:변화 후 현재값 1:변화량</param>
        void OnChangeValue(ValueType vt, params float[] args);

        /// <summary>
        ///     값이 변화한 후 메시지를 받아 그것을 처리합니다.
        /// </summary>
        /// <param name="vt">변화한 값의 종류</param>
        /// <param name="args">0:변화 후 현재값 1:변화량(현재값-이후값)</param>
        void OnChangeValue(ValueType vt, params int[] args);
    }
}