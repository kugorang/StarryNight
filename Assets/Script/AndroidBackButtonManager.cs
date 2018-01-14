using UnityEngine;
using System.Collections;

public class AndroidBackButtonManager : MonoBehaviour
{
    bool isPaused = false;

    private static AndroidBackButtonManager instance;

    public static AndroidBackButtonManager GetInstance()
    {
        if (instance == null)
        {
            instance = FindObjectOfType<AndroidBackButtonManager>();

            if (instance == null)
            {
                GameObject container = new GameObject("AndroidBackButttonManager");

                instance = container.AddComponent<AndroidBackButtonManager>();
            }
        }

        return instance;
    }

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            if (!isPaused)
            {
                // if game is not yet paused, ESC will pause it
                isPaused = true;

                StartCoroutine(CheckTime());
            }
            else
            {
                // if game is paused and ESC is pressed, it's the second press. QUIT
                Application.Quit();
            }
        }
    }

    IEnumerator CheckTime()
    {
        yield return new WaitForSeconds(3.0f);

        isPaused = false;
    }
}