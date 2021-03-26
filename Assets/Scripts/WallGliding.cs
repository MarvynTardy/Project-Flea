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
    [SerializeField] private AnimationCurve m_AnimationRotationBackSpeedWallGlide;

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
        WallGlideInput(p_Controller);
        WallGlide(p_Controller);
    }

    public bool IsWallGliding()
    {
        return m_IsWallGLiding;
    }

    private void WallGlideInput(CharacterController p_Controller)
    {
        if(m_TouchingWallRight && !p_Controller.isGrounded) { StartWallGlideRight(); }
        if(m_TouchingWallLeft && !p_Controller.isGrounded) { StartWallGlideLeft(); }
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
                //m_PlayerGraphicVisual.transform.localRotation = Quaternion.Euler(Vector3.Lerp(Vector3.zero, new Vector3(0, 0, 60), m_AnimationRotationSpeedWallGlide.Evaluate(m_RotationLerpValueWallGlide)));
                if (Vector3.Angle(transform.forward, m_HitRight.transform.forward) < Vector3.Angle(transform.forward, -m_HitRight.transform.forward))
                {
                    if (Vector3.Angle(transform.forward, m_HitRight.transform.forward) < Vector3.Angle(transform.right, m_HitRight.transform.forward))
                    {
                        if (Vector3.Angle(m_HitRight.transform.forward, transform.forward) < 90)
                            transform.forward = m_HitRight.transform.forward;
                        else
                        {
                            Debug.Log(transform.forward);
                            Debug.Log(m_HitRight.transform.forward);
                            transform.forward = -m_HitRight.transform.forward;
                        }
                    }
                    else
                    {
                        Debug.Log("aaa");
                        if (Vector3.Angle(m_HitRight.transform.forward, -transform.right) < 90)
                            transform.forward = m_HitRight.transform.right;
                        /*Debug.Log(Vector3.Angle(m_HitRight.transform.forward, transform.right));*/
                        else
                        {
                            //Debug.Log(transform.forward);
                            //Debug.Log(m_HitRight.transform.localRotation);
                            //transform.localRotation = Quaternion.AngleAxis(0, Vector3.up);
                            transform.forward = -m_HitRight.transform.right;
                            //transform.localRotation = Quaternion.AngleAxis(90, Vector3.up);
                            //transform.localRotation = Quaternion.Euler(transform.localRotation.x, -90, transform.localRotation.z);
                            //Debug.Break();
                            Debug.Log(transform.forward);
                        }
                    }
                }
                else
                {
                    if (Vector3.Angle(transform.forward, -m_HitRight.transform.forward) < Vector3.Angle(transform.right, -m_HitRight.transform.forward))
                    {
                        if (Vector3.Angle(m_HitRight.transform.forward, transform.forward) < 90)
                            transform.forward = m_HitRight.transform.forward;
                        else
                        {
                            //là
                            transform.forward = -m_HitRight.transform.forward;
                        }
                    }
                    else
                    {
                        Debug.Log("bbb");
                        if (Vector3.Angle(m_HitRight.transform.forward, transform.right) < 90)
                            transform.forward = m_HitRight.transform.right;
                        else
                            transform.forward = -m_HitRight.transform.right;
                    }
                }

                WallJump();
                /*if (Input.GetAxisRaw("Horizontal") < 0)
                {
                    EndWallGlide();
                }*/
                //m_WallGlideGravity = new Vector3(150 * Time.deltaTime, 0, 0);
            }
            else if (m_TouchingWallLeft)
            {
                m_PlayerGraphicVisual.transform.localRotation = Quaternion.Euler(Vector3.Lerp(Vector3.zero, new Vector3(0, 0, -60), m_AnimationRotationSpeedWallGlide.Evaluate(m_RotationLerpValueWallGlide)));
                if (Vector3.Angle(m_HitLeft.transform.forward, transform.forward) < 90 && Vector3.Angle(m_HitLeft.transform.forward, transform.forward) > -90)
                    transform.forward = m_HitLeft.transform.forward;
                else
                    transform.forward = -m_HitLeft.transform.forward;

                WallJump();
                /*if (Input.GetAxisRaw("Horizontal") > 0)
                {
                    EndWallGlide();
                }*/
                //m_WallGlideGravity = new Vector3(-150 * Time.deltaTime, 0, 0);
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
            m_PlayerGraphicVisual.transform.localRotation = Quaternion.Euler(Vector3.Lerp(Vector3.zero, new Vector3(0, 0, 60), m_AnimationRotationBackSpeedWallGlide.Evaluate(m_RotationLerpValueWallGlide * 1.5f)));
        }
        else if (m_IsAnimationBackRotationLeft)
        {
            m_RotationLerpValueWallGlide -= m_RotationSpeedWallGlide * Time.deltaTime;
            m_RotationLerpValueWallGlide = Mathf.Clamp(m_RotationLerpValueWallGlide, 0, 1);
            m_PlayerGraphicVisual.transform.localRotation = Quaternion.Euler(Vector3.Lerp(Vector3.zero, new Vector3(0, 0, -60), m_AnimationRotationBackSpeedWallGlide.Evaluate(m_RotationLerpValueWallGlide * 1.5f)));
        }

        if (m_RotationLerpValueWallGlide <= 0)
        {
            m_IsAnimationBackRotationRight = false;
            m_IsAnimationBackRotationLeft = false;
        }
    }

    private void WallJump()
    {
        if(Input.GetButtonDown("Jump"))
        {
            if (m_TouchingWallRight)
            {
                m_WallJumpPower = (-transform.right + transform.up * 0.75f)/*.normalized*/ * m_WallJumpForce * 3;
            }
            else if (m_TouchingWallLeft)
            {
                m_WallJumpPower = (transform.right + transform.up * 0.75f)/*.normalized*/ * m_WallJumpForce * 3;
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