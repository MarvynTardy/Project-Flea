using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Walking : MonoBehaviour
{
    [Header("VariablesWalk")]
    [SerializeField]
    // Speed qui sert de référence 
    private float m_TrueSpeed = 6f;
    [SerializeField]
    private float m_TurnSmoothTime = 0.5f;
    [SerializeField]
    private float m_TurnSmoothVelocity;
    //[SerializeField]
    //private float m_Gravity = -9.81f;
    [SerializeField]
    // Courbe utilisé pour le momentum du début de mouvement
    private AnimationCurve m_BeginVelocity = null;
    [SerializeField]
    // Courbe utilisé pour le momentum de fin de mouvement
    private AnimationCurve m_EndVelocity = null;

    Vector3 m_Velocity = Vector3.zero;

    private bool m_BeginWalk = false;

    // timer pour le momentum du début de mouvement
    private float m_BTimer = 0f;

    private float m_Speed = 0f;

    private Vector3 m_PastDirection = Vector3.zero;

    private bool m_IsWalking = false;

    private bool m_EndingWalk = false;

    // timer pour le momentum de fin de mouvement
    private float m_ETimer = 0f;

    private void Start()
    {
        m_Speed = m_TrueSpeed;
    }

    public Vector3 Walk(Transform p_Camera, CharacterController p_Controller, Vector3 p_Direction)
    {
        Vector3 l_DirectionToReturn = Vector3.zero;

        // est vrai si la longeur du vecteur direction est supérieur à zéro, donc si on le retour des inputs est supérieur à zéro 
        if (p_Direction.magnitude >= 0.1f)
        {
            m_BeginWalk = true;
            float l_TargetAngle = Mathf.Atan2(p_Direction.x, p_Direction.z) * Mathf.Rad2Deg + p_Camera.eulerAngles.y;
            float l_Angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, l_TargetAngle, ref m_TurnSmoothVelocity, m_TurnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, l_Angle, 0f);

            Vector3 l_MoveDir = Quaternion.Euler(0f, l_TargetAngle, 0f) * Vector3.forward;
            // p_Controller.Move(l_MoveDir.normalized * m_Speed * Time.deltaTime);
            l_DirectionToReturn = l_MoveDir.normalized * m_Speed;
            // sauvegarde de la direction pour l'appliquer pour le momentum de fin de mouvement
            m_PastDirection = l_MoveDir;
            m_IsWalking = true;
        }
        else
        {
            m_BeginWalk = false;
            // on passe dedans après l'arrêt du mouvement, effectivement, iswalking est vrai quand on bouge mais quand on arrête de bouger, is walking n'est pas mit à faux
            if (m_IsWalking)
            {
                m_EndingWalk = true;
                m_IsWalking = false;
            }
        }
        BeginWalking();
        EndWalking(p_Controller);

        return l_DirectionToReturn;
    }

    private void BeginWalking()
    {
        // on rentre dedans quand on bouge
        if (m_BeginWalk)
        {
            // on s'assure qu'il n'y est pas le momentum de fin de mouvement 
            m_EndingWalk = false;
            m_BTimer += 1 * Time.deltaTime;
            // la speed est calculé en fonction de la speed référence et de la position de la courbe, qui va de 0 à 1, par rapport au timer
            m_Speed = m_TrueSpeed * m_BeginVelocity.Evaluate(m_BTimer);
        }
        else
            m_BTimer = 0;
    }

    private void EndWalking(CharacterController p_Controller)
    {
        // si la speed est à 0, on arrète de décrémenter la speed en rendant false le bool de controle 
        if (m_Speed == 0)
        {
            m_EndingWalk = false;
            m_ETimer = 0;
        }
        // on rentre si on vient d'arrêter le mouvement 
        if (m_EndingWalk)
        {
            m_ETimer += 1 * Time.deltaTime;
            // la speed est calculé en fonction de la speed référence et de la position de la courbe, qui va de 0 à 1, par rapport au timer
            m_Speed = m_TrueSpeed * m_EndVelocity.Evaluate(m_ETimer);
            // movement dans la dernière direction 
            p_Controller.Move(m_PastDirection.normalized * m_Speed * Time.deltaTime);
        }
    }

    public float WalkingTrueSpeed
    {
        get { return m_TrueSpeed; }
    }

    public float WalkingSpeed
    {
        get { return m_Speed; }
    }

    public Vector3 WalkingPastDirection
    {
        get { return m_PastDirection; }
    }
}