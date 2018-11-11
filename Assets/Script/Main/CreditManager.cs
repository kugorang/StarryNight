#region

using UnityEngine;
using UnityEngine.Video;

#endregion

namespace Script.Main
{
    public class CreditManager : MonoBehaviour
    {
        private VideoPlayer _videoPlayer;

        private void Awake()
        {
            _videoPlayer = gameObject.GetComponent<VideoPlayer>();
            _videoPlayer.loopPointReached += EndReached;
        }

        private void EndReached(VideoPlayer vp)
        {
            gameObject.SetActive(false);
        }

        private void OnMouseDown()
        {
            gameObject.SetActive(false);
        }
    }
}