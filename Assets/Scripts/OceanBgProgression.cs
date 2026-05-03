using UnityEngine;

public class OceanBgProgression : MonoBehaviour
{
    public Sprite[] backgrounds;
    private SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        int index = Mathf.Clamp(FishSpawner.currentLevel, 0, backgrounds.Length - 1);
        sr.sprite = backgrounds[index];
    }
}
