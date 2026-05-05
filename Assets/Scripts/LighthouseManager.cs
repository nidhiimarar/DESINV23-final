using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class LighthouseManager : MonoBehaviour
{
    [Header("Stages")]
    [SerializeField] private GameObject stage1;
    [SerializeField] private GameObject stage2;
    [SerializeField] private GameObject stage3;

    [Header("Info Panel")]
    [SerializeField] private GameObject popUpCanvas;
    [SerializeField] private GameObject infoPanel;
    [SerializeField] private TextMeshProUGUI infoText;
    [SerializeField] private Button closeButton;

    private bool infoPanelOpen = false;

    private Dictionary<string, string> objectTexts = new Dictionary<string, string>()
    {
        { "stage0_testresults", @"Test results: subject is no bueno" },
        { "stage0_symptoms", @"Symptoms: coughing, dizziness, noticable decrease in aura" },
        { "stage0_checklist", @"Recipe for Ozempic:
        -crab legs" },

        { "stage1_capelore", @"Lore: mom said to go to med school but i didn't wanna so here i am :D" },  
        { "stage1_testresults", @"Test results: subject is not very sigma :(" }, 
        { "stage1_symptoms", @"Symptoms: coughing, dizziness, noticable decrease in aura, unable to understand current memes" }, 
        { "stage1_checklist", @"Recipe for Ozempic:
        -crab legs
        -fish fins" },

        { "stage2_capelore", @"Lore: mom said to go to med school but i didn't wanna so here i am :D" },  
        { "stage2_testresults", @"Test results: subject is not very sigma :(" }, 
        { "stage2_symptoms", @"Symptoms: coughing, dizziness, noticable decrease in aura, unable to understand current memes" }, 
        { "stage2_oceanarticle", @"FIHHHHHHHH" },
        { "stage2_polarbears", @"When Apex Predators Are Starved: A Case Study on Polar Bears
Along with thinning sea ice, polar bear attacks on humans have been on the rise in recent years. As Earth gets warmer, the sea ice takes longer to solidify with each passing year. Since the large, half-ton bears rely on seals that dwell on the sea ice for food, it's not uncommon for polar bears to go months without eating in the summer. Amidst these bleak conditions, these bears are desperate for alternative food sources.
It just so happens that less ice brings more humans closer to the bears, including those with less bear experience – more tourists, more industries, more shipping. And just like that,  we have all the ingredients for increased polar bear-human contact and conflict.
Studies show that 61% of bears that attacked humans were estimated to be in below-average body condition. Furthermore, 88% percent of attacks occurred between July and December since 2020, when the sea ice usually covers the smallest area. This is a 20% increase from attacks that occurred at the same time of year between 1870 to 2014.
" },
        { "stage2_checklist", @"Recipe for Ozempic:
        -crab legs
        -fish fins
        -fish eyes" },

        { "stage3_capelore", @"Lore: mom said to go to med school but i didn't wanna so here i am :D" },  
        { "stage3_testresults", @"Test results: subject is cooked X_X" }, 
        { "stage3_symptoms", @"Symptoms: coughing, dizziness, noticable decrease in aura, unable to understand current memes, OH SHIT THERES BLOOD AHHHH" }, 
        { "stage3_oceanarticle", @"FIHHHHHHHH" },
        { "stage3_polarbears", @"breaking news: idiotic humans distrupt bears nap time" },
        { "stage3_checklist", @"Recipe for Ozempic:
        -crab legs
        -fish fins
        -fish eyes
        -octopus tentacles" },

        { "stage4_capelore", @"Lore: mom said to go to med school but i didn't wanna so here i am :D" },  
        { "stage4_testresults", @"Test results: subject is cooked X_X" }, 
        { "stage4_symptoms", @"Symptoms: coughing, dizziness, noticable decrease in aura, unable to understand current memes, OH SHIT THERES BLOOD AHHHH" }, 
        { "stage4_oceanarticle", @"FIHHHHHHHH" },
        { "stage4_polarbears", @"breaking news: idiotic humans distrupt bears nap time" },
        { "stage4_checklist", @"Recipe for Ozempic:
        -crab legs
        -fish fins
        -fish eyes
        -octopus tentacles
        -a miracle as rare as getting an internship at Berkeley" }, 
    };

    void Awake()
    {
        popUpCanvas.SetActive(false);
        infoPanelOpen = false;
        infoText.color = Color.black;
        stage1.SetActive(false);
        stage2.SetActive(false);
        stage3.SetActive(false);

        if (stage1 != null) stage1.SetActive(FishSpawner.currentLevel == 0);
        if (stage2 != null) stage2.SetActive(FishSpawner.currentLevel == 1);
        if (stage3 != null) stage3.SetActive(FishSpawner.currentLevel >= 2);

        closeButton.onClick.AddListener(OnCloseInfo);
    }

    // Hook this up in each Button's OnClick in the inspector.
    // Pass the flavor text as the string argument.
    public void OnObjectClicked(string objectID)
    {
        if (infoPanelOpen) return;
        
        string key = $"stage{FishSpawner.currentLevel}_{objectID}";
        if (!objectTexts.ContainsKey(key)) return;

        infoText.text = objectTexts[key];
        popUpCanvas.SetActive(true);
        infoPanelOpen = true;
    }

    public void OnCloseInfo()
    {
        popUpCanvas.SetActive(false);
        infoPanelOpen = false;
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