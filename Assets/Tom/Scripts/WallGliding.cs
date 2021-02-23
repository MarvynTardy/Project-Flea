using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallGliding : MonoBehaviour
{
    [SerializeField] private GameObject m_PlayerGraphicVisual = null;

    private void Update()
    {
        DetectionWall();
    }

    private void ChangePosition(RaycastHit p_HitInfo, string p_Orientation)
    {
        if(p_Orientation == "right")
        {
            Debug.Log("aaa");
            m_PlayerGraphicVisual.transform.rotation = Quaternion.Euler(45, 45, 45);
        }
        if (p_Orientation == "left")
        {
            m_PlayerGraphicVisual.transform.rotation = Quaternion.Euler(0, 0, -45);
        }
    }

    private void DetectionWall()
    {
        RaycastHit l_HitRight;
        if (Physics.Raycast(transform.position, transform.right, out l_HitRight, 1))
        {  
            if(l_HitRight.transform.tag == "WallGlide")
            {
                Debug.Log("Right");
                ChangePosition(l_HitRight, "right");
            }
        }
        RaycastHit l_HitLeft;
        if (Physics.Raycast(transform.position, transform.right * -1, out l_HitLeft, -1))
        {
            if (l_HitRight.transform.tag == "WallGlide")
            {
                Debug.Log("Left");
                ChangePosition(l_HitLeft, "left");
            }
        }
    }
}