using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FollowWP : MonoBehaviour
{
    private float posY;

    private Vector3 pos1;
    private Vector3 pos2;
    private Vector3 pos3;
    private Vector3 pos4;

    private List<Vector3> destinations;

    private int x = 0;

    private NavMeshAgent agent;
    
    private GameObject pj;


    public float distance = 10;

    private bool following = false;
    
    [SerializeField] private GameObject obstacle;
    // Start is called before the first frame update
    void Start()
    {
        pj = GameObject.FindWithTag("ai");
        posY = transform.position.y;
        pos1 = new Vector3(18, posY, 10);
        pos2 = new Vector3(0, posY, 10);
        pos3 = new Vector3(-24, posY, -11);
        pos4 = new Vector3(-3, posY, -21);
        destinations = new List<Vector3>(){pos1, pos2, pos3, pos4};
        agent = GetComponent<NavMeshAgent>();
        
        
    }

    // Update is called once per frame
    void Update()
    {
        Follow();
        if (!following)
        {
            Patrol();
        }

    }


    private void Patrol()
    {
        GoToNextWP();
        if (Vector3.Magnitude(agent.transform.position - destinations[x]) < 1)
        {
            if (x < destinations.Count -1)
            {
                x++;
            }
            else
            {
                x = 0;
            }
            GoToNextWP();
        } 
    }
    private void GoToNextWP()
    {
        agent.SetDestination(destinations[x]);
    }


    private void Follow()
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
                    following = true;
                    agent.SetDestination(pj.transform.position);
                }
                else
                {
                    following = false;
                } 
            } else
            {
                following = false;
            }
        }
        else
        {
            following = false;
        }
    }
    
    private void Hiding()
    {
        agent.SetDestination(obstacle.transform.position +
                             Vector3.Normalize(obstacle.transform.position - pj.transform.position) * 4);
    }
    
    private void RunAway()
    {
        agent.SetDestination(2*transform.position - pj.transform.position);
    }
}
