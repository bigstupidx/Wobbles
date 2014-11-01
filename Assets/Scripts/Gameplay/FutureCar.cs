using UnityEngine;
using System.Collections;

public class FutureCar : MonoBehaviour
{
    float speed;
    Rect bounds;
    tk2dSprite sprite;

    void Start()
    {
        sprite = GetComponent<tk2dSprite>();
        bounds = Camera.main.GetComponent<CameraControl>().CameraBounds;
        StartCoroutine(Randomize(5f * Random.value));
    }

    void Update()
    {
        transform.Translate(Vector3.right * Time.deltaTime * speed);
        if (transform.position.x < bounds.xMin - 5f || transform.position.x > bounds.xMax + 5f)
        {
            StartCoroutine(Randomize(5f * Random.value));
        }
    }

    IEnumerator Randomize(float seconds)
    {
        
        float y = bounds.yMin + bounds.height * Random.value;
        speed = 0f;
        if(Random.Range(0, 1000) % 2 == 0)
        {
            transform.position = new Vector3(bounds.xMax + 4f, y, transform.position.z);            
        }
        else
        {
            transform.position = new Vector3(bounds.xMin - 5f, y, transform.position.z);
        }
        yield return new WaitForSeconds(seconds);
        speed = 3f + 5f * Random.value;
        if (transform.position.x > Camera.main.transform.position.x)
        {
            speed = -speed;
        }
        if (speed > 0f)
        {
            sprite.scale = new Vector3(-1f, 1f, 1f);
        }
        else
        {
            sprite.scale = Vector3.one;
        }
        sprite.scale *= Mathf.Abs(speed) * 0.125f;
    }
}
