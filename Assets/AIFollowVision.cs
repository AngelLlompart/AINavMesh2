using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIFollowVision : MonoBehaviour
{
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
        RaycastHit hit;
        if (Physics.Raycast(transform.position, (pj.transform.position - transform.position), out hit, distance))
        {
            if (hit.transform.gameObject.CompareTag("ai"))
            {
                float angle = Vector3.Angle(pj.transform.position - transform.position, transform.forward);
                Debug.Log(angle);
                if (angle < 60 && angle > -60)
                {
                    _control.agent.isStopped = false;
                    _control.agent.SetDestination(pj.transform.position);
                }
                else
                {
                    _control.agent.isStopped = true;
                } 
            } else {
                _control.agent.isStopped = true;
            }
        }
        else
        {
            _control.agent.isStopped = true;
        }
    }
}
