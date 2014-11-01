using UnityEngine;
using System.Collections;

public class LevelMenuControl : MonoBehaviour
{
    public GameObject[] WorldButtons;
    public GameObject[] Maps;
    public GameObject[] QuickButtons;
    public GameObject[] StorySlides;
    public GameObject[] Titles;
    public GameObject StoryParent;
    public GameObject Star;
    public tk2dSprite Mapbase;
    public GameObject LoadingSprite;
    public tk2dTextMesh WorldRequirements;

    public GameObject InitialStory;
        
    Vector3 prevMousePosition;
    static int currentWorld = 0;
    float screenWidth;
    float timeSinceSwitch;
    BitArray worldUnlocked;
    bool storyBeingShown = false;
    bool inSwitch;

    void Awake()
    {
        // Sets the star sprite to be used in the menu
        foreach(GameObject obj in WorldButtons)
        {
            if (obj)
            {
                obj.SetActive(true);
                obj.BroadcastMessage("SetStar", Star);
            }
        }
    }
    void Start()
    {
        WorldRequirements.gameObject.SetActive(false);
        worldUnlocked = new BitArray(6, false); worldUnlocked[0] = true;
        LoadingSprite.transform.localScale = Vector3.zero;
        LoadingSprite.SetActive(false);
        screenWidth = Camera.main.orthographicSize * (4f/3f) * 2f +  1f;

        StoryParent.SetActive(false);

        if(!Save.IsStoryShown(0))
        {
            worldUnlocked[0] = true;
            SwitchWorld(0);
            storyBeingShown = true;
            InitialStory.SetActive(true);
            iTween.MoveFrom(InitialStory, iTween.Hash(
                "position", new Vector3(0f, Camera.main.orthographicSize * 2f, -2f),
                "time", 1f,
                "easetype", iTween.EaseType.easeOutExpo));
            Save.SetStoryShown(0);
        }

        for(int i=0; i<6; ++i)
        {
            // Positions maps in a row parented to this object
            Maps[i].transform.parent = transform;
            Maps[i].transform.localPosition = new Vector3(screenWidth * i, 0f, 0f);
                        
            // Check for world unlocks, tint correponding backgrounds and buttons if locked
            if(!Save.WorldIsUnlocked(i))
            {
                Maps[i].GetComponent<tk2dSprite>().color = new Color(0f, 0f, 0f);
                WorldButtons[i].BroadcastMessage("Tint");
            }
            else if (i > 0)
            {
                worldUnlocked[i] = true;                
            }
        }
        for (int i = 1; i < 6; ++i)
        {
            if (Save.WorldIsUnlocked(i) && !Save.IsStoryShown(i))
            {
                SwitchWorld(i);
                ShowStory(i);
                Save.SetStoryShown(i);
                break;
            }
        }
        
        // Set up the current world
        transform.Translate(Vector3.left * currentWorld * screenWidth);
        QuickButtons[currentWorld].transform.rotation = Quaternion.Euler(0f, 0f, 10f);
        iTween.RotateFrom(QuickButtons[currentWorld], iTween.Hash(
            "z", -10f,
            "time", 1f,
            "easetype", iTween.EaseType.linear,
            "looptype", iTween.LoopType.pingPong
            ));     
        WorldButtons[currentWorld].BroadcastMessage("DropIn", 0f);
        Titles[currentWorld].SetActive(true);
    }
    void Update()
    {
        timeSinceSwitch += Time.deltaTime;
        if (timeSinceSwitch > 1.5f) inSwitch = false;
        if (storyBeingShown) return;
        if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android)
        {
            if (Input.touchCount > 0 && !inSwitch)
            {
                if(Input.touches[0].phase == TouchPhase.Began )
                {
                    iTween.Stop(gameObject);
                }
                if(Input.touches[0].phase == TouchPhase.Moved)
                {
                    Vector3 translateAmt = Vector3.zero;
                    translateAmt.x = (Camera.main.ScreenToWorldPoint(Input.touches[0].deltaPosition) - Camera.main.ScreenToWorldPoint(Vector3.zero)).x;
                    transform.Translate(translateAmt);
                }
                if (Input.touches[0].phase == TouchPhase.Ended)
                {
                    if (transform.position.x > (-currentWorld - 0.25f) * screenWidth && transform.position.x < (-currentWorld + 0.25f) * screenWidth)
                    {
                        iTween.MoveTo(gameObject, iTween.Hash(
                            "x", -currentWorld * screenWidth,
                            "time", 1f,
                            "easetype", iTween.EaseType.linear));
                    }
                    else
                    {
                        if (transform.position.x > -currentWorld * screenWidth)
                        {
                            if (currentWorld > 0)
                            {
                               SwitchWorld(currentWorld - 1);
                            }
                            iTween.MoveTo(gameObject, iTween.Hash(
                                "x", -currentWorld * screenWidth,
                                "time", 0.5f,
                                "easetype", iTween.EaseType.linear));
                        }
                        else
                        {
                            if (currentWorld < 5)
                            {
                                SwitchWorld(currentWorld + 1);
                            }
                            iTween.MoveTo(gameObject, iTween.Hash(
                                "x", -currentWorld * screenWidth,
                                "time", 0.5f,
                                "easetype", iTween.EaseType.linear));
                        }
                    }
                }
            }
        }
        else if(!inSwitch)
        {
            if(Input.GetMouseButtonDown(0))
            {
                prevMousePosition = Input.mousePosition;
                iTween.Stop(gameObject);
            }
            if (Input.GetMouseButton(0))
            {
                Vector3 translateAmt = Input.mousePosition - prevMousePosition;
                prevMousePosition = Input.mousePosition;
                translateAmt = Camera.main.ScreenToWorldPoint(translateAmt) - Camera.main.ScreenToWorldPoint(Vector3.zero);
                translateAmt.y = 0f;
                translateAmt.z = 0f;
                transform.Translate(translateAmt);
            }
            if(Input.GetMouseButtonUp(0))
            {
                if(transform.position.x > (-currentWorld - 0.25f) * screenWidth && transform.position.x < (-currentWorld + 0.25f) * screenWidth)
                {
                    iTween.MoveTo(gameObject, iTween.Hash(
                        "x", -currentWorld * screenWidth,
                        "time", 1f,
                        "easetype", iTween.EaseType.linear));                        
                }
                else
                {
                    if(transform.position.x > -currentWorld * screenWidth)
                    {                        
                        if(currentWorld > 0)
                        {
                            SwitchWorld(currentWorld - 1);
                        }
                        iTween.MoveTo(gameObject, iTween.Hash(
                            "x", -currentWorld * screenWidth,
                            "time", 0.5f,
                            "easetype", iTween.EaseType.linear));
                    }
                    else
                    {                        
                        if(currentWorld < 5)
                        {
                            SwitchWorld(currentWorld + 1);
                        }
                        iTween.MoveTo(gameObject, iTween.Hash(
                            "x", -currentWorld * screenWidth,
                            "time", 0.5f,
                            "easetype", iTween.EaseType.linear));  
                    }
                }
            }
        }
    }

    void SwitchWorld(int targetWorld)
    {
        inSwitch = true;
        timeSinceSwitch = 0f;
        if (targetWorld == currentWorld) return;
        for(int i=0; i<6; i++)
        {
            if(i != currentWorld) WorldButtons[i].BroadcastMessage("SetOffscreen");
        }

        // Remove previous world
        WorldButtons[currentWorld].BroadcastMessage("DropOut", 0f);
        iTween.Stop(QuickButtons[currentWorld]);
        QuickButtons[currentWorld].transform.rotation = Quaternion.identity;
        Titles[currentWorld].SetActive(false);

        currentWorld = targetWorld;
        
        QuickButtons[currentWorld].transform.rotation = Quaternion.Euler(0f, 0f, 10f);
        iTween.RotateFrom(QuickButtons[currentWorld], iTween.Hash(
            "z", -10f,
            "time", 1f, 
            "easetype", iTween.EaseType.linear,
            "looptype", iTween.LoopType.pingPong
            ));
        Titles[currentWorld].SetActive(true);

        WorldButtons[currentWorld].BroadcastMessage("DropIn", 0f);


        //Check if current world is unlocked and tint if not
        if(worldUnlocked[currentWorld])
        {
            // unlocked
            if (Mathf.Approximately(Mapbase.color.r, 0.5f))
            {
                iTween.ValueTo(gameObject, iTween.Hash(
                    "from", 0.5f,
                    "to", 1f,
                    "onupdate", "OnUpdateTintMap",
                    "onupdatetarget", gameObject,
                    "time", 0.5f));
            }
            WorldRequirements.gameObject.SetActive(false);
        }
        else
        {
            // locked
            if (Mathf.Approximately(Mapbase.color.r, 1f))
            {
                iTween.ValueTo(gameObject, iTween.Hash(
                    "from", 1f,
                    "to", 0.5f,
                    "onupdate", "OnUpdateTintMap",
                    "onupdatetarget", gameObject,
                    "time", 0.5f));
            }
            WorldRequirements.gameObject.SetActive(true);
            WorldRequirements.text = Save.GetTotalStars().ToString() + "/" + Save.GetWorldRequirements(currentWorld).ToString(); 
            WorldRequirements.Commit();
        }
    }

    void OnUpdateTintMap(float value)
    {
        Mapbase.color = new Color(value, value, value);
    }
    void ShowStory(int world)
    {
        StoryParent.SetActive(true);
        storyBeingShown = true;
        StorySlides[world].SetActive(true);
        iTween.MoveFrom(StoryParent, iTween.Hash(
            "position", new Vector3(0f, Camera.main.orthographicSize * 2f, -2f),
            "time", 1f,
            "easetype", iTween.EaseType.easeOutExpo
            ));
    }
    void HideStory()
    {
        storyBeingShown = false;
        iTween.MoveTo(StoryParent, iTween.Hash(
            "position", new Vector3(0f, Camera.main.orthographicSize * 2f, -2f),
            "time", 1f,
            "easetype", iTween.EaseType.easeOutExpo
            ));
    }
    void HideInitialStory()
    {
        storyBeingShown = false;
        InitialStory.SetActive(false);
    }

    // Button to go to main menu
    void Home()
    {
        Application.LoadLevel("MainMenu");
    }

    // Called by buttons to go to level
    void TweenInLoadIndicator(string name)
    {
        if (!storyBeingShown)
        {
            LoadingSprite.SetActive(true);
            iTween.ScaleTo(LoadingSprite, iTween.Hash(
                "scale", Vector3.one,
                "time", 0.25f,
                "easetype", iTween.EaseType.easeOutElastic,
                "oncomplete", "LoadLevel",
                "oncompletetarget", gameObject,
                "oncompleteparams", name));
        }
    }
    void LoadLevel(string name)
    {        
        Application.LoadLevel(name);
    }

    // Shortcut buttons
    void One()
    {
        if (currentWorld != 0 && !storyBeingShown && !inSwitch)
        {
            iTween.MoveTo(gameObject, iTween.Hash(
                "x", 0f,
                "time", 0.5f,
                "easetype", iTween.EaseType.linear));
            SwitchWorld(0);    
        }
    }
    void Two()
    {
        if (currentWorld != 1 && !storyBeingShown && !inSwitch)
        {
            iTween.MoveTo(gameObject, iTween.Hash(
                "x", -screenWidth,
                "time", 0.5f,
                "easetype", iTween.EaseType.linear));
            SwitchWorld(1);
        }
    }
    void Three()
    {
        if (currentWorld != 2 && !storyBeingShown && !inSwitch)
        {
            iTween.MoveTo(gameObject, iTween.Hash(
                "x", -2f * screenWidth,
                "time", 0.5f,
                "easetype", iTween.EaseType.linear));
            SwitchWorld(2);
        }
    }
    void Four()
    {
        if (currentWorld != 3 && !storyBeingShown && !inSwitch)
        {
            iTween.MoveTo(gameObject, iTween.Hash(
                "x", -3f * screenWidth,
                "time", 0.5f,
                "easetype", iTween.EaseType.linear));
            SwitchWorld(3);
        }
    }
    void Five()
    {
        if (currentWorld != 4 && !storyBeingShown && !inSwitch)
        {
            iTween.MoveTo(gameObject, iTween.Hash(
                "x", -4f * screenWidth,
                "time", 0.5f,
                "easetype", iTween.EaseType.linear));
            SwitchWorld(4);
        }
    }
    void Six()
    {
        if (currentWorld != 5 && !storyBeingShown && !inSwitch)
        {
            iTween.MoveTo(gameObject, iTween.Hash(
                "x", -5f * screenWidth,
                "time", 0.5f,
                "easetype", iTween.EaseType.linear));
            SwitchWorld(5);
        }
    }
}
