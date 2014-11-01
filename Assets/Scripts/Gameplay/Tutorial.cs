using UnityEngine;
using System.Collections;

public class Tutorial : MonoBehaviour
{
    public string[] Messages;
    public tk2dTextMesh TutorialText;
    public GameObject[] ThingsToHide;
    public GameObject TutorialSprite;  

    bool doneShowingText = false;
    int currentMessage = 0;

    void Start()
    {
        //TutorialText.text = Messages[0];
        //TutorialText.maxChars = TutorialText.text.Length;
        //TutorialText.Commit();

        foreach(GameObject obj in ThingsToHide)
        {
            if (obj) obj.SetActive(false);
        }
        iTween.ScaleTo(GameObject.Find("TutorialBtn"), iTween.Hash(
            "scale", Vector3.one * 1.2f,
            "time", 0.5f,
            "looptype", iTween.LoopType.pingPong,
            "ignoretimescale", true,
            "easetype", iTween.EaseType.linear));
    }
    void Update()
    {
        if(!doneShowingText) Menu.State = GameMenuState.Tutorial;
    }

    void TutorialDone()
    {
        Menu.State = GameMenuState.Planning;
        foreach (GameObject obj in ThingsToHide)
        {
            if (obj) obj.SetActive(true);
        }
        Destroy(TutorialSprite);
        doneShowingText = true;
        return;
        if (currentMessage < Messages.Length - 1)
        {
            ++currentMessage;
            TutorialText.text = Messages[currentMessage];
            TutorialText.maxChars = TutorialText.text.Length;
            TutorialText.Commit();
            return;
        }
        else
        {
            Menu.State = GameMenuState.Planning;
            foreach (GameObject obj in ThingsToHide)
            {
                if (obj) obj.SetActive(true);
            }
            Destroy(TutorialSprite);
            doneShowingText = true;
        }
    }
}
