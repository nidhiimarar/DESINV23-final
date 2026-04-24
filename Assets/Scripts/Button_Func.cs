using UnityEngine;
using UnityEngine.SceneManagement;

public class Button_Func : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("Lighthouse");
    }

    public void ToTheOcean()
    {
        FishSpawner.currentLevel++;
        SceneManager.LoadScene("Ocean");
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
