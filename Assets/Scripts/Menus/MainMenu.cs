using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour
{
    public GameObject MuteOnBtn;
    public GameObject MuteOffBtn;

    void Start()
    {
        MuteOnBtn.SetActive(AudioManager.Mute);
        MuteOffBtn.SetActive(!AudioManager.Mute); 
    }

    void Play()
    {
        Application.LoadLevel("LevelSelect");
    }
        
    void Extras()
    {
        //Application.LoadLevel("Extras");
    }

    void Credits()
    {
        Application.LoadLevel("Credits");
    }

    void Facebook()
    {
        Application.OpenURL("https://www.facebook.com/PlayNimbus");
    }

    void Twitter()
    {
        Application.OpenURL("https://twitter.com/PlayNimbus");
    }

    void Nimbus()
    {
        Application.OpenURL("http://www.playnimbus.com/");
    }

    void Clear()
    {
        Save.ClearAndCommit();
    }

    void Bonus()
    {
        Save.AddBonus();
    }

    void Mute()
    {
        AudioManager.Mute = !AudioManager.Mute;
        MuteOnBtn.SetActive(AudioManager.Mute);
        MuteOffBtn.SetActive(!AudioManager.Mute); 
    }

    void Dubstep()
    {
        //AudioManager.PlayDubstep();
    }
}
