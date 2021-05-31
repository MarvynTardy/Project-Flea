using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewpointManager : MonoBehaviour
{
    public bool[] m_ActivationList = new bool[] { false, false, false };
    public MeshRenderer[] m_GlypheFeedback;

    public Material m_GlowingMaterial;
    // public Dictionary<int, ViewpointBehaviour> m_ViewPoints = new Dictionary<int, ViewpointBehaviour>();
    // public ViewpointBehaviour[] m_ViewPointsList;


    //private void Start()
    //{
    //    m_ViewPointsList = FindObjectsOfType<ViewpointBehaviour>();
    //}

    //public void AddValidViewPoints()
    //{
    //    for (int i = 0; i < m_ViewPointsList.Length; i++)
    //    {
    //        if (m_ViewPointsList[i].m_IsExit)
    //            m_ActivationList[i] = true;
    //    }
    //}

    public void AddValidViewPointsOpti(int p_ViewPointID)
    {
        m_ActivationList[p_ViewPointID] = true;
        m_GlypheFeedback[p_ViewPointID].material = m_GlowingMaterial;
    }
}
