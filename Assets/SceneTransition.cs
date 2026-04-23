using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneTransition : MonoBehaviour
{
    public Image fadePanel;
    public RectTransform background;
    public float zoomAmount = 1.5f;
    public float duration = 2f;
    public string nextScene;

    public void StartTransition()
    {
        StartCoroutine(ZoomAndFade());
    }

    IEnumerator ZoomAndFade()
    {
        float elapsed = 0f;
        Vector3 originalScale = background.localScale;
        Vector3 targetScale = originalScale * zoomAmount;
        Color fadeColor = Color.black;
        fadeColor.a = 0f;
        fadePanel.color = fadeColor;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // Zoom
            background.localScale = Vector3.Lerp(originalScale, targetScale, t);

            // Fade
            fadeColor.a = Mathf.Lerp(0f, 1f, t);
            fadePanel.color = fadeColor;

            yield return null;
        }

        SceneManager.LoadScene("Lighthouse");
    }
}
