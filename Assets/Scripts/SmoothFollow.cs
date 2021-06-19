using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothFollow : MonoBehaviour
{
    // The target we are following
    private Transform m_Target;
    // The distance in the x-z plane to the target
    public float m_Distance = 10.0f;
    // the height we want the camera to be above the target
    public float m_Height = 5.0f;
    // How much we 
    public float m_HeightDamping = 2.0f;
    public float m_RotationDamping = 3.0f;

    // Place the script in the Camera-Control group in the component menu
    [AddComponentMenu("Camera-Control/Smooth Follow")]
    
    void Start()
    {
        m_Target = transform.parent;
        transform.parent = null;
    }

    void LateUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, m_Target.position, 10f * Time.deltaTime);

    }

    void LagFollow()
    {
        // Early out if we don't have a target
        if (!m_Target) return;

        // Calculate the current rotation angles
        float wantedRotationAngle = m_Target.eulerAngles.y;
        float wantedHeight = m_Target.position.y + m_Height;

        float currentRotationAngle = transform.eulerAngles.y;
        float currentHeight = transform.position.y;

        // Damp the rotation around the y-axis
        currentRotationAngle = Mathf.LerpAngle(currentRotationAngle, wantedRotationAngle, m_RotationDamping * Time.deltaTime);

        // Damp the height
        currentHeight = Mathf.Lerp(currentHeight, wantedHeight, m_HeightDamping * Time.deltaTime);

        // Convert the angle into a rotation
        var currentRotation = Quaternion.Euler(0, currentRotationAngle, 0);

        // Set the position of the camera on the x-z plane to:
        // distance meters behind the target
        transform.position = m_Target.position;
        transform.position -= currentRotation * Vector3.forward * m_Distance;

        // Set the height of the camera
        transform.position = new Vector3(transform.position.x, currentHeight, transform.position.z);

        // Always look at the target
        transform.LookAt(m_Target);
    }
}
