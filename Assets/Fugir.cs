using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Fugir : MonoBehaviour
{
    private GameObject pj;
    private NavMeshAgent agent;
    // Start is called before the first frame update
    void Start()
    {
        pj = GameObject.FindWithTag("ai");
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        RunAway();
    }

    private void RunAway()
    {
        agent.SetDestination(2*transform.position - pj.transform.position);
    }
}
