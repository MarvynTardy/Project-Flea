using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaminaComponent : MonoBehaviour
{
    [SerializeField]
    private float m_MaxStamina = 100;
    [SerializeField]
    private MeshRenderer m_MeshParchment;
    private float m_CurrentStamina;
    private StaminaUI m_StaminaUI;
    private float m_CutoffValue;

    public float CurrentStamina
    {
        get
        {
            return m_CurrentStamina;
        }
        set
        {
            m_CurrentStamina = value;
        }       
    }

    private void Awake()
    {
        m_StaminaUI = FindObjectOfType<StaminaUI>();
        m_CurrentStamina = m_MaxStamina;
        m_MeshParchment.material.SetFloat("_Cutoff", 1f);
        AudioManager.Initialize();
    }

    private void Start()
    {        
        m_StaminaUI.SetStamina(CurrentStamina);

        // m_MeshParchment.material.SetFloat("_Cutoff", 0.5f);
    }

    public void UseStamina(float p_Amount)
    {
        if(m_CurrentStamina > 0)
        {
            m_CurrentStamina -= p_Amount;
            m_StaminaUI.SetStamina(CurrentStamina);
            m_MeshParchment.material.SetFloat("_Cutoff", (CurrentStamina - 28.571f) / 71.429f);
        }
    }

    public void GainStamina(float p_Amount)
    {
        m_CurrentStamina += p_Amount;
        m_StaminaUI.SetStamina(CurrentStamina);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 13)
        {
            GainStamina(m_MaxStamina - CurrentStamina);
        }
    }
}
