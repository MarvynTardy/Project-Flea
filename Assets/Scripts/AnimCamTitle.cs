using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimCamTitle : MonoBehaviour
{
    public Vector3 startPos = new Vector3(0, 0, -11);
    public Vector3 endPos = new Vector3(0, 0, 0);
    public float travelTime = 1.0f;
    public float speed;
    public bool repeatable;

    float startTime = 0;
    Vector3 centerPoint;
    Vector3 startRelCenter;
    Vector3 endRelCenter;


    void Update()
    {
        GetCenter(Vector3.up);
        if (!repeatable)
        {
            float fracComplete = (Time.time - startTime) / travelTime * speed;
            transform.localPosition = Vector3.Slerp(startRelCenter, endRelCenter, fracComplete * speed);
            transform.localPosition += centerPoint;
        }
        else
        {
            float fracComplete = Mathf.PingPong(Time.time - startTime, travelTime / speed);
            transform.localPosition = Vector3.Slerp(startRelCenter, endRelCenter, fracComplete * speed);
            transform.localPosition += centerPoint;
        }

    }

    public void GetCenter(Vector3 direction)
    {
        centerPoint = (startPos + endPos) * 0.5f;
        centerPoint -= direction;
        startRelCenter = startPos - centerPoint;
        endRelCenter = endPos - centerPoint;
    }
}
