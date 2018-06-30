using UnityEngine;

public class DetectCamera : MonoBehaviour
{
    public Camera mainCamera;
    private Vector3 uiPos;

    // Use this for initialization
    private void Start()
    {
        uiPos = new Vector3(mainCamera.transform.position.x, 0, 0);
    }

    // Update is called once per frame
    private void Update()
    {
        uiPos.x = mainCamera.transform.position.x;
        GetComponent<RectTransform>().localPosition = uiPos;
    }
}