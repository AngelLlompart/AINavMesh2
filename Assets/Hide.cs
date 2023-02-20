using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Hide : MonoBehaviour
{
    private GameObject[] obstacles;
    private GameObject pj;
    private NavMeshAgent agent;
    // Start is called before the first frame update
    void Start()
    {
        pj = GameObject.FindWithTag("ai");
        agent = GetComponent<NavMeshAgent>();
        obstacles = GameObject.FindGameObjectsWithTag("Obstacle");
    }

    // Update is called once per frame
    void Update()
    {
        Hiding();
    }

    private void Hiding()
    {
        //GameObject obstacleDesti = obstacles.Min(x => Vector3.Magnitude(x.transform.position - agent.transform.position));
        GameObject obstacleDesti = new GameObject();
        float minDis = Mathf.Infinity;
        foreach (var obstacle in obstacles)
        {
            float distance = Vector3.Magnitude(obstacle.transform.position - agent.transform.position);
            if (distance < minDis)
            {
                minDis = distance;
                obstacleDesti = obstacle;
            }
        }
        agent.SetDestination(obstacleDesti.transform.position +
                             Vector3.Normalize(obstacleDesti.transform.position - pj.transform.position) * 4);
    }
}
