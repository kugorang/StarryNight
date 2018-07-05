﻿using UnityEngine;

namespace Script
{
    public class DetectCamera : MonoBehaviour
    {
        public Camera MainCamera;
        private Vector3 _uiPos;

        // Use this for initialization
        private void Start()
        {
            _uiPos = new Vector3(MainCamera.transform.position.x, 0, 0);
        }

        // Update is called once per frame
        private void Update()
        {
            _uiPos.x = MainCamera.transform.position.x;
            GetComponent<RectTransform>().localPosition = _uiPos;
        }
    }
}