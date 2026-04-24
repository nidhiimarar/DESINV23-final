using UnityEngine;
using UnityEngine.SceneManagement;

public class Button_Func : MonoBehaviour
{
    [SerializeField] public FishSpawner fs;
    private static int currentLevel = 1;

    public void PlayGame()
    {
        SceneManager.LoadScene("Lighthouse");
    }

    public void ToTheOcean()
    {
        fs.LoadLevel(currentLevel);
        currentLevel++;
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
