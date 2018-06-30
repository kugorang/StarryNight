using System.Collections;
using UnityEngine;

public class AndroidBackButtonManager : MonoBehaviour
{
    private static AndroidBackButtonManager instance;
    private bool isPaused;

    public static AndroidBackButtonManager GetInstance()
    {
        if (instance == null)
        {
            instance = FindObjectOfType<AndroidBackButtonManager>();

            if (instance == null)
            {
                var container = new GameObject("AndroidBackButttonManager");

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

    private IEnumerator CheckTime()
    {
        yield return new WaitForSeconds(3.0f);

        isPaused = false;
    }
}