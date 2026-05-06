using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using NUnit.Framework.Constraints;

public class LighthouseManager : MonoBehaviour
{
    private bool hasTurnedAround = false;
    
    [Header("Stages")]
    [SerializeField] private GameObject stage1;
    [SerializeField] private GameObject stage2;
    [SerializeField] private GameObject stage3;
    [SerializeField] private GameObject stage4;
    [SerializeField] private GameObject stage5;

    [Header("Info Panel")]
    [SerializeField] private GameObject popUpCanvas;
    [SerializeField] private GameObject infoPanel;
    [SerializeField] private TextMeshProUGUI infoText;
    [SerializeField] private Button closeButton;
    [SerializeField] private TMP_FontAsset defaultFont;
    [SerializeField] private TMP_FontAsset altFont;
    [SerializeField] private GameObject turnAround3;
    [SerializeField] private GameObject turnAround4;
    [SerializeField] private GameObject turnAround5;

    private bool infoPanelOpen = false;
    private Dictionary<string, TMP_FontAsset> objectFonts = new Dictionary<string, TMP_FontAsset>();

    private Dictionary<string, string> objectTexts;

    private const string TEST_RESULTS_TEXT = @"Patient Name: [REDACTED]
    Date of Birth: 4/4/2004 (?)

    TEST RESULTS:

    CELIAC DISEASE PANEL

    TTG AB, IGA
        Normal value: <15. u/mL
        Value: <0.5
        
            RESULT: NEGATIVE
        
        Gliadin DGP Ab IgA
            Normal value: <15. u/mL
        Value: 0.3

        RESULT: NEGATIVE

    GIARDIA  AG, EIA
        RESULT: NEGATIVE
    ";

    private const string CAPE_LORE_TEXT = @"Tropical beach resort closes after frequent unexplained shipwrecks
    Located on a tiny Atlantic archipelago accessible only by boat, tropical beach resort Cape Thalassa had quickly risen in popularity internationally . Its star of the show was its one-of-a-kind aquatic wildlife, which guests could witness via scuba-diving. 
    However, after just 2 years of success, a sudden slew of unexplained shipwrecks with tens of casualties would taint Cape Thalassa's image. Witness accounts vary widely. Some describe the ships to ""hit an invisible brick wall"" shortly before sinking while others swore they saw ""legs pulling [them] down."" 
    Administration would announce jagged rocks on the coast as the cause for these tragedies and construct a red lighthouse as a safety measure. Unfortunately, three more shipwrecks would occur even in the lighthouse's presence. 
    After struggling for a year to convince guests to make the now harrowing journey to the resort, Cape Thalassa is set to close its doors by the end of the week
    ";

    private const string OCEAN_ARTICLE_TEXT = @"There is a common assumption in modern times that medicinal drugs as we know them, or at least the most effective kind, are synthetic. While modern medicine is great in its own right, its flaws are starting to peak through the cracks. In recent years, there’s been an epidemic of antibiotic-resistant bacteria that fewer and more expensive medications can treat. Humanity is caught in an arms race against pathogens, one which we are destined to lose. That is, unless we turn back to nature. 
There is a reason we model birds after our aircrafts and mimic termite mounds after our architecture. The solutions are in nature and medicine should be no different. 
	One place we’ve sorely underexplored is the ocean. The ocean covers about 360 million square kilometers and is 3,682 meters deep on average. We’ve only explored 28.7% of all this. 
There is undiscovered life to exploit in these depths.
";

    private const string POLAR_BEARS_TEXT = @"When Apex Predators Are Starved: A Case Study on Polar Bears
    Along with thinning sea ice, polar bear attacks on humans have been on the rise in recent years. As Earth gets warmer, the sea ice takes longer to solidify with each passing year. Since the large, half-ton bears rely on seals that dwell on the sea ice for food, it's not uncommon for polar bears to go months without eating in the summer. Amidst these bleak conditions, these bears are desperate for alternative food sources.
    It just so happens that less ice brings more humans closer to the bears, including those with less bear experience – more tourists, more industries, more shipping. And just like that,  we have all the ingredients for increased polar bear-human contact and conflict.
    Studies show that 61% of bears that attacked humans were estimated to be in below-average body condition. Furthermore, 88% percent of attacks occurred between July and December since 2020, when the sea ice usually covers the smallest area. This is a 20% increase from attacks that occurred at the same time of year between 1870 to 2014.
    ";

    private const string POEM_TEXT = @"Breathe.
Breathe
breathe
breath
When will I draw my last?
I don’t want to see what’s on the other side
But the abyss in my veins
Drags me six feet under
If I tear out my intestines, will they stop aching?
If I puncture my lungs, will they stop wheezing?
No, I refuse to genuflect
To the scythe blade against my throat
I’ll claw my way out of this grave
No you won’t take me yet
Not yet
Not yet
Not yet.
    ";

