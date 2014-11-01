using UnityEngine;

public class LevelControl : MonoBehaviour
{
    public tk2dTextMesh WobbleCountText;
    public tk2dSprite[] Stars;

    bool levelWon;
    int world, level;
    int wobblesScored;
    int wobblesRemaining;
    [HideInInspector]
    public int starsScored;
    int wobblesAtStart;
    
    // Unity functions
    void Start()
    {
        if(GetComponent<Menu>().OneStarLevel)
        {
            Destroy(Stars[1].gameObject);
            Destroy(Stars[2].gameObject);
        }
        GameObject home = GameObject.Find("Home");
        if (home)
        {
            home.collider.isTrigger = true;
            BoxCollider boxColider = home.collider as BoxCollider;
            if (boxColider)
            {
                boxColider.size = new Vector3(boxColider.size.x, boxColider.size.y, 5f);
            }
        }
    }
    void OnDestroy()
    {
        // Return timescale to normal so game functions normally
        Time.timeScale = 1.0f;
    }
    
    // Helper functions
    void AddWobble(int amount)
    {
        wobblesAtStart += amount;
        wobblesRemaining += amount;

        WobbleCountText.text = wobblesScored.ToString() + "/" + wobblesAtStart.ToString();
        WobbleCountText.Commit();
    }
    void ScoreWobble()
    {        
        ++wobblesScored;
        WobbleCountText.text = wobblesScored.ToString() + "/" + wobblesAtStart.ToString();
        WobbleCountText.Commit();
        if(wobblesAtStart == wobblesScored)
        {
            Win();
        }
    }
    void SubtractWobble()
    {
        --wobblesRemaining;
        if (wobblesRemaining <= 0)
        {
            if (wobblesScored > 0)
            {
                Win();
            }
            else
            {
                Lose();
            }
        }
    }
    void ScoreStar()
    {
        if (GetComponent<Menu>().OneStarLevel)
        {            
            Stars[0].SetSprite("star");
            starsScored = 1;
        }
        else
        {
            if (starsScored < 3)
            {
                Stars[starsScored++].SetSprite("star");
            }
        }
        gameObject.SendMessage("StarScored");
    }   
    void Win()
    {
        if (levelWon) return; else levelWon = true;
        GA.API.Design.NewEvent(Application.loadedLevelName + " stars scored.", (float)starsScored);
        GA.API.Design.NewEvent(Application.loadedLevelName + " time to complete.", Time.timeSinceLevelLoad);
        GA.API.Design.NewEvent(Application.loadedLevelName + " wobbles lost.", (float)(wobblesAtStart - wobblesScored));

        Save.SubmitScore(Application.loadedLevelName, starsScored);
        Save.SetComplete(Application.loadedLevelName);
        GadgetDrag.InDrag = false;
        Time.timeScale = 0.0f;
        gameObject.SendMessage("Win");
    }
    void Lose()
    {
        if (levelWon) return; else levelWon = true;
        GadgetDrag.InDrag = false;
        Time.timeScale = 0.0f;
        gameObject.SendMessage("Lose");
    }
}
