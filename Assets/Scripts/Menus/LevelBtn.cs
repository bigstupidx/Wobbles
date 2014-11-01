using UnityEngine;
using System.Collections;

public class LevelBtn : MonoBehaviour 
{
    public string LevelName;

    GameObject star;
    Vector3 initialPosition;
    bool locked;
    int amountOfStars;

    void Awake()
    {
        // Store start position for later positioning
        initialPosition = transform.position;
        initialPosition.z = -1f;
    }
    void Start()
    {
        // Put in initial offscreen start position
        transform.position = new Vector3(transform.position.x, Camera.main.orthographicSize + 1f, -1f);
        
        // Check if the level is unlocked
        if(!Save.LevelIsUnlocked(LevelName))
        {
            Destroy(GetComponent<tk2dButton>());
            GetComponent<tk2dSprite>().SetSprite("lock-gold");
            locked = true;
        }
        amountOfStars = Save.GetBestScore(LevelName);
    }    

    void Tint()
    {
        GetComponent<tk2dSprite>().color = new Color(.75f, .75f, .75f);
    }
    
    void SetOffscreen()
    {
        transform.position = new Vector3(transform.position.x, -(Camera.main.orthographicSize * 2f), transform.position.z);
    }

    void DropIn(float delay)
    {
        // Drop the button down onto the map and
        // place stars on finish
        transform.position = initialPosition;
        iTween.MoveFrom(gameObject, iTween.Hash(
            "y", Camera.main.orthographicSize + 2f,
            "time", 1f, 
            "delay", Random.value * 0.5f + delay,
            "easetype", iTween.EaseType.easeInExpo,
            "oncomplete", "ExpandStars",
            "oncompletetarget", gameObject));
    }
    void DropOut(float delay)
    {
        // Remove stars then drop off map
        RetractStars();
        iTween.MoveTo(gameObject, iTween.Hash(
            "y", -(Camera.main.orthographicSize * 2f),
            "time", 0.5f,
            "delay", Random.value * 0.25f + delay,
            "easetype", iTween.EaseType.linear));
    }

    void ExpandStars()
    {
        // Calculate positions of stars and tween them
        if (amountOfStars <= 0) return;
        float totalDegrees = (amountOfStars - 1) * 45f;
        float currentDegrees = totalDegrees * -0.5f + 270f;
        for(int i=0; i<amountOfStars; ++i)
        {
            GameObject obj = Instantiate(star, transform.position + Vector3.back * 0.1f, Quaternion.identity) as GameObject;
            obj.transform.parent = transform;
            Vector3 deltaPos = new Vector3(Mathf.Cos(Mathf.Deg2Rad * currentDegrees), Mathf.Sin(Mathf.Deg2Rad * currentDegrees), 0f) * 0.5f;
            iTween.MoveTo(obj, iTween.Hash(
                "position", deltaPos,
                "time", 0.2f, 
                "islocal", true,
                "easetype", iTween.EaseType.easeInOutExpo));
            currentDegrees += 45f;
        }
    }
    void RetractStars()
    {
        // Retract them remove all stars
        foreach(Transform star in transform)
        {
            iTween.MoveTo(star.gameObject, iTween.Hash(
                "position", Vector3.forward * 0.1f,
                "time", 0.2f,
                "easetype", iTween.EaseType.easeInOutExpo,
                "islocal", true,
                "oncomplete", "OnStarFinish",
                "oncompletetarget", gameObject, 
                "oncompleteparams", star.gameObject));
        }
    }
    void OnStarFinish(GameObject star)
    {
        Destroy(star);
    }

    void OnClick()
    {
        if (!locked)
        {
            GameObject.Find("LevelMenu").SendMessage("TweenInLoadIndicator", LevelName);
        }
    }
    void SetStar(GameObject _star)
    {
        star = _star;
    }    
}
