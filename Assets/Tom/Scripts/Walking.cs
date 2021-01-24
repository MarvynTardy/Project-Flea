using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Walking : MonoBehaviour
{
    [Header("VariablesWalk")]
    [SerializeField]
    private float m_Speed = 6f;
    [SerializeField]
    private float m_TurnSmoothTime = 0.5f;
    [SerializeField]
    private float m_TurnSmoothVelocity;
    [SerializeField]
    private float m_Gravity = -9.81f;

    Vector3 m_Velocity = Vector3.zero;

    public void Walk(Transform p_Camera, CharacterController p_Controller)
    {
        float l_Horizontal = Input.GetAxisRaw("Horizontal");
        float l_Vertical = Input.GetAxisRaw("Vertical");
        Vector3 l_Direction = new Vector3(l_Horizontal, 0f, l_Vertical).normalized;

        if (l_Direction.magnitude >= 0.1f)
        {
            float l_TargetAngle = Mathf.Atan2(l_Direction.x, l_Direction.z) * Mathf.Rad2Deg + p_Camera.eulerAngles.y;
            float l_Angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, l_TargetAngle, ref m_TurnSmoothVelocity, m_TurnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, l_Angle, 0f);

            Vector3 l_MoveDir = Quaternion.Euler(0f, l_TargetAngle, 0f) * Vector3.forward;
            p_Controller.Move(l_MoveDir.normalized * m_Speed * Time.deltaTime);
        }
    }
}
