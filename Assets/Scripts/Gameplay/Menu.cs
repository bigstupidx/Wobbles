using UnityEngine;
using System.Collections;


public enum GameMenuState
{
    Tutorial,
    Planning,
    Closed,
    Paused,
    Win,
    Lose
}

struct FuncParams
{
    public GameObject obj;
    public Vector3 position;
    public FuncParams(GameObject _obj, Vector3 _position)
    {
        obj = _obj;
        position = _position;
    }
}

public class Menu : MonoBehaviour 
{
    public GameObject Fade;
    public GameObject MenuBackground;
    public GameObject PauseMenu;
    public GameObject PauseWobble;
    public GameObject WinMenu;
    public GameObject WinWobble;
    public GameObject LoseMenu;
    public GameObject LoseWobble;
    public GameObject PauseBtn;
    public GameObject FastForwardBtn;
    public tk2dTextMesh TextMesh;
    public GameObject InitialPlayBtn;
    public tk2dSprite[] Stars;
    public bool OneStarLevel;

    static public GameMenuState State = GameMenuState.Planning;

    AudioClip MenuSound;

    bool wasPlanning = false;
    float timeSincePauseBtnPress;
    float tempLastTime;
    
    void Start()
    {
        InitialPlayBtn.SetActive(true);
        Fade.SetActive(false);
        MenuBackground.SetActive(false);
        PauseMenu.SetActive(false);
        WinMenu.SetActive(false);
        LoseMenu.SetActive(false);

        State = GameMenuState.Planning;
        MenuSound = AudioManager.GetClip("WobbleWobbleWobble");
        if (!MenuSound) print("Error retrieving MenuSound");
        
        int world, level;
        Save.GetLevelIndices(Application.loadedLevelName, out world, out level);
        ++world; ++level;
        TextMesh.text = world.ToString() + "-" + level.ToString();
        TextMesh.maxChars = TextMesh.text.Length;
        TextMesh.Commit();

        if (OneStarLevel)
        {
            Destroy(Stars[0].gameObject);
            Destroy(Stars[2].gameObject);
            Stars[0] = Stars[2] = Stars[1];
            Stars[1].SetSprite("star-dark");
        }
        else
        {            
            for (int i = 0; i < 3; ++i)
            {
                Stars[i].SetSprite("star-dark");
            }
        }

        SetZ(4f, InitialPlayBtn, PauseBtn, FastForwardBtn);
        SetZ(3f, Fade);
        SetZ(2f, MenuBackground);
        SetZ(1f, WinWobble, PauseWobble, LoseWobble);
        SetZ(1f, PauseMenu, WinMenu, LoseMenu);

        iTween.ScaleTo(InitialPlayBtn, iTween.Hash(
            "scale", Vector3.one * 1.2f,
            "time", 0.5f,
            "looptype", iTween.LoopType.pingPong,
            "ignoretimescale", true,
            "easetype", iTween.EaseType.linear));            
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && State == GameMenuState.Closed && !GadgetDrag.InDrag && Time.timeScale != 0f)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(ray))
            {
                Time.timeScale = 1f;
            }
        }
        timeSincePauseBtnPress += Time.realtimeSinceStartup - tempLastTime;
        tempLastTime = Time.realtimeSinceStartup;
    }
    void OnDestroy()
    {
        State = GameMenuState.Closed;
        Time.timeScale = 1f;
    }

    void StartPlaying()
    {
        if (State == GameMenuState.Planning)
        {
            GameObject.Find("WobbleCreator").SendMessage("StartWobbles");
            wasPlanning = false;
            PauseBtn.SetActive(true);
            Destroy(InitialPlayBtn);
            State = GameMenuState.Closed;
        }
    }    
    void Pause()
    {        
        if (State == GameMenuState.Tutorial) return;
        if(timeSincePauseBtnPress > 0.25f)
        {
            timeSincePauseBtnPress = 0f;
            if(State == GameMenuState.Closed || State == GameMenuState.Planning)
            {
                PlayWinLoseSound();
                if (State == GameMenuState.Planning)
                {
                    wasPlanning = true;
                    iTween.Pause(InitialPlayBtn);
                }
                Time.timeScale = 0f;
                PutdownBackground(PauseMenu, PauseWobble);
                State = GameMenuState.Paused;
            }
            else if(State == GameMenuState.Paused)
            {
                if(wasPlanning)
                {
                    iTween.Resume(InitialPlayBtn);
                    State = GameMenuState.Planning;
                }
                else
                {
                    State = GameMenuState.Closed;
                }
                Time.timeScale = 1f;
                PickupBackground(PauseMenu, PauseWobble);
            }
        }
    }


    void PlayWinLoseSound()
    {
        if (MenuSound)
        {
            Time.timeScale = 1f;
            AudioSource.PlayClipAtPoint(MenuSound, Vector3.zero);
            Time.timeScale = 0f;
        }
    }
    void Win()
    {
        if(State == GameMenuState.Closed)
        {
            PlayWinLoseSound();
            Time.timeScale = 0f;
            PutdownBackground(WinMenu, WinWobble);
            State = GameMenuState.Win;
        }
    }
    void Lose()
    {        
        if(State == GameMenuState.Closed)
        {
            PlayWinLoseSound();
            Time.timeScale = 0f;
            PutdownBackground(LoseMenu, LoseWobble);
            State = GameMenuState.Lose;
        }
    }
    void Home()
    {
        Application.LoadLevel("MainMenu");
    }
    void Restart()
    {
        GA.API.Design.NewEvent(Application.loadedLevelName + " restarted.");
        GA.API.Design.NewEvent(Application.loadedLevelName + " time to restart.", Time.timeSinceLevelLoad);
        Application.LoadLevel(Application.loadedLevel);
    }
    void NextLevel()
    {
        int world, level;
        Save.GetLevelIndices(Application.loadedLevelName, out world, out level);
        if (level < 9)
        {
            Application.LoadLevel("level" + (level + 2 + world * 10).ToString());
        }
        else
        {
            Application.LoadLevel("LevelSelect");
        }
    }
    void FastForward()
    {
        if(State == GameMenuState.Closed)
        {
            if(Mathf.Approximately(Time.timeScale, 1f))
            {
                Time.timeScale = 4f;                
            }
            else
            {
                Time.timeScale = 1f;
            }
        }
        else if (State == GameMenuState.Planning)
        {
            Time.timeScale = 4f; 
            GameObject.Find("WobbleCreator").SendMessage("StartWobbles");
            wasPlanning = false;
            PauseBtn.SetActive(true);
            Destroy(InitialPlayBtn);
            State = GameMenuState.Closed;
        }
    }    
    void LevelSelect()
    {
        Application.LoadLevel("LevelSelect");
    }

    int currentStar = 0;
    void StarScored()
    {
        if(OneStarLevel)
        {
            Stars[1].SetSprite("star");
        }
        else
        {
            if (currentStar < 3)
            {
                Stars[currentStar++].SetSprite("star");
            }
        }
    }

    void PutdownBackground(GameObject menu, GameObject wobble)
    {
        menu.SetActive(true);
        iTween.MoveFrom(menu, iTween.Hash(
            "position", menu.transform.position + Vector3.up * 6f,
            "easetype", iTween.EaseType.easeOutQuad,
            "time", 0.25f,
            "ignoretimescale", true));

        wobble.SetActive(true);
        iTween.MoveFrom(wobble, iTween.Hash(
            "position", wobble.transform.position + Vector3.down * 6f,
            "easetype", iTween.EaseType.easeOutQuad,
            "time", 0.25f,
            "ignoretimescale", true));
  

        MenuBackground.SetActive(true);
        iTween.MoveFrom(MenuBackground, iTween.Hash(
            "position", MenuBackground.transform.position + Vector3.up * 6f,
            "easetype", iTween.EaseType.easeOutQuad,
            "time", 0.25f,
            "ignoretimescale", true));
        
        Fade.SetActive(true);
        PauseBtn.SetActive(false);
        FastForwardBtn.SetActive(false);
        if (InitialPlayBtn) InitialPlayBtn.SetActive(false);
    }
    void PickupBackground(GameObject menu, GameObject wobble)
    {
        iTween.MoveTo(menu, iTween.Hash(
            "position", menu.transform.position + Vector3.up * 6f,
            "easetype", iTween.EaseType.easeOutQuad,
            "time", 0.25f,
            "ignoretimescale", true,
            "oncomplete", "ReturnToOriginalPosition",
            "oncompletetarget", gameObject,
            "oncompleteparams", new FuncParams(menu, menu.transform.position)));

        iTween.MoveTo(wobble, iTween.Hash(
            "position", wobble.transform.position + Vector3.down * 6f,
            "easetype", iTween.EaseType.easeOutQuad,
            "time", 0.25f,
            "ignoretimescale", true,
            "oncomplete", "ReturnToOriginalPosition",
            "oncompletetarget", gameObject,
            "oncompleteparams", new FuncParams(wobble, wobble.transform.position)));
        
        iTween.MoveTo(MenuBackground, iTween.Hash(
            "position", MenuBackground.transform.position + Vector3.up * 6f,
            "easetype", iTween.EaseType.easeOutQuad,
            "time", 0.25f,
            "ignoretimescale", true,
            "oncomplete", "ReturnToOriginalPosition",
            "oncompletetarget", gameObject,
            "oncompleteparams", new FuncParams(MenuBackground, MenuBackground.transform.position)));

        Fade.SetActive(false);
        if(InitialPlayBtn) InitialPlayBtn.SetActive(true);
        PauseBtn.SetActive(true);
        FastForwardBtn.SetActive(true);
    }
    void ReturnToOriginalPosition(FuncParams param)
    {
        param.obj.SetActive(false);
        param.obj.transform.position = param.position;
    }
    void SetZ(float z, params GameObject[] obj)
    {
        foreach (GameObject o in obj)
        {
            o.transform.localPosition = new Vector3(o.transform.localPosition.x, o.transform.localPosition.y, z);
        }
    }
}
