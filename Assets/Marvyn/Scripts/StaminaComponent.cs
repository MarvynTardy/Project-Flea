using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaminaComponent : MonoBehaviour
{
    [SerializeField]
    private float m_MaxStamina;
    private float m_CurrentStamina;

    public void UseStamina(float p_Amount)
    {
        m_CurrentStamina -= p_Amount;
    }

    public void GainStamina(float p_Amount)
    {
        m_CurrentStamina += p_Amount;
    }
}
