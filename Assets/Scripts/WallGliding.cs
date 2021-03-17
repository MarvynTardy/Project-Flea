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

    private float m_RotationLerpValueWallGlide = 0;
    [SerializeField] private float m_RotationSpeedWallGlide = 0.5f;
    [SerializeField] private AnimationCurve m_AnimationRotationSpeedWallGlide;

    private bool m_IsAnimationBackRotationRight = false;
    private bool m_IsAnimationBackRotationLeft = false;

    private Vector3 m_WallGlideGravity = Vector3.zero;

    private void Update()
    {
        AnimationBackRotation();
    }

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
        m_IsAnimationBackRotationRight = false;
        m_IsAnimationBackRotationLeft = false;
    }

    private void StartWallGlideLeft()
    {
        m_IsWallGLiding = true;
        m_IsAnimationBackRotationRight = false;
        m_IsAnimationBackRotationLeft = false;
    }

    public void EndWallGlide()
    {
        m_IsWallGLiding = false;
        //m_PlayerGraphicVisual.transform.localRotation = Quaternion.Euler(0, 0, 0);
        if(m_PlayerGraphicVisual.transform.localRotation.z > 0)
            m_IsAnimationBackRotationRight = true;
        else if (m_PlayerGraphicVisual.transform.localRotation.z < 0)
            m_IsAnimationBackRotationLeft = true;
        m_WallGlideGravity = Vector3.zero;
    }

    private void WallGlide(CharacterController p_Controller)
    {
        if(m_IsWallGLiding)
        {
            m_RotationLerpValueWallGlide += m_RotationSpeedWallGlide * Time.deltaTime;
            m_RotationLerpValueWallGlide = Mathf.Clamp(m_RotationLerpValueWallGlide, 0, 1);

            if (m_TouchingWallRight)
            {
                m_PlayerGraphicVisual.transform.localRotation = Quaternion.Euler(Vector3.Lerp(Vector3.zero, new Vector3(0, 0, 60), m_AnimationRotationSpeedWallGlide.Evaluate(m_RotationLerpValueWallGlide)));
                if (Vector3.Angle(m_HitRight.transform.forward, transform.forward) < 90 && Vector3.Angle(m_HitRight.transform.forward, transform.forward) > -90)
                    transform.forward = m_HitRight.transform.forward;
                else
                    transform.forward = -m_HitRight.transform.forward;

                WallJump();
                if (Input.GetAxisRaw("Horizontal") < 0)
                {
                    EndWallGlide();
                }
                m_WallGlideGravity = new Vector3(150 * Time.deltaTime, 0, 0);
            }
            else if (m_TouchingWallLeft)
            {
                m_PlayerGraphicVisual.transform.localRotation = Quaternion.Euler(Vector3.Lerp(Vector3.zero, new Vector3(0, 0, -60), m_AnimationRotationSpeedWallGlide.Evaluate(m_RotationLerpValueWallGlide)));
                if (Vector3.Angle(m_HitLeft.transform.forward, transform.forward) < 90 && Vector3.Angle(m_HitLeft.transform.forward, transform.forward) > -90)
                    transform.forward = m_HitLeft.transform.forward;
                else
                    transform.forward = -m_HitLeft.transform.forward;

                WallJump();
                if (Input.GetAxisRaw("Horizontal") > 0)
                {
                    EndWallGlide();
                }
                m_WallGlideGravity = new Vector3(-150 * Time.deltaTime, 0, 0);
            }
            else
                EndWallGlide(); 
        }
    }

    private void AnimationBackRotation()
    {
        if (m_IsAnimationBackRotationRight)
        {
            m_RotationLerpValueWallGlide -= m_RotationSpeedWallGlide * Time.deltaTime;
            m_RotationLerpValueWallGlide = Mathf.Clamp(m_RotationLerpValueWallGlide, 0, 1);
            m_PlayerGraphicVisual.transform.localRotation = Quaternion.Euler(Vector3.Lerp(Vector3.zero, new Vector3(0, 0, 60), m_AnimationRotationSpeedWallGlide.Evaluate(m_RotationLerpValueWallGlide * 1.5f)));
        }
        else if (m_IsAnimationBackRotationLeft)
        {
            m_RotationLerpValueWallGlide -= m_RotationSpeedWallGlide * Time.deltaTime;
            m_RotationLerpValueWallGlide = Mathf.Clamp(m_RotationLerpValueWallGlide, 0, 1);
            m_PlayerGraphicVisual.transform.localRotation = Quaternion.Euler(Vector3.Lerp(Vector3.zero, new Vector3(0, 0, -60), m_AnimationRotationSpeedWallGlide.Evaluate(m_RotationLerpValueWallGlide * 1.5f)));
        }

        if (m_RotationLerpValueWallGlide <= 0)
        {
            m_IsAnimationBackRotationRight = false;
            m_IsAnimationBackRotationLeft = false;
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
    }

    public Vector3 WallGlideGravity
    {
        get { return m_WallGlideGravity; }
    }

    public Vector3 WallJumpPower
    {
        get { return m_WallJumpPower;  }
        set { m_WallJumpPower = value; }
    }
}