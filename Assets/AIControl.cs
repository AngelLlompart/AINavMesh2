using System;
using UnityEngine;
using UnityEngine.AI;

public class AIControl : MonoBehaviour {

    // Storage for the prefab
    public NavMeshAgent agent;


    private void Start()
    {
        agent = this.GetComponent<NavMeshAgent>();
    }

}
