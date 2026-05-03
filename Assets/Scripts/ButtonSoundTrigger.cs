using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonSoundTrigger : MonoBehaviour
{
    private void Start()
    {
        Button btn = GetComponent<Button>();
        
        // Add a listener via code so you don't have to drag references in the Inspector
        btn.onClick.AddListener(TriggerSound);
    }

    private void TriggerSound()
    {
        // Check if the instance exists to avoid the UnassignedReferenceException
        if (revampedAudio.Instance != null)
        {
            revampedAudio.Instance.PlayButtonSound();
        }
        else
        {
            Debug.LogError("No SoundManager found in the scene!");
        }
    }
}