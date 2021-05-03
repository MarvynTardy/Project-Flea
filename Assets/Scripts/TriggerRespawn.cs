using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerRespawn : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        other.GetComponent<RespawnSystem>().RespawnBegin();
    }
}