    void BuildObjectTexts()
    {
        objectTexts = new Dictionary<string, string>()
        {
            // stage 0
            { "stage0_testresults", TEST_RESULTS_TEXT },
            { "stage0_symptoms", @"Symptoms: 
            -nausea
            -160->147->132->121 lbs
            -fatigue
    " },
            { "stage0_checklist", @"Recipe for Ozempic:
            -crab legs" },

            // stage 1
            { "stage1_capelore",    CAPE_LORE_TEXT },
            { "stage1_testresults", TEST_RESULTS_TEXT },
            { "stage1_symptoms", @"Symptoms: 
            -nausea
            -160->147->132->121 lbs
            -fatigue
            -vomiting
            -abdominal pain
            everything fucking hurts
            " },
            { "stage1_checklist", @"Recipe for Ozempic:
            -crab legs
            -fish fins" },

            // stage 2
            { "stage2_capelore",      CAPE_LORE_TEXT },
            { "stage2_testresults",   TEST_RESULTS_TEXT },
            { "stage2_symptoms", @"Symptoms: 
            -nausea
            -160->147->132->121 lbs
            -fatigue
            -vomiting
            -abdominal pain
            -everything fucking hurts
            -EVERYTHING FUCKING HURTS" },
            { "stage2_oceanarticle",  OCEAN_ARTICLE_TEXT },
            { "stage2_polarbears",    POLAR_BEARS_TEXT },
            { "stage2_checklist", @"Recipe for Ozempic:
            -crab legs
            -fish fins
            -fish eyes" },

            // stage 3
            { "stage3_capelore",     CAPE_LORE_TEXT },
            { "stage3_testresults",  TEST_RESULTS_TEXT },
            { "stage3_symptoms", @"Symptoms: 
            -nausea
            -160->147->132->121 lbs
            -fatigue
            -vomiting
            -abdominal pain
            -everything fucking hurts
            -EVERYTHING FUCKING HURTS" },
            { "stage3_oceanarticle", OCEAN_ARTICLE_TEXT },
            { "stage3_polarbears",   POLAR_BEARS_TEXT },
            { "stage3_checklist", @"Recipe for Ozempic:
            -crab legs
            -fish fins
            -fish eyes
            -octopus tentacles" },
            { "stage3_poem", POEM_TEXT },

            // stage 4
            { "stage4_capelore",     CAPE_LORE_TEXT },
            { "stage4_testresults",  TEST_RESULTS_TEXT },
            { "stage4_symptoms", @"Symptoms: 
            -nausea
            -160->147->132->121 lbs
            -fatigue
            -vomiting
            -abdominal pain
            -everything fucking hurts
            -EVERYTHING FUCKING HURTS
            -why are they staring at me 
            -they’re coming to get me.

            " },
            { "stage4_oceanarticle", OCEAN_ARTICLE_TEXT },
            { "stage4_polarbears",   POLAR_BEARS_TEXT },
            { "stage4_checklist", @"Recipe for Ozempic:
            -crab legs
            -fish fins
            -fish eyes
            -octopus tentacles
            -a miracle as rare as getting an internship at Berkeley" },
            { "stage4_poem", POEM_TEXT }
        };
    }

    void Awake()
    {
        BuildObjectTexts();
        objectFonts["stage0_symptoms"] = altFont;
        objectFonts["stage1_symptoms"] = altFont;
        objectFonts["stage2_symptoms"] = altFont;
        objectFonts["stage3_symptoms"] = altFont;
        objectFonts["stage4_symptoms"] = altFont;
        popUpCanvas.SetActive(false);
        infoPanelOpen = false;
        infoText.color = Color.black;
        stage1.SetActive(false);
        stage2.SetActive(false);
        stage3.SetActive(false);
        stage4.SetActive(false);
        stage5.SetActive(false);
        turnAround3.SetActive(false);
        turnAround4.SetActive(false);
        turnAround5.SetActive(false);

        if (stage1 != null) stage1.SetActive(FishSpawner.currentLevel == 0);
        if (stage2 != null) stage2.SetActive(FishSpawner.currentLevel == 1);
        if (stage3 != null) stage3.SetActive(FishSpawner.currentLevel == 2);
        if (stage4 != null) stage4.SetActive(FishSpawner.currentLevel == 3);
        if (stage5 != null) stage5.SetActive(FishSpawner.currentLevel == 4);

        closeButton.onClick.AddListener(OnCloseInfo);
        revampedAudio.Instance.UpdateMusic();
    }

    // Hook this up in each Button's OnClick in the inspector.
    // Pass the flavor text as the string argument.
    public void OnObjectClicked(string objectID)
    {
        if (infoPanelOpen) return;

        string key = $"stage{FishSpawner.currentLevel}_{objectID}";
        if (!objectTexts.ContainsKey(key)) return;

        infoText.text = objectTexts[key];
        infoText.font = objectFonts.ContainsKey(key) ? objectFonts[key] : defaultFont;
        infoText.enableAutoSizing = true;
        infoText.fontSizeMin = 8f;
        infoText.fontSizeMax = 24f;
        popUpCanvas.SetActive(true);
        infoPanelOpen = true;
    }

    public void OnCloseInfo()
    {
        popUpCanvas.SetActive(false);
        infoPanelOpen = false;
    }

    public void TurnAroundLighthouse3()
    {
        if (hasTurnedAround == false)
        {
            Debug.Log("TurnAround clicked!");
            turnAround3.SetActive(true);
            hasTurnedAround = true;
        }
        else
        {
            Debug.Log("TurnAround clicked!");
            turnAround3.SetActive(false);
            hasTurnedAround = false;
        }
    }

    public void TurnAroundLigthouse4()
    {
        if (hasTurnedAround == false)
        {
            Debug.Log("TurnAround clicked!");
            turnAround4.SetActive(true);
            hasTurnedAround = true;
        }
        else
        {
            Debug.Log("TurnAround clicked!");
            turnAround4.SetActive(false);
            hasTurnedAround = false;
        }  
    }

    public void TurnAroundLighthouse5()
    {
        if (hasTurnedAround == false)
        {
            Debug.Log("TurnAround clicked!");
            turnAround5.SetActive(true);
            hasTurnedAround = true;
        }
        else
        {
            Debug.Log("TurnAround clicked!");
            turnAround5.SetActive(false);
            hasTurnedAround = false;
        }  
    }
    
    public void ToTheOcean()
    {
        FishSpawner.currentLevel++;
        revampedAudio.Instance.UpdateMusic();
        hasTurnedAround = false;
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