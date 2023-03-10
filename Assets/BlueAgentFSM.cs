using UnityEngine;
using UnityEngine.AI;

public class BlueAgentFSM : MonoBehaviour {

    // An array of GameObjects to store all the agents
    public GameObject Redagent;
    private NavMeshAgent Blueagent;
    State currentState;


    void Start() {

        // Grab everything with the 'ai' tag
        Blueagent=this.GetComponent<NavMeshAgent>();
        currentState = new State.Patrol(this.gameObject, Blueagent, Redagent.transform); // Create our first state.
    }

    // Update is called once per frame
    void Update() {
        // Trobam a l'enemic
                 currentState = currentState.Process(); // Calls Process method to ensure correct state is set.
            }
    
}
