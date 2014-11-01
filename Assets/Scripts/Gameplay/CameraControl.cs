using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour
{
    public Rect CameraBounds;
    public GameObject Background;
    public GameObject OffscreenArrow;

    bool drag = false;
    bool destroyingArrow = false;
    float prevTwoTouchDist;
    float screenRatio;
    float MaxCameraOrthoSize;
    float MinCameraOrthoSize = 5f;
    Vector3 prevMousePosition;
    Vector3 prevCameraDelta;
    public Rect Debugdraw;

    // Unity defined functions
    void Start()
    {
        Debugdraw = CameraBounds;
        Rect rect = TextAssetLoader.GetCameraBounds(Application.loadedLevelName);
        Debugdraw = rect;
        CameraBounds = rect;
                
        // Figures out what the maximum zoom can be and sets it to that
        screenRatio = ((float)Screen.width / (float)Screen.height);
        float height = CameraBounds.height;
        float width = CameraBounds.width / screenRatio;
        if(height < width)
        {
            // Use height as max ortho
            MaxCameraOrthoSize = height * 0.5f;            
        }
        else
        {
            // Use width as max ortho
            MaxCameraOrthoSize = (CameraBounds.width * 0.5f) / screenRatio;            
        }
        if (MaxCameraOrthoSize < MinCameraOrthoSize)
        {
            Camera.main.orthographicSize = MinCameraOrthoSize = MaxCameraOrthoSize;
        }
        Camera.main.orthographicSize = MaxCameraOrthoSize;
        transform.localScale = (Vector3.up + Vector3.right) * Camera.main.orthographicSize * 0.2f + Vector3.forward;

        // Update the position of the background so it centers correctly
        CheckCameraBounds();
        float dx = transform.position.x - (rect.xMin + rect.width * 0.5f);
        CameraMoved(new Vector3(dx, 0f));

        float rightBound = transform.position.x + Camera.main.orthographicSize * screenRatio;
        if(rightBound > rect.xMax - 2f)
        {
            Destroy(OffscreenArrow);
        }
        else
        {
            iTween.MoveBy(OffscreenArrow, iTween.Hash(
                "x", 0.4f,
                "time", 0.8f,
                "looptype", iTween.LoopType.pingPong,
                "easetype", iTween.EaseType.linear));        
        }

    }    
    void LateUpdate()
    {
        // Dont update during pause
        if (Menu.State != GameMenuState.Closed && Menu.State != GameMenuState.Planning) return;

        Vector3 prevPosition = transform.position;
        float prevCamSize = Camera.main.orthographicSize;
        
        // We don't want to move the camera while we are moving a gadget
        if (!GadgetDrag.InDrag)
        {
            // Detect touch drag
            if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android)
            {
                if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)
                {
                    drag = true;
                } 
                if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Ended)
                {
                    drag = false;
                }
            }
            else
            {
                // Detect mouse drag
                if (Input.GetMouseButtonDown(0))
                {
                    drag = true;
                    
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    drag = false;
                }
            }            
            if (drag)
            {
                MoveCamera();
            }
            ZoomCamera();
            CameraMomentum();
            CheckCameraBounds();
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                DrawCameraBounds();
            }
        }
        CameraMoved(transform.position - prevPosition);
        if (!Mathf.Approximately(prevCamSize, Camera.main.orthographicSize))
        {
            transform.localScale = (Vector3.up + Vector3.right) * Camera.main.orthographicSize * 0.2f + Vector3.forward;
        }
        
	}

    // Helper functions
    void MoveCamera()
    {
        // Use camera orthographic size and screen ratio to move 1:1 with input
        if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android)
        {
            // Finger scroll
            if(Input.touches[0].phase == TouchPhase.Moved)
            {
                Vector3 touchDelta = Input.touches[0].deltaPosition;
                touchDelta = Camera.main.ScreenToWorldPoint(touchDelta) - Camera.main.ScreenToWorldPoint(Vector3.zero);
                transform.Translate(-touchDelta);
                prevCameraDelta = -touchDelta;
            }
        }
        else
        {
            // Mouse scroll
            if (Input.GetMouseButtonDown(0)) prevMousePosition = Input.mousePosition;
            Vector3 mouseDelta = Input.mousePosition - prevMousePosition;
            mouseDelta = Camera.main.ScreenToWorldPoint(mouseDelta) - Camera.main.ScreenToWorldPoint(Vector3.zero);
            prevMousePosition = Input.mousePosition;
            transform.Translate(-mouseDelta);
            prevCameraDelta = -mouseDelta;
        }
    }
    void ZoomCamera()
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android)
        {
            // Pinch Zoom
            if (Input.touchCount == 2)
            {
                if (Input.touches[0].phase == TouchPhase.Began || Input.touches[1].phase == TouchPhase.Began)
                {
                    prevTwoTouchDist = (Input.touches[0].position - Input.touches[1].position).magnitude;
                }
                float currentDist = (Input.touches[0].position - Input.touches[1].position).magnitude;
                float deltaDist = currentDist - prevTwoTouchDist;
                prevTwoTouchDist = currentDist;
                if (Mathf.Abs(deltaDist) > 10f)
                {
                    Camera.main.orthographicSize += (deltaDist > 0f ? -0.3f : 0.3f);
                    Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, MinCameraOrthoSize, MaxCameraOrthoSize);
                }
            }
        }
        else
        {
            // Mouse wheel zoom
            Camera.mainCamera.orthographicSize -= Input.GetAxis("Mouse ScrollWheel") * 3;
            Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, MinCameraOrthoSize, MaxCameraOrthoSize);
        }
    }
    void CheckCameraBounds()
    {
        float rightBound = transform.position.x + Camera.main.orthographicSize * screenRatio;
        float leftBound = transform.position.x - Camera.main.orthographicSize * screenRatio;
        float topBound = transform.position.y + Camera.main.orthographicSize;
        float bottomBound = transform.position.y - Camera.main.orthographicSize;
        if (rightBound > CameraBounds.xMax)
        {
            if (OffscreenArrow && !destroyingArrow)
            {
                destroyingArrow = true;
                iTween.Stop(OffscreenArrow);
                iTween.ScaleTo(OffscreenArrow, iTween.Hash(
                "scale", Vector3.zero,
                "time", 0.35f,
                "ignoretimescale", true,
                "easetype", iTween.EaseType.linear,
                "oncomplete", "DestroyArrow",
                "oncompletetarget", gameObject));
            }
            transform.position = new Vector3(CameraBounds.xMax - Camera.main.orthographicSize * screenRatio, transform.position.y, -20f);
        }
        else if (leftBound < CameraBounds.xMin)
        {
            transform.position = new Vector3(CameraBounds.xMin + Camera.main.orthographicSize * screenRatio, transform.position.y, -20f);
        }

        if (topBound > CameraBounds.yMax)
        {
            transform.position = new Vector3(transform.position.x, CameraBounds.yMax - Camera.main.orthographicSize, -20f);
        }
        else if (bottomBound < CameraBounds.yMin)
        {
            transform.position = new Vector3(transform.position.x, CameraBounds.yMin + Camera.main.orthographicSize, -20f);
        }
    }
    void DrawCameraBounds()
    {
        Vector3 topLeft = new Vector3(Debugdraw.x, Debugdraw.yMax);
        Vector3 topRight = new Vector3(Debugdraw.xMax, Debugdraw.yMax);
        Vector3 bottomLeft = new Vector3(Debugdraw.x, Debugdraw.y);
        Vector3 bottomRight = new Vector3(Debugdraw.xMax, Debugdraw.y);

        Debug.DrawLine(topLeft, topRight);
        Debug.DrawLine(topRight, bottomRight);
        Debug.DrawLine(bottomRight, bottomLeft);
        Debug.DrawLine(bottomLeft, topLeft);
    }   
    void CameraMomentum()
    {
        // Continues scrolling after release until it is slower than a set speed
        if (!drag && prevCameraDelta.sqrMagnitude > 0.0025f)
        {
            Vector3 prevCamPos = transform.position;
            prevCameraDelta *= 0.9f;
            transform.Translate(prevCameraDelta);
            CheckCameraBounds();
            CameraMoved(transform.position - prevCamPos);
        }
    }
    void CameraMoved(Vector3 amount)
    {
        // Offsets the background with scrolling for parallax effect
        Background.transform.localPosition -= (amount *= 0.1f);
    }
    void DestroyArrow()
    {
        Destroy(OffscreenArrow);
    }
}
