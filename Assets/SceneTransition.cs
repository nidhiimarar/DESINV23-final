using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneTransition : MonoBehaviour
{
    public Image fadePanel;
    public RectTransform background;
    public float zoomAmount = 1.5f;
    public float duration = 1f;
    public string nextScene;
    [SerializeField] private float elapsed = 0f; // for debugging
    
    void Start()
    {
        if (SceneManager.GetActiveScene().name == "Main_Menu")
        {
            // do nothing, wait for button press
        }
        else
        {
            StartCoroutine(ZoomAndFade());
        }
    }
    public void StartTransition()
    {
        StartCoroutine(ZoomAndFade()); //lets function play over multiple frames
    }

    IEnumerator ZoomAndFade()
    {
        elapsed = 0f;
        Vector3 originalScale = background.localScale;
        Vector3 targetScale = originalScale * zoomAmount;
        Color fadeColor = Color.black;
        fadeColor.a = SceneManager.GetActiveScene().name == "Main_Menu" ? 0f : 1f;
        fadePanel.color = fadeColor;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            if (SceneManager.GetActiveScene().name == "Main_Menu") // im reusing this fade to black transition for when we enter the lighthouse but in reverse so fade out of black
            {
                // Zoom
                background.localScale = Vector3.Lerp(originalScale, targetScale, t);

                // Fade
                fadeColor.a = Mathf.Lerp(0, 1f, t);
            }
            else
            {
                fadeColor.a = Mathf.Lerp(1f, 0, t);
            }
            
            fadePanel.color = fadeColor;

            yield return null; //wait till next frame
        }

        if (SceneManager.GetActiveScene().name == "Main_Menu")
        {
            SceneManager.LoadScene("Lighthouse");
        }
        
    }
}
