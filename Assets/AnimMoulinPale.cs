using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimMoulinPale : MonoBehaviour
{
    Vector3 rotationDirection = Vector3.up;
    public float durationTime = 1;
    private float smooth = 1;

    // Update is called once per frame
    void Update()
    {
        smooth = Time.deltaTime * durationTime;
        transform.Rotate(rotationDirection * smooth);
    }
}
