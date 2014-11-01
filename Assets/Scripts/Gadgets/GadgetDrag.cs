using UnityEngine;
using System.Collections;

public class GadgetDrag : MonoBehaviour 
{    
    public BoxCollider TouchCollider;
    public GameObject PlatformImplementation;
    public GameObject ReturnPoof;
    static public bool InDrag = false;

    bool allowDrag = true;
    bool drag = false;
    float timeSinceTap;
    Vector3 previousPosition;
    tk2dSprite sprite;

	void Start () 
    {
        sprite = GetComponent<tk2dSprite>();        
	}    	
	void Update () 
    {
        // Scale the touch collider so it is easier to touch zoomed out
        TouchCollider.gameObject.transform.localScale = (Vector3.up + Vector3.right) * Camera.main.orthographicSize * 0.2f + Vector3.forward;

        // Drag on touch input
        if (Menu.State == GameMenuState.Closed || Menu.State == GameMenuState.Planning)
        {
            if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android)
            {
                if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began && !InDrag)
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.touches[0].position); RaycastHit hit;
                    if (TouchCollider.Raycast(ray, out hit, float.PositiveInfinity))
                    {
                        // Return if gadget was double tapped
                        if (timeSinceTap < 0.3f && timeSinceTap > 0f)
                        {
                            Camera.main.gameObject.BroadcastMessage("AddGadgetBack", name);
                            Destroy(Instantiate(ReturnPoof, transform.position, Quaternion.identity), 1f);
                            Destroy(gameObject);
                            return;
                        }
                        else
                        {
                            timeSinceTap = 0f;
                        }
                        Time.timeScale = 0.25f;
                        InDrag = drag = true;
                        iTween.Stop(gameObject);
                        previousPosition = Input.mousePosition;
                        sprite.color = new Color(1f, 1f, 1f, 0.5f);
                    }
                }
                if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Ended && drag)
                {
                    Time.timeScale = 1f;
                    InDrag = drag = false;
                    iTween.MoveBy(gameObject, iTween.Hash(
                    "y", 0.1f,
                    "time", 2f,
                    "easetype", iTween.EaseType.linear,
                    "ignoretimescale", true,
                    "looptype", iTween.LoopType.pingPong));
                    sprite.color = new Color(1f, 1f, 1f, 1f);
                    AudioManager.Play("Gadget Place");

                    Ray ray = Camera.main.ScreenPointToRay(Input.touches[0].position); RaycastHit hit;
                    if (GameObject.Find("Gadget GUI").collider.Raycast(ray, out hit, 1000f))
                    {
                        Camera.main.gameObject.BroadcastMessage("AddGadgetBack", name);
                        Destroy(gameObject);
                        return;
                    }
                }
            }
            else // Computer input
            {
                if (Input.GetMouseButtonDown(0) && !InDrag)
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); RaycastHit hit;
                    if (TouchCollider.Raycast(ray, out hit, float.PositiveInfinity))
                    {
                        // Return if gadget was double tapped
                        if (timeSinceTap < 0.35f && timeSinceTap > 0f)
                        {
                            Camera.main.gameObject.BroadcastMessage("AddGadgetBack", name);
                            Instantiate(ReturnPoof, transform.position, Quaternion.identity);
                            Destroy(gameObject);
                            return;
                        }
                        else
                        {
                            timeSinceTap = 0f;
                        }
                        Time.timeScale = 0.25f;
                        InDrag = drag = true;
                        iTween.Stop(gameObject);
                        previousPosition = Input.mousePosition;
                        sprite.color = new Color(1f, 1f, 1f, 0.5f);
                    }
                }
                if (Input.GetMouseButtonUp(0) && drag)
                {
                    Time.timeScale = 1f;
                    InDrag = drag = false;
                    iTween.MoveBy(gameObject, iTween.Hash(
                    "y", 0.1f,
                    "time", 2f,
                    "easetype", iTween.EaseType.linear,
                    "ignoretimescale", true,
                    "looptype", iTween.LoopType.pingPong));                    
                    sprite.color = new Color(1f, 1f, 1f, 1f);
                    AudioManager.Play("Gadget Place");

                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); RaycastHit hit;
                    if (GameObject.Find("Gadget GUI").collider.Raycast(ray, out hit, 1000f))
                    {
                        Camera.main.gameObject.BroadcastMessage("AddGadgetBack", name);
                        Destroy(gameObject);
                        return;
                    }
                }
            }
        
            if (drag &&  allowDrag)
            {
                MoveGadget();
            }
        }// Menu State
        timeSinceTap += Time.deltaTime;
	}   
    
    void MoveGadget()
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android)
        {
            Vector3 touchDelta = Camera.main.ScreenToWorldPoint(new Vector3(Input.touches[0].position.x, Input.touches[0].position.y)) - Camera.main.ScreenToWorldPoint(previousPosition);
            touchDelta.z = 0f; 
            transform.Translate(touchDelta);
            previousPosition = new Vector3(Input.touches[0].position.x, Input.touches[0].position.y);

            // Scroll when platform is dragged to the edge of view
            Vector2 normalizedTouchPosition = new Vector2(Input.touches[0].position.x / Screen.width, Input.touches[0].position.y / Screen.height);
            Vector3 cameraTranslate = Vector3.zero;
            if (normalizedTouchPosition.x < 0.1f) cameraTranslate += new Vector3(-0.2f, 0f);
            if (normalizedTouchPosition.x > 0.9f) cameraTranslate += new Vector3(0.2f, 0f);
            if (normalizedTouchPosition.y < 0.1f) cameraTranslate += new Vector3(0f, -0.2f);
            if (normalizedTouchPosition.y > 0.9f) cameraTranslate += new Vector3(0f, 0.2f);
            Vector3 prevCameraPosition = Camera.main.transform.position;
            Camera.main.transform.Translate(cameraTranslate);
            Camera.main.gameObject.SendMessage("CheckCameraBounds");
            transform.Translate(Camera.main.transform.position - prevCameraPosition);
        }
        else
        {
            Vector3 mouseDelta = Camera.main.ScreenToWorldPoint(Input.mousePosition) - Camera.main.ScreenToWorldPoint(previousPosition);
            mouseDelta.z = 0f; 
            transform.Translate(mouseDelta);
            previousPosition = Input.mousePosition;

            // Scroll when platform is dragged to the edge of view
            Vector2 normalizedMousePosition = new Vector2(Input.mousePosition.x / Screen.width, Input.mousePosition.y / Screen.height);
            Vector3 cameraTranslate = Vector3.zero;
            if (normalizedMousePosition.x < 0.1f) cameraTranslate += new Vector3(-0.2f, 0f);
            if (normalizedMousePosition.x > 0.9f) cameraTranslate += new Vector3(0.2f, 0f);
            if (normalizedMousePosition.y < 0.1f) cameraTranslate += new Vector3(0f, -0.2f);
            if (normalizedMousePosition.y > 0.9f) cameraTranslate += new Vector3(0f, 0.2f);
            Vector3 prevCameraPosition = Camera.main.transform.position;
            Camera.main.transform.Translate(cameraTranslate);
            Camera.main.gameObject.SendMessage("CheckCameraBounds");
            transform.Translate(Camera.main.transform.position - prevCameraPosition);  
        }
        
    }
    public bool GetDrag()
    {
        return drag;
    }
    public void SetAllowDrag(bool val)
    {
        allowDrag = val;
    }
}