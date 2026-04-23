using UnityEngine;
using UnityEngine.UI;

public class CameraPan : MonoBehaviour
{
    [SerializeField] private Slider panSlider;
    [SerializeField] private float panAmount = 3f; // how far left/right camera can travel

    private Vector3 basePosition;

    void Start()
    {
        basePosition = transform.position; // store camera's original center position
        panSlider.onValueChanged.AddListener(OnSliderChanged);
    }

    void OnSliderChanged(float value)
    {
        // value is -1 to 1, multiply by panAmount to get world offset
        transform.position = new Vector3(basePosition.x + value * panAmount, basePosition.y, basePosition.z);
    }
}