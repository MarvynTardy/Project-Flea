using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallGliding : MonoBehaviour
{
    [SerializeField] private GameObject m_PlayerGraphicVisual = null;

    [SerializeField] private LayerMask m_GlideableWall;
    private bool m_TouchingWallRight = false;
    private bool m_TouchingWallLeft = false;

    private bool m_IsWallGLiding = false;

    private Vector3 m_WallJumpPower = Vector3.zero;

    [SerializeField] private float m_WallJumpForce = 15f;

    public void WallGlidingUpdate(CharacterController p_Controller)
    {
        DetectionWall();
        WallGlideInput();
        WallGlide(p_Controller);
    }

    public bool IsWallGliding()
    {
        return m_IsWallGLiding;
    }

    private void WallGlideInput()
    {
        if(Input.GetAxisRaw("Horizontal") > 0 && m_TouchingWallRight) { Debug.Log("right"); StartWallGlide(); }
        if(Input.GetAxisRaw("Horizontal") < 0 && m_TouchingWallLeft) { Debug.Log("left"); StartWallGlide(); }
    }

    private void StartWallGlide()
    {
        m_IsWallGLiding = true;
    }

    private void EndWallGlide()
    {
        m_IsWallGLiding = false;
    }

    private void WallGlide(CharacterController p_Controller)
    {
        if(m_IsWallGLiding)
        {
            if(m_TouchingWallRight)
            {
                WallJump();
                if(Input.GetAxisRaw("Horizontal") == 0)
                {
                    EndWallGlide();
                }
            }
            else if (m_TouchingWallLeft)
            {
                WallJump();
                if (Input.GetAxisRaw("Horizontal") == 0)
                {
                    EndWallGlide();
                }
            }
            else
            { EndWallGlide(); }
        }
    }

    private void WallJump()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            if (m_TouchingWallRight)
            {
                m_WallJumpPower = (-transform.right + transform.up).normalized * m_WallJumpForce * 3;
                Debug.Log(m_WallJumpPower);
            }
            else if (m_TouchingWallLeft)
            {
                m_WallJumpPower = (transform.right + transform.up).normalized * m_WallJumpForce * 3;
                Debug.Log(m_WallJumpPower);
            }
        }
    }

    private void DetectionWall()
    {
        m_TouchingWallRight = Physics.Raycast(transform.position, transform.right, 1f, m_GlideableWall);
        m_TouchingWallLeft = Physics.Raycast(transform.position, -transform.right, 1f, m_GlideableWall);
    }

    public Vector3 WallJumpPower
    {
        get { return m_WallJumpPower;  }
        set { m_WallJumpPower = value; }
    }
}