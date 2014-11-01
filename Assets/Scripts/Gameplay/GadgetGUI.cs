using UnityEngine;
using System.Collections;

public class GadgetGUI : MonoBehaviour
{
    public float distanceBetweenGadgets;
    public tk2dTextMesh TextSprite;
    GameObject[] Gadgets;
    tk2dSprite[] Thumbnails;
    int[] Amount;

    tk2dTextMesh[] numberText;
    tk2dSlicedSprite sliceSprite;

    // Unity functions
    void Start()
    {
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 3f);
        sliceSprite = GetComponent<tk2dSlicedSprite>();
        GenerateGUI();
    }
    void Update()
    {
        if (Menu.State == GameMenuState.Planning || Menu.State == GameMenuState.Closed)
        {
            if (Input.GetMouseButtonDown(0))
            {
                GenerateGadgets();
            }
        }                
    }

    // Helper functions
    void GenerateGUI()
    {
        Amount = new int[6];
        numberText = new tk2dTextMesh[6];
        Gadgets = GadgetIncludeInfo.Gadgets;
        Thumbnails = new tk2dSprite[6]; 

        GadgetIncludeInfo[] gii = TextAssetLoader.GetIncludedGadgets(Application.loadedLevelName);
        
        // Add their total length for spacing
        float lengthSum = distanceBetweenGadgets * 5;
        for (int i = 0; i < 6; ++i)
        {
            lengthSum +=  GadgetIncludeInfo.Thumbnails[i].GetUntrimmedBounds().size.x;
        }

        // Set the dimensions (assumes current size is sufficient for one gadget)
        //sliceSprite.dimensions = new Vector2(sliceSprite.dimensions.x * 6, sliceSprite.dimensions.y);

        // Generate the thumbnails and add to tray
        float cumulativeLength = 0f;
        for (int i = 0; i < 6; ++i)
        {
            float positionX = -(lengthSum * 0.5f) + cumulativeLength + GadgetIncludeInfo.Thumbnails[i].GetUntrimmedBounds().size.x * 0.5f;
            GameObject thumbnail = Instantiate(GadgetIncludeInfo.Thumbnails[i].gameObject, new Vector3(positionX, 0f, -0.05f) + transform.position, transform.rotation) as GameObject;
            thumbnail.transform.parent = transform;
            Thumbnails[i] = thumbnail.GetComponent<tk2dSprite>();
            Thumbnails[i].color = new Color(0.5f, 0.5f, 0.5f, 1f);
            cumulativeLength += Thumbnails[i].GetUntrimmedBounds().size.x + distanceBetweenGadgets;

            GameObject text = (Instantiate(TextSprite.gameObject, new Vector3(positionX + 0.44f, 0.5f, -0.1f) + transform.position, transform.rotation) as GameObject);
            text.transform.parent = transform;
            numberText[i] = text.GetComponent<tk2dTextMesh>();
            numberText[i].color *= 0.5f;
            numberText[i].text = "0";
            numberText[i].Commit();
        }

        foreach(GadgetIncludeInfo info in gii)
        {
            for(int i=0; i<6; i++)
            {
                if(Gadgets[i].name.StartsWith(info.Gadget))
                {
                    Amount[i] = info.Amount;
                    numberText[i].text = Amount[i].ToString();
                    numberText[i].color *= 2f;
                    numberText[i].Commit();                    
                    Thumbnails[i].color = new Color(1f, 1f, 1f, 1f);
                }
            }
        }
    }
    void GenerateGadgets()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); RaycastHit hit;
        if (collider.Raycast(ray, out hit, 40f))
        {
            // Loop through each thumbnail and tests for hit
            // Generate corresponding gadget if hit
            for (int i = 0; i < 6; ++i)
            {
                if (Thumbnails[i].GetComponent<BoxCollider>().Raycast(ray, out hit, 20f) && Amount[i] > 0)
                {
                    --Amount[i];
                    numberText[i].text = Amount[i].ToString();                    
                    if (Amount[i] <= 0)
                    {
                        numberText[i].color *= 0.5f;
                        Thumbnails[i].color = new Color(0.5f, 0.5f, 0.5f, 1f);
                    }
                    numberText[i].Commit();

                    Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    if (Gadgets[i].name != "Tunnel") position.z = -0.5f;
                    else position.z = -1.5f;
                    GameObject obj = Instantiate(Gadgets[i], position, transform.rotation) as GameObject;
                    obj.SendMessage("Start");
                    obj.SendMessage("Update");
                    break;
                }
            }
        }
    }
    void AddGadgetBack(string gadgetName)
    {
        // Adds back gadget if double clicked
        for(int i=0; i<6; ++i)
        {
            if (gadgetName.Contains(Gadgets[i].name))
            {
                ++Amount[i];
                numberText[i].text = Amount[i].ToString();
                if(Amount[i] == 1)
                {
                    numberText[i].color *= 2f;
                }
                numberText[i].Commit();
                Thumbnails[i].color = new Color(1f, 1f, 1f, 1f);
                break;
            }
        }
    }
}
