using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Script
{
    public class TextDisplayer : MonoBehaviour
    {
        public GameObject DialougeWindowHolder;

        private string _currentTypingDialgoue;
        public Text NameDisplayer, SayDisplayer;

        [Range(0.0f, 1.0f)] 
        public float TimeBetUpdateLetters;

        public bool IsTyping { get; private set; }

        private IEnumerator _typeText/*, _typeAndAddText*/;

        private void ShowDialogueHolder()
        {
            DialougeWindowHolder.SetActive(true);
        }

        public void HideDialogueHolder()
        {
            DialougeWindowHolder.SetActive(false);
        }

        // Skip and Complete Current Dialogues
        public void SkipTypingLetter()
        {
            StopCoroutine(_typeText);
            /*StopCoroutine(_typeAndAddText);*/

            IsTyping = false;

            SayDisplayer.text = _currentTypingDialgoue;
        }

        public void SetSay(string dialogue)
        {
            _currentTypingDialgoue = dialogue;

            if (TimeBetUpdateLetters <= 0f)
            {
                SayDisplayer.text = dialogue;
            }
            else
            {
                gameObject.SetActive(true);
                _typeText = TypeText(_currentTypingDialgoue);
                StartCoroutine(_typeText);
            }
        }

        public void SetSay(string speakerName, string dialogue)
        {
            NameDisplayer.text = speakerName;
            _currentTypingDialgoue = dialogue;

            if (TimeBetUpdateLetters <= 0f)
                SayDisplayer.text = dialogue;
            else
                _typeText = TypeText(_currentTypingDialgoue);
                StartCoroutine(_typeText);
        }

        // Update Text from buffer
        private IEnumerator TypeText(string texts)
        {
            IsTyping = true;

            ShowDialogueHolder();

            SayDisplayer.text = string.Empty;

            foreach (var letter in texts)
            {
                SayDisplayer.text += letter;
                yield return new WaitForSeconds(TimeBetUpdateLetters);
            }

            IsTyping = false;
        }

        /*// Add Text, not replace 
        private IEnumerator TypeAndAddText(string texts)
        {
            IsTyping = true;

            ShowDialogueHolder();

            foreach (var letter in texts)
            {
                SayDisplayer.text += letter;
                yield return new WaitForSeconds(TimeBetUpdateLetters);
            }

            IsTyping = false;
        }*/
    }
}