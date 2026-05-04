using UnityEngine;
using UnityEngine.UI;

public class CameraPan : MonoBehaviour
{
    [SerializeField] private Slider panSlider;
    [SerializeField] private float panAmount = 3f; // how far left/right camera can travel

    private Vector3 basePosition;
    private Fish[] allFish;

    void Start()
    {
        basePosition = transform.position;
        panSlider.onValueChanged.AddListener(OnSliderChanged);
        allFish = FindObjectsOfType<Fish>();
    }

    void OnSliderChanged(float value)
    {
        transform.position = new Vector3(basePosition.x + value * panAmount, basePosition.y, basePosition.z);
        foreach (Fish fish in allFish)
            fish.RefreshBounds();
    }
}