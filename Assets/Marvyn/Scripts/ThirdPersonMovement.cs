using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class ThirdPersonMovement : MonoBehaviour
{
    [SerializeField]
    private CharacterController m_Controller;
    [SerializeField]
    private Transform m_Camera;

    private float m_Speed = 6f;
    private float m_TurnSmoothTime = 0.1f;
    private float m_TurnSmoothVelocity;

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;
        
        if(direction.magnitude >= 0.1f)
        {
            float l_TargetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + m_Camera.eulerAngles.y;
            float l_Angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, l_TargetAngle, ref m_TurnSmoothVelocity, m_TurnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, l_Angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, l_TargetAngle, 0f) * Vector3.forward;
            m_Controller.Move(moveDir.normalized * m_Speed * Time.deltaTime);
        }
    }
}
