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

    private RaycastHit m_HitRight;
    private RaycastHit m_HitLeft;

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
        if(Input.GetAxisRaw("Horizontal") > 0 && m_TouchingWallRight) { StartWallGlideRight(); }
        if(Input.GetAxisRaw("Horizontal") < 0 && m_TouchingWallLeft) { StartWallGlideLeft(); }
    }

    private void StartWallGlideRight()
    {
        m_IsWallGLiding = true;
        m_PlayerGraphicVisual.transform.localRotation = Quaternion.Euler(0, 0, 60f);
    }

    private void StartWallGlideLeft()
    {
        m_IsWallGLiding = true;
        m_PlayerGraphicVisual.transform.localRotation = Quaternion.Euler(0, 0, -60f);
    }

    public void EndWallGlide()
    {
        m_IsWallGLiding = false;
        m_PlayerGraphicVisual.transform.localRotation = Quaternion.Euler(0, 0, 0);
    }

    private void WallGlide(CharacterController p_Controller)
    {
        if(m_IsWallGLiding)
        {

            if(m_TouchingWallRight)
            {
                if (Vector3.Angle(m_HitRight.transform.forward, transform.forward) < 90 && Vector3.Angle(m_HitRight.transform.forward, transform.forward) > -90)
                    transform.forward = m_HitRight.transform.forward;
                else
                    transform.forward = -m_HitRight.transform.forward;

                WallJump();
                if (Input.GetAxisRaw("Horizontal") == 0)
                {
                    EndWallGlide();
                }
            }
            else if (m_TouchingWallLeft)
            {
                if (Vector3.Angle(m_HitLeft.transform.forward, transform.forward) < 90 && Vector3.Angle(m_HitLeft.transform.forward, transform.forward) > -90)
                    transform.forward = m_HitLeft.transform.forward;
                else
                    transform.forward = -m_HitLeft.transform.forward;

                WallJump();
                if (Input.GetAxisRaw("Horizontal") == 0)
                {
                    EndWallGlide();
                }
            }
            else
                EndWallGlide(); 
        }
    }

    private void WallJump()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            if (m_TouchingWallRight)
            {
                m_WallJumpPower = (-transform.right + transform.up).normalized * m_WallJumpForce * 3;
            }
            else if (m_TouchingWallLeft)
            {
                m_WallJumpPower = (transform.right + transform.up).normalized * m_WallJumpForce * 3;
            }
        }
    }

    private void DetectionWall()
    {
        m_TouchingWallRight = Physics.Raycast(transform.position, transform.right, out m_HitRight, 0.75f, m_GlideableWall);
        m_TouchingWallLeft = Physics.Raycast(transform.position, -transform.right, out m_HitLeft, 0.75f, m_GlideableWall);
        /*if(Input.GetKeyDown(KeyCode.L))
        {
            Vector3 l_Forward = transform.forward;
            if(Vector3.Angle(m_HitRight.transform.forward, transform.forward) < 90 && Vector3.Angle(m_HitRight.transform.forward, transform.forward) > -90)
                transform.forward = m_HitRight.transform.forward;
            else
                transform.forward = -m_HitRight.transform.forward;
        }*/
    }

    public Vector3 WallJumpPower
    {
        get { return m_WallJumpPower;  }
        set { m_WallJumpPower = value; }
    }
}