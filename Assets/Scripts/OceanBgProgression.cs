using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;

public class OceanBgProgression : MonoBehaviour
{
    public Sprite[] backgrounds;
    private SpriteRenderer sr;
    [SerializeField] public Sprite bigBug;
    [SerializeField] private SpriteRenderer bugRenderer;
    [SerializeField] public Image blackPanel;
    [SerializeField] public TextMeshProUGUI bugText;

    void Start()
    {
        bugText.gameObject.SetActive(false);
        bugRenderer.gameObject.SetActive(false);
        sr = GetComponent<SpriteRenderer>();
        int index = Mathf.Clamp(FishSpawner.currentLevel, 0, backgrounds.Length - 1);
        sr.sprite = backgrounds[index];
    }

    public void uhOhBigBug()
    {
        bugRenderer.sprite = bigBug;
        // start fully transparent and small
        Color c = bugRenderer.color;
        c.a = 0f;
        bugRenderer.color = c;
        bugRenderer.transform.localScale = Vector3.zero;
        bugRenderer.gameObject.SetActive(true);
        StartCoroutine(FadeAndGrow(bugRenderer, 3f));
    }

    IEnumerator FadeAndGrow(SpriteRenderer spriteRenderer, float duration)
    {
        yield return new WaitForSeconds(4f); // wait after camera pans

        float elapsed = 0f;
        Vector3 startingScale = new Vector3(1.1f, 1.1f, 1f);
        Vector3 targetScale = new Vector3(2.0f, 2.0f, 1f);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // fade in
            Color c = spriteRenderer.color;
            c.a = Mathf.Lerp(0f, 0.7f, t);
            spriteRenderer.color = c;

            // grow
            spriteRenderer.transform.localScale = Vector3.Lerp(startingScale, targetScale, t);

            yield return null;
        }
        blackPanel.gameObject.SetActive(true);
        StartCoroutine(FadeInCredits(bugText, 3f));
    }
    
    IEnumerator FadeInCredits(TextMeshProUGUI bugText, float duration)
    {
        yield return new WaitForSeconds(2f); 
        Debug.Log("FadeInCredits started, bugText: " + bugText);
        Color c = bugText.color;
        c.a = 0f;
        bugText.color = c;
        bugText.gameObject.SetActive(true);
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // fade in
            c = bugText.color;
            c.a = Mathf.Lerp(0f, 0.7f, t);
            bugText.color = c;
            yield return null;
        }
    }
}