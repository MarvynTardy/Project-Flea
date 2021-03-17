using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaUI : MonoBehaviour
{
    [SerializeField]
    private Slider m_StaminaAmount;
    private StaminaComponent m_StaminaComponent;
    private void Start()

    {
        m_StaminaComponent = FindObjectOfType<StaminaComponent>();
        m_StaminaAmount.value = m_StaminaComponent.CurrentStamina;
    }
    public void SetStamina(float p_Stamina)
    {
        m_StaminaAmount.value = m_StaminaComponent.CurrentStamina;
        
    }


    
}
