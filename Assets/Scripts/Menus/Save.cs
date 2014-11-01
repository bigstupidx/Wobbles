using UnityEngine;
using System.Collections;
using System.IO;

public struct World
{
    public int NumberOfStars;
    public int[] Levels;
}

public static class Save 
{
    static BitArray levelsCompleted;
    static BitArray storyShown;    
    static World[] worlds;
    static int[] requirements;
    static int totalStars;

    // Accessing functions
    static public void GetLevelIndices(string levelName, out int world, out int level)
    {
        int levelNum;
        if (int.TryParse(levelName.ToLower().Replace("level", ""), out levelNum))
        {
            --levelNum;
            world = levelNum / 10;
            level = levelNum % 10;
        }
        else
        {
            world = level = 0;
            Debug.LogError("Error parsing level name into indices");
        }
    }

    // Level unlock/lock functions
    static public void SetComplete(string levelName)
    {
        int world, level;
        GetLevelIndices(levelName, out world, out level);
        levelsCompleted[level + world * 10] = true;
    }
    static public void SubmitScore(string levelName, int score)
    {
        if(score > 3) score = 3;
        int world, level;
        GetLevelIndices(levelName, out world, out level);

        if(worlds[world].Levels[level] < score)
        {
            totalStars += (score - worlds[world].Levels[level]);
            worlds[world].Levels[level] = score;
        }
    }
    static public bool LevelIsUnlocked(string levelName)
    {
        int world, level;
        GetLevelIndices(levelName, out world, out level);
        if (!WorldIsUnlocked(world)) return false;
        else if (level <= 0) return true;
        else return levelsCompleted[level - 1 + world * 10];
    }
    static public bool WorldIsUnlocked(int world)
    {
        if (world <= 0) return true;           
        else return requirements[world] <= totalStars;
    }
    static public int GetBestScore(string levelName)
    {
        int world, level;
        GetLevelIndices(levelName, out world, out level);
        return worlds[world].Levels[level];
    }
    static public void SetStoryShown(int story)
    {
        if (story >= 0 && story < 6) storyShown[story] = true;
    }
    static public bool IsStoryShown(int story)
    {
        return storyShown[story];
    }
    static public int GetTotalStars()
    {
        return totalStars;
    }
    static public int GetWorldRequirements(int world)
    {
        return requirements[world];
    }

    // Interface
    static public void Initialize(int[] req)
    {
        requirements = req;
        if (PlayerPrefs.HasKey("SaveCreated"))
        {
            Load();
        }
        else
        {
            PlayerPrefs.SetInt("SaveCreated", 1);
            ClearAndCommit();
        }
    }
    static public void ClearAndCommit()
    {
        levelsCompleted = new BitArray(60, false);
        storyShown = new BitArray(6, false);
        worlds = new World[6];
        totalStars = 0;
        for (int i = 0; i < 6; ++i)
        {
            worlds[i].Levels = new int[10];
        }
        Commit();
    }
    static public void Commit()
    {
        for (int i = 0; i < 6; ++i)
        {
            for (int j = 0; j < 10; ++j)
            {
                string levelKey = "World" + i + "Level" + j;
                PlayerPrefs.SetInt(levelKey, worlds[i].Levels[j]);
                if (levelsCompleted[j + i * 10])
                {
                    PlayerPrefs.SetInt(levelKey + "Complete", 1);
                }
                else
                {
                    PlayerPrefs.SetInt(levelKey + "Complete", 0);
                }
            }
            if(storyShown[i])
            {
                PlayerPrefs.SetInt("Story" + i, 1);
            }
            else
            {
                PlayerPrefs.SetInt("Story" + i, 0);
            }
        }
        PlayerPrefs.Save();
    }
    static void Load()
    {
        levelsCompleted = new BitArray(60, false);
        storyShown = new BitArray(6, false);
        worlds = new World[6];
        for(int i=0; i<6; ++i)
        {
            worlds[i].NumberOfStars = 0;
            worlds[i].Levels = new int[10];
            for(int j=0; j<10; ++j)
            {
                string levelKey = "World" + i + "Level" + j;
                if (PlayerPrefs.HasKey(levelKey))
                {
                    totalStars += worlds[i].Levels[j] = PlayerPrefs.GetInt(levelKey);
                }
                if(PlayerPrefs.HasKey(levelKey + "Complete"))
                {
                    levelsCompleted[j + i * 10] = PlayerPrefs.GetInt(levelKey + "Complete") == 1 ? true : false;
                }
            }
            if(PlayerPrefs.HasKey("Story" + i))
            {
                storyShown[i] = PlayerPrefs.GetInt("Story" + i) == 1 ? true : false;
            }
        }
    }
        
    // Main menu debugging buttons
    static public void AddBonus()
    {
        levelsCompleted = new BitArray(60, true);
        storyShown = new BitArray(6, true);
        worlds = new World[6];
        for (int i = 0; i < 6; ++i)
        {
            worlds[i].Levels = new int[10];
            for(int j=0; j<10; ++j)
            {
                SubmitScore("level" + (i * 10 + j + 1).ToString(), 3);
            }
        }
        Commit();
    }
}
