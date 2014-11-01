using UnityEngine;
using System.Collections;
using System.IO;

public class GadgetIncludeInfo
{
    public static tk2dSprite[] Thumbnails;
    public static GameObject[] Gadgets;

    public string Gadget;
    public int Amount;
}

public class TextAssetLoader : MonoBehaviour
{
    public TextAsset LevelRequirementsTxt;
    public TextAsset CameraBoundsTxt;
    public TextAsset GadgetAmountsTxt;

    public GameObject[] Gadgets;
    public tk2dSprite[] Thumbnails;

    static GadgetIncludeInfo[][] GadgetInfo = null;
    static Rect[] CameraBounds = null;    

    static public GadgetIncludeInfo[] GetIncludedGadgets(string levelName)
    {
        int world, level;
        Save.GetLevelIndices(levelName, out world, out level);
        return GadgetInfo[world * 10 + level];
    }
    static public Rect GetCameraBounds(string levelName)
    {
        int world, level;
        Save.GetLevelIndices(levelName, out world, out level);
        return CameraBounds[world * 10 + level];
    }
    void Awake()
    {
        if (GadgetInfo == null)
        {
            ParseGadgetInfo();
            ParseCameraInfo();
            ParseRequirementInfo();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void ParseGadgetInfo()
    {
        GadgetIncludeInfo.Thumbnails = Thumbnails;
        GadgetIncludeInfo.Gadgets = Gadgets;
        GadgetInfo = new GadgetIncludeInfo[60][];
        ArrayList tempGadgetList = new ArrayList(8);
        using (StringReader sr = new StringReader(GadgetAmountsTxt.text))
        {
            int currentWorld = -1, currentLevel = -1;
            string s;
            while ((s = sr.ReadLine()) != null)
            {
                s = s.Trim();
                if (s.StartsWith("world") || s.StartsWith("level"))
                {
                    // Add the gadget to the static array
                    if (tempGadgetList.Count > 0)
                    {
                        GadgetInfo[currentWorld * 10 + currentLevel] = new GadgetIncludeInfo[tempGadgetList.Count];
                        for (int i = 0; i < tempGadgetList.Count; ++i)
                        {
                            GadgetInfo[currentWorld * 10 + currentLevel][i] = tempGadgetList[i] as GadgetIncludeInfo;
                        }
                        tempGadgetList.Clear();
                    }
                    if (s.StartsWith("world"))
                    {
                        currentLevel = -1;
                        ++currentWorld;
                    }
                    if (s.StartsWith("level"))
                    {
                        ++currentLevel;                     
                    }
                }
                else
                {
                    // Determine what gadget is in the current string
                    string[] sa = s.Split(':');
                    if (sa.Length == 2)
                    {
                        for (int i = 0; i < Gadgets.Length; ++i)
                        {
                            if (Gadgets[i].name.ToLower().Contains(sa[0]))
                            {
                                GadgetIncludeInfo gadget = new GadgetIncludeInfo();
                                gadget.Gadget = Gadgets[i].name;
                                //gadget.Thumbnail = Thumbnails[i];
                                int num = 0;
                                if (int.TryParse(sa[1], out num))
                                {
                                    gadget.Amount = num;
                                }
                                tempGadgetList.Add(gadget);
                            }
                        }
                    }
                }
            }
        }
    }
    void ParseCameraInfo()
    {
        CameraBounds = new Rect[60];
        using (StringReader sr = new StringReader(CameraBoundsTxt.text))
        {
            int currentWorld = -1, currentLevel = -1;
            string s; Rect tempRect = new Rect();
            while ((s = sr.ReadLine()) != null)
            {
                s = s.Trim();
                if (s.StartsWith("world") || s.StartsWith("level"))
                {
                    // Add the gadget to the static array
                    if (tempRect.width > 0f)
                    {
                        CameraBounds[currentWorld * 10 + currentLevel] = tempRect;
                        tempRect = new Rect();
                    }
                    if (s.StartsWith("world"))
                    {
                        currentLevel = -1;
                        ++currentWorld;
                    }
                    if (s.StartsWith("level"))
                    {
                        ++currentLevel;
                    }
                }
                else
                {
                    // Parses the individual bounds             
                    string[] sa = s.Split(':');
                    float num;
                    if (sa.Length >= 1 && float.TryParse(sa[1], out num))
                    {
                        switch (s[0])
                        {
                            case 'x':
                                tempRect.x = num;
                                break;
                            case 'y':
                                tempRect.y = num;
                                break;
                            case 'w':
                                tempRect.width = num;
                                break;
                            case 'h':
                                tempRect.height = num;
                                break;
                            default:
                                print("Parsing error.");
                                break;
                        }
                    }
                }
            }
        }
    }
    void ParseRequirementInfo()
    {
        int[] requirements = new int[6];
        using (StringReader sr = new StringReader(LevelRequirementsTxt.text))
        {
            int world = 0; string s;
            while ((s = sr.ReadLine()) != null)
            {
                if (s.StartsWith("world"))
                {
                    int num;
                    string[] sa = s.Split(':');
                    if (sa.Length == 2 && int.TryParse(sa[1], out num))
                    {
                        requirements[world] = num;
                    }
                    else
                    {
                        Debug.LogError("Error parsing world string " + world);
                    }
                    ++world;
                }
            }
        }
        Save.Initialize(requirements);
    }
}
