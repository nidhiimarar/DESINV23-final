using UnityEngine;
using UnityEngine.SceneManagement;

public class Button_Func : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("Lighthouse");
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                Application.Quit();
        #endif
    }

}
