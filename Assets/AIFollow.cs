using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIFollow : MonoBehaviour
{
    private GameObject pj;

    private AIControl _control;
    // Start is called before the first frame update
    void Start()
    {
        pj = GameObject.FindWithTag("ai");
        _control = GetComponent<AIControl>();

       
    }

    // Update is called once per frame
    void Update()
    {
        _control.agent.SetDestination(pj.transform.position); 
    }
}
