using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIFollowDistance : MonoBehaviour
{
    // Start is called before the first frame update
    private GameObject pj;

    private AIControl _control;

    public float distance = 10;
    // Start is called before the first frame update
    void Start()
    {
        pj = GameObject.FindWithTag("ai");
        _control = GetComponent<AIControl>();

       
    }


    // Update is called once per frame
    void Update()
    {
        if (Vector3.Magnitude(pj.transform.position - transform.position) < distance)
        {
            _control.agent.isStopped = false;
            _control.agent.SetDestination(pj.transform.position);
        }
        else
        {
            _control.agent.isStopped = true;
        }
    }
}
