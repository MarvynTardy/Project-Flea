using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaminaComponent : MonoBehaviour
{
    [SerializeField] private float m_MaxStamina = 100;
    [SerializeField] public GameObject m_Parchment;
    [SerializeField] public MeshRenderer m_MeshParchment;
    [SerializeField] public GameObject m_ParchmentEmpty;
    public MeshRenderer m_MeshParchmentEmpty;
    private float m_CurrentStamina;
    private StaminaUI m_StaminaUI;
    private float m_CutoffValue;
    private float m_Amount;

    public float CurrentStamina
    {
        get
        {
            return m_CurrentStamina;
        }
        set
        {
            m_CurrentStamina = value;
            if (m_CurrentStamina < 0)
            {
                m_CurrentStamina = 0;
            }
            if (m_CurrentStamina > m_MaxStamina)
            {
                m_CurrentStamina = m_MaxStamina;
            }
        }       
    }

    private void Awake()
    {
        m_MeshParchmentEmpty = m_ParchmentEmpty.GetComponentInChildren<MeshRenderer>();

        m_StaminaUI = FindObjectOfType<StaminaUI>();
        m_CurrentStamina = m_MaxStamina;
        SetShaderStamina();
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
            // m_MeshParchment.material.SetFloat("_Cutoff", (CurrentStamina - 28.571f) / 71.429f);
            // m_Amount = Mathf.Lerp(-0.5f, 1, m_CurrentStamina / 100);
            // m_Amount = ((m_CurrentStamina - 0 * (-0.5f - 0.1f)) / 100)+ 0.1f;
            SetShaderStamina();

        }
    }

    public void GainStamina(float p_Amount)
    {
        m_CurrentStamina += p_Amount; 
        m_StaminaUI.SetStamina(CurrentStamina);
        SetShaderStamina();
    }

    private void SetShaderStamina()
    {
        m_Amount = (-0.006f * m_CurrentStamina) + 0.1f;
        m_MeshParchment.material.SetFloat("_Amount", m_Amount);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 13)
        {
            GainStamina(m_MaxStamina - CurrentStamina);
            AudioManager.PlaySound(AudioManager.Sound.ManaRestore);
        }
    }
}
