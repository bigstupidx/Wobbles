using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour 
{
    static AudioManager Instance;
    
    public AudioClip MenuMusic;
    public AudioClip Dubstep;
    public AudioClip[] Music;
    public AudioClip[] Clips;

    bool mute = false;
    static public bool Mute
    {
        get
        {
            if (Instance)
            {
                return Instance.mute;
            }
            else
            {
                return false;
            }
        }
        set
        {
            if (Instance)
            {
                Instance.mute = value;
                if (value) Instance.src.volume = 0f;
                else Instance.src.volume = 1f;
            }
        }
    }

    Hashtable clipTable;
    AudioSource src;
    //float timeSinceAwake;

    void Awake()
    {
        //if (Screen.height > 800) tk2dSystem.CurrentPlatform = "2x";
        //else tk2dSystem.CurrentPlatform = "1x";
    }

    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            src = GetComponent<AudioSource>();
            src.clip = MenuMusic;
            src.loop = true;
            src.volume = 0.8f;
            src.Play();

            clipTable = new Hashtable(Clips.Length);
            foreach (AudioClip clip in Clips)
            {
                if (clip) clipTable.Add(clip.name, clip);
            }
            Clips = null;
        }
        else
        {
            print("Destroying duplicate audio manager");
            Destroy(gameObject);
        } 
    }

    /*void Update()
    {
        timeSinceAwake += Time.deltaTime;
        if (timeSinceAwake > 2.1f)
        {
            if (Menu.State != GameMenuState.Closed)
            {
                iTween.AudioTo(gameObject, iTween.Hash(
                    "pitch", 1f,
                    "time", 1f
                    ));
            }
            else
            {
                float targetPitch = Mathf.Clamp(Time.timeScale, 0.95f, 1.05f);
                iTween.AudioUpdate(gameObject, iTween.Hash(
                    "pitch", targetPitch,
                    "time", targetPitch
                    ));
            }
        }
    }*/

    void OnLevelWasLoaded(int scene)
    {
        if (GetHashCode() == Instance.GetHashCode())
        {
            if (!mute)
            {
                if (Application.loadedLevelName == "MainMenu" || Application.loadedLevelName == "LevelSelect" || Application.loadedLevelName == "Credits")
                {
                    if (Instance.src.clip.name != Instance.MenuMusic.name)
                    {
                        TweenMusicOut(MenuMusic);
                    }
                }
                else
                {
                    int world, level;
                    Save.GetLevelIndices(Application.loadedLevelName, out world, out level);
                    if (src.clip.name != Music[world].name)
                    {
                        TweenMusicOut(Music[world]);
                    }
                }
            }
        }
    }
    void OnApplicationQuit()
    {
        Save.Commit();
    }
    void OnApplicationPause(bool pauseValue)
    {
        if (pauseValue) Save.Commit();
    }

    void TweenMusicOut(AudioClip targetMusic)
    {
        iTween.AudioTo(gameObject, iTween.Hash(
            "audiosource", src,
            "volume", 0f,
            "time", 1f,
            "oncomplete", "TweenMusicIn",
            "oncompletetarget", gameObject,
            "oncompleteparams", targetMusic));
    }
    void TweenMusicIn(AudioClip targetMusic)
    {
        src.Stop();
        src.clip = targetMusic;
        src.loop = true;
        src.Play();
        iTween.AudioTo(gameObject, iTween.Hash(
            "audiosource", src,
            "volume", 1f,
            "time", 1f));
    }

    static public void Play(string name)
    {        
        if (Instance && !Instance.mute)
        {
            if (Instance.clipTable.Contains(name))
            {               
                AudioSource.PlayClipAtPoint(Instance.clipTable[name] as AudioClip, Vector3.zero);
            }
            else
            {
                Debug.LogWarning(name + " not found in audio clips.");
            }
        }
    }
    static public void PlayDubstep()
    {
        if (Instance && !Instance.mute && Instance.src.clip.name != Instance.Dubstep.name)
        {
            Instance.src.Stop();
            Instance.src.clip = Instance.Dubstep;
            Instance.src.loop = true;
            Instance.src.Play();
        }
    }

    static public AudioClip GetClip(string name)
    {
        if(Instance.clipTable.ContainsKey(name))
        {
            return Instance.clipTable[name] as AudioClip;
        }
        else
        {
            return null;
        }
    }
}
