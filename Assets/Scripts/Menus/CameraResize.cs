using UnityEngine;
using System.Collections;

public class CameraResize : MonoBehaviour
{
    public GameObject LeftBorder, RightBorder;

	void Start ()
    {
        const float fourToThree = 4f/3f;
        float screenRatio = (float)Screen.width / (float)Screen.height;
        if(screenRatio < fourToThree - 0.05f || screenRatio > fourToThree + 0.05f)
        {
            float actualWidth = camera.orthographicSize * screenRatio * 2f;
            float targetWidth = camera.orthographicSize * 2f * fourToThree;
            float percent = targetWidth / actualWidth;
            float extra = 1f - percent;
            camera.rect = new Rect(extra * 0.5f, 0f, percent, 1f);

            LeftBorder.transform.localPosition = Vector3.left * targetWidth * 0.5f + Vector3.forward;
            RightBorder.transform.localPosition = Vector3.right * targetWidth * 0.5f + Vector3.forward;
        }
	}
}
