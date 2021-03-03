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

    public bool WallGlidingUpdate(CharacterController p_Controller)
    {
        DetectionWall();
        WallGlideInput();
        WallGlide(p_Controller);
        return m_IsWallGLiding;
    }

    private void WallGlideInput()
    {
        if(Input.GetKey(KeyCode.D) && m_TouchingWallRight) { Debug.Log("right"); StartWallGlide(); }
        if(Input.GetKey(KeyCode.Q) && m_TouchingWallLeft) { Debug.Log("left"); StartWallGlide(); }
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
                //p_Controller.Move(transform.right * 2);
                //p_Controller.Move(transform.up * 5 * Time.deltaTime);
            }
            else if (m_TouchingWallLeft)
            {
                //p_Controller.Move(-transform.right * 2);
                //p_Controller.Move(transform.up * Time.deltaTime);
            }
            else
            { Debug.Log("marche pas"); }
        }
    }

    private void DetectionWall()
    {
        m_TouchingWallRight = Physics.Raycast(transform.position, transform.right, 1f, m_GlideableWall);
        m_TouchingWallLeft = Physics.Raycast(transform.position, -transform.right, 1f, m_GlideableWall);

        if (!m_TouchingWallRight && !m_TouchingWallLeft) EndWallGlide();
    }
}