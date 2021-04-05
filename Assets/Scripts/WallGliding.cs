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
    // courbe pour la speed de la rotation d'entrée dans le wallglide
    [SerializeField] private AnimationCurve m_AnimationRotationSpeedWallGlide;
    // courbe pour la speed de la rotation de sortie du wallglide
    [SerializeField] private AnimationCurve m_AnimationRotationBackSpeedWallGlide;

    private bool m_IsAnimationBackRotationRight = false;
    private bool m_IsAnimationBackRotationLeft = false;

    private Vector3 m_WallGlideGravity = Vector3.zero;

    private void Update()
    {
        // on fait cette animation dans l'update pour être sur que le perso reprenne sa rotation même si il sort du glide
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
        // si un des raycast est vrai et que le player n'est pas au sol déclenche le start de wallglide correspondant à un des côtés
        if(m_TouchingWallRight && !p_Controller.isGrounded) { StartWallGlideRight(); }
        if(m_TouchingWallLeft && !p_Controller.isGrounded) { StartWallGlideLeft(); }
    }

    private void StartWallGlideRight()
    {
        m_IsWallGLiding = true;
        // fait en sorte d'arrêter les animations de retour de wallglide
        m_IsAnimationBackRotationRight = false;
        m_IsAnimationBackRotationLeft = false;
    }

    private void StartWallGlideLeft()
    {
        m_IsWallGLiding = true;
        // fait en sorte d'arrêter les animations de retour de wallglide
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
            // ajout à la valeur de lerp la speed de la rotation par rapport au temps
            m_RotationLerpValueWallGlide += m_RotationSpeedWallGlide * Time.deltaTime;
            // clamp de la valeur de lerp
            m_RotationLerpValueWallGlide = Mathf.Clamp(m_RotationLerpValueWallGlide, 0, 1);

            if (m_TouchingWallRight)
            {
                //Rotate le player sur le côté grâce à un lerp utilisant une courbe avec la speed de la rotation
                m_PlayerGraphicVisual.transform.localRotation = Quaternion.AngleAxis(Mathf.Lerp(0, 60, m_AnimationRotationSpeedWallGlide.Evaluate(m_RotationLerpValueWallGlide)), Vector3.forward);

                // check pour chaque angle si il est inférieur à chaque autre angle pour rentrer 
                // si on rentre dedans, ajuste le forward avec le sens du mur dans la direction du joueur 
                if(Vector3.Angle(transform.forward, m_HitRight.transform.forward) < Vector3.Angle(transform.forward, -m_HitRight.transform.forward) 
                    && Vector3.Angle(transform.forward, m_HitRight.transform.forward) < Vector3.Angle(transform.right, m_HitRight.transform.forward) 
                    && Vector3.Angle(transform.forward, m_HitRight.transform.forward) < Vector3.Angle(transform.right, -m_HitRight.transform.forward))
                {
                    transform.forward = m_HitRight.transform.forward;
                    Debug.Log("aaa");
                }
                else if (Vector3.Angle(transform.forward, -m_HitRight.transform.forward) < Vector3.Angle(transform.forward, m_HitRight.transform.forward) 
                    && Vector3.Angle(transform.forward, -m_HitRight.transform.forward) < Vector3.Angle(transform.right, m_HitRight.transform.forward) 
                    && Vector3.Angle(transform.forward, -m_HitRight.transform.forward) < Vector3.Angle(transform.right, -m_HitRight.transform.forward))
                {
                    transform.forward = -m_HitRight.transform.forward;
                    Debug.Log("bbb");
                }
                else if (Vector3.Angle(transform.right, m_HitRight.transform.forward) < Vector3.Angle(transform.forward, m_HitRight.transform.forward) 
                    && Vector3.Angle(transform.right, m_HitRight.transform.forward) < Vector3.Angle(transform.forward, -m_HitRight.transform.forward) 
                    && Vector3.Angle(transform.right, m_HitRight.transform.forward) < Vector3.Angle(transform.right, -m_HitRight.transform.forward))
                {
                    transform.forward = -m_HitRight.transform.right;
                    Debug.Log("ccc");
                }
                else if (Vector3.Angle(transform.right, -m_HitRight.transform.forward) < Vector3.Angle(transform.forward, m_HitRight.transform.forward) 
                    && Vector3.Angle(transform.right, -m_HitRight.transform.forward) < Vector3.Angle(transform.forward, -m_HitRight.transform.forward) 
                    && Vector3.Angle(transform.right, -m_HitRight.transform.forward) < Vector3.Angle(transform.right, m_HitRight.transform.forward))
                {
                    transform.forward = m_HitRight.transform.right;
                    Debug.Log("ddd");
                }
                else
                {
                    Debug.LogWarning("NANI");
                }

                WallJump();
            }
            else if (m_TouchingWallLeft)
            {
                //Rotate le player sur le côté grâce à un lerp utilisant une courbe avec la speed de la rotation
                m_PlayerGraphicVisual.transform.localRotation = Quaternion.AngleAxis(Mathf.Lerp(0, -60, m_AnimationRotationSpeedWallGlide.Evaluate(m_RotationLerpValueWallGlide)), Vector3.forward);

                // check pour chaque angle si il est inférieur à chaque autre angle pour rentrer 
                // si on rentre dedans, ajuste le forward avec le sens du mur dans la direction du joueur 
                if (Vector3.Angle(transform.forward, m_HitLeft.transform.forward) < Vector3.Angle(transform.forward, -m_HitLeft.transform.forward)
                    && Vector3.Angle(transform.forward, m_HitLeft.transform.forward) < Vector3.Angle(transform.right, m_HitLeft.transform.forward)
                    && Vector3.Angle(transform.forward, m_HitLeft.transform.forward) < Vector3.Angle(transform.right, -m_HitLeft.transform.forward))
                {
                    transform.forward = m_HitLeft.transform.forward;
                    Debug.Log("aaa");
                }
                else if (Vector3.Angle(transform.forward, -m_HitLeft.transform.forward) < Vector3.Angle(transform.forward, m_HitLeft.transform.forward)
                    && Vector3.Angle(transform.forward, -m_HitLeft.transform.forward) < Vector3.Angle(transform.right, m_HitLeft.transform.forward)
                    && Vector3.Angle(transform.forward, -m_HitLeft.transform.forward) < Vector3.Angle(transform.right, -m_HitLeft.transform.forward))
                {
                    transform.forward = -m_HitLeft.transform.forward;
                    Debug.Log("bbb");
                }
                else if (Vector3.Angle(transform.right, m_HitLeft.transform.forward) < Vector3.Angle(transform.forward, m_HitLeft.transform.forward)
                    && Vector3.Angle(transform.right, m_HitLeft.transform.forward) < Vector3.Angle(transform.forward, -m_HitLeft.transform.forward)
                    && Vector3.Angle(transform.right, m_HitLeft.transform.forward) < Vector3.Angle(transform.right, -m_HitLeft.transform.forward))
                {
                    transform.forward = -m_HitLeft.transform.right;
                    Debug.Log("ccc");
                }
                else if (Vector3.Angle(transform.right, -m_HitLeft.transform.forward) < Vector3.Angle(transform.forward, m_HitLeft.transform.forward)
                    && Vector3.Angle(transform.right, -m_HitLeft.transform.forward) < Vector3.Angle(transform.forward, -m_HitLeft.transform.forward)
                    && Vector3.Angle(transform.right, -m_HitLeft.transform.forward) < Vector3.Angle(transform.right, m_HitLeft.transform.forward))
                {
                    transform.forward = m_HitLeft.transform.right;
                    Debug.Log("ddd");
                }
                else
                {
                    Debug.LogWarning("NANI");
                }

                WallJump();
            }
            else
                EndWallGlide(); 
        }
    }

    private void AnimationBackRotation()
    {
        if (m_IsAnimationBackRotationRight)
        {
            // Décrementation de la valeur par rapport au temps 
            m_RotationLerpValueWallGlide -= m_RotationSpeedWallGlide * Time.deltaTime;
            m_RotationLerpValueWallGlide = Mathf.Clamp(m_RotationLerpValueWallGlide, 0, 1);
            // rotation du personnage dans sa rotation initial grâce à un lerp utilisant une courbe avec la speed de la rotation
            m_PlayerGraphicVisual.transform.localRotation = Quaternion.Euler(Vector3.Lerp(Vector3.zero, new Vector3(0, 0, 60), m_AnimationRotationBackSpeedWallGlide.Evaluate(m_RotationLerpValueWallGlide * 1.5f)));
        }
        else if (m_IsAnimationBackRotationLeft)
        {
            // Décrementation de la valeur par rapport au temps 
            m_RotationLerpValueWallGlide -= m_RotationSpeedWallGlide * Time.deltaTime;
            m_RotationLerpValueWallGlide = Mathf.Clamp(m_RotationLerpValueWallGlide, 0, 1);
            // rotation du personnage dans sa rotation initial grâce à un lerp utilisant une courbe avec la speed de la rotation
            m_PlayerGraphicVisual.transform.localRotation = Quaternion.Euler(Vector3.Lerp(Vector3.zero, new Vector3(0, 0, -60), m_AnimationRotationBackSpeedWallGlide.Evaluate(m_RotationLerpValueWallGlide * 1.5f)));
        }

        // si la valeur du lerp est égale où inférieur à 0 on arrête rotation inverse
        if (m_RotationLerpValueWallGlide <= 0)
        {
            m_IsAnimationBackRotationRight = false;
            m_IsAnimationBackRotationLeft = false;
        }
    }

    private void WallJump()
    {
        // si on appuie sur le bouton de saut on saute selon sur quel mur on est 
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
        // deux vecteur partant chacun d'un côté du player qui se return sur un bool
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