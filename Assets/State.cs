using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; // Added since we're using a navmesh.

public class State
{
    // 'States' that the NPC could be in.
    public enum STATE
    {
        PATROL,
        ATTACK,
        HIDE,
        RUNAWAY,
        IDLE
    };

    // 'Events' - where we are in the running of a STATE.
    public enum EVENT
    {
        ENTER,
        UPDATE,
        EXIT
    };

    public STATE name; // To store the name of the STATE.
    protected EVENT stage; // To store the stage the EVENT is in.
    protected GameObject npc; // To store the NPC game object.

    protected Transform
        player; // To store the transform of the player. This will let the guard know where the player is, so it can face the player and know whether it should be shooting or chasing (depending on the distance).

    protected State
        nextState; // This is NOT the enum above, it's the state that gets to run after the one currently running (so if IDLE was then going to PATROL, nextState would be PATROL).

    protected NavMeshAgent agent; // To store the NPC NavMeshAgent component.

    public float distance = 6;

    private float angleMax = 60;

    // Constructor for State
    public State(GameObject _npc, NavMeshAgent _agent, Transform _player)
    {
        npc = _npc;
        agent = _agent;
        stage = EVENT.ENTER;
        player = _player;
    }

    // Phases as you go through the state.
    public virtual void Enter()
    {
        stage = EVENT.UPDATE;
    } // Runs first whenever you come into a state and sets the stage to whatever is next, so it will know later on in the process where it's going.

    public virtual void Update()
    {
        stage = EVENT.UPDATE;
    } // Once you are in UPDATE, you want to stay in UPDATE until it throws you out.

    public virtual void Exit()
    {
        stage = EVENT.EXIT;
    } // Uses EXIT so it knows what to run and clean up after itself.

    // The method that will get run from outside and progress the state through each of the different stages.
    public State Process()
    {
        if (stage == EVENT.ENTER) Enter();
        if (stage == EVENT.UPDATE) Update();
        if (stage == EVENT.EXIT)
        {
            Exit();
            return nextState; // Notice that this method returns a 'state'.
        }

        return this; // If we're not returning the nextState, then return the same state.
    }

    public void ChoiceMaker()
    {
        RaycastHit hit;
        if (Physics.Raycast(player.position, (npc.transform.position - player.position), out hit, distance))
        {
            if (hit.transform.gameObject.CompareTag("enemy"))
            {
                float angle = Vector3.Angle(npc.transform.position - player.position, player.forward);
                if (angle < 60 && angle > -60)
                {
                    nextState = new Hides(npc, agent, player);
                    stage = EVENT.EXIT;
                }
                else
                {
                    nextState = new Attack(npc, agent, player);
                    stage = EVENT.EXIT;
                }
            }
            else
            {
                nextState = new Attack(npc, agent, player);
                stage = EVENT.EXIT;
            }
        }
        else
        {
            Debug.Log("THIS AINT HAPPENING");
        }
    }

    public bool RedNotCloseToBlue()
    {
        return Vector3.Distance(player.position, npc.transform.position) > distance;
    }

    public bool RedLookAtBlue()
    {
        RaycastHit hit;
        if (Physics.Raycast(player.position, (npc.transform.position - player.position), out hit, distance))
        {
            if (hit.transform.gameObject.CompareTag("enemy"))
            {
                float angle = Vector3.Angle(npc.transform.position - player.position, player.forward);
                if (angle < angleMax && angle > -angleMax)
                {
                    return true;
                }
            }
        }

        return false;
    }
    
    //si red esta mirantlo encara que estigui darrera obstacle

    public bool BlueInRedAngle()
    {
        RaycastHit hit;
        if (Physics.Raycast(player.position, (npc.transform.position - player.position), out hit, distance))
        {
            float angle = Vector3.Angle(npc.transform.position - player.position, player.forward);
            if (angle < angleMax && angle > -angleMax)
            {
                return true;
            }
        }

        return false;
    }

    // Constructor for Idle state.
    public class Idle : State
    {
        public Idle(GameObject _npc, NavMeshAgent _agent, Transform _player)
                    : base(_npc, _agent, _player)
        {
            name = STATE.IDLE; // Set name of current state.
        }

        public override void Enter()
        {
            agent.isStopped = true;
            Debug.Log(name);
            base.Enter(); // Sets stage to UPDATE.
        }

        public override void Update()
        {
            distance = 10;
            if (RedNotCloseToBlue())
            {
                nextState = new Patrol(npc, agent, player);
                stage = EVENT.EXIT;
            }
            else
            {
                if (!BlueInRedAngle())
                {
                    nextState = new Attack(npc, agent, player);
                    stage = EVENT.EXIT;
                }
                else if (RedLookAtBlue())
                {
                    nextState = new Hides(npc, agent, player);
                    stage = EVENT.EXIT;
                }
            }
            
        }
        public override void Exit()
        {
            distance = 6;
            Debug.Log(distance);
            agent.isStopped = false;
            base.Exit();
        }
    }

// Constructor for Patrol state.
    public class Patrol : State
    {
        int puntActual = 0;
        Vector3[] listaPuntos = new Vector3[2];

        public Patrol(GameObject _npc, NavMeshAgent _agent, Transform _player)
            : base(_npc, _agent, _player)
        {
            name = STATE.PATROL; // Set name of current state.
            agent.speed =
                2; // How fast your character moves ONLY if it has a path. Not used in Idle state since agent is stationary.
            agent.isStopped = false; // Start and stop agent on current path using this bool.
        }

        public override void Enter()
        {
            listaPuntos[0] = new Vector3(18f, 0f, 7f);
            listaPuntos[1] = new Vector3(0f, 0f, 13f);
            float lastDist = Mathf.Infinity; // Store distance between NPC and waypoints.

            // Calculate closest waypoint by looping around each one and calculating the distance between the NPC and each waypoint.
            for (int i = 0; i < listaPuntos.Length; i++)
            {
                float distance = Vector3.Distance(npc.transform.position, listaPuntos[i]);
                if (distance < lastDist)
                {
                    puntActual = i;
                    lastDist = distance;
                }
            }

            base.Enter();
        }

        public override void Update()
        {
            // Check if agent hasn't finished walking between waypoints.
            if (RedNotCloseToBlue())
            {

                if (Vector3.Distance(npc.transform.position, listaPuntos[puntActual]) < 1)
                {
                    puntActual += 1;
                }

                if (puntActual == listaPuntos.Length)
                {
                    puntActual = 0;
                }

                agent.SetDestination(listaPuntos[puntActual]); // Set agents destination 
            }
            else
            {
                if (RedLookAtBlue())
                {
                    nextState = new Hides(npc, agent, player);
                    stage = EVENT.EXIT;
                }
                else
                {
                    nextState = new Attack(npc, agent, player);
                    stage = EVENT.EXIT;
                }
            }

            if (Input.GetKeyDown(KeyCode.A))
            {
                nextState = new Attack(npc, agent, player);
                stage = EVENT
                    .EXIT; // The next time 'Process' runs, the EXIT stage will run instead, which will then return the nextState.
            }
            else if (Input.GetKeyDown(KeyCode.H))
            {
                nextState = new Hides(npc, agent, player);
                stage = EVENT.EXIT;
            }
            else if (Input.GetKeyDown(KeyCode.R))
            {
                nextState = new RunAway(npc, agent, player);
                stage = EVENT.EXIT;
            }
        }

        public override void Exit()
        {
            base.Exit();
        }
    }


    public class Attack : State
    {
        public Attack(GameObject _npc, NavMeshAgent _agent, Transform _player)
            : base(_npc, _agent, _player)
        {
            name = STATE.ATTACK; // Set name to correct state.
            agent.speed = 4; // How fast your character moves
        }

        public override void Enter()
        {
            agent.isStopped = false;
            base.Enter();
        }

        public override void Update()
        {

            if (RedNotCloseToBlue())
            {
                nextState = new Patrol(npc, agent, player);
                stage = EVENT.EXIT;
            }
            else
            {
                if (RedLookAtBlue())
                {
                    nextState = new Hides(npc, agent, player);
                    stage = EVENT.EXIT;
                }
                else
                {
                    agent.SetDestination(player.position); // Set agents destination 
                }
            }

            //poner las condiciones aqui tmb




            /*if(Input.GetKeyDown(KeyCode.P))
            {
                nextState = new Patrol(npc, agent, player);
                stage = EVENT.EXIT; // The next time 'Process' runs, the EXIT stage will run instead, which will then return the nextState.
            }
            else if (Input.GetKeyDown(KeyCode.H))
            {
                nextState = new Hides(npc, agent, player);
                stage = EVENT.EXIT;
            }
            else if (Input.GetKeyDown(KeyCode.R))
            {
                nextState = new RunAway(npc, agent, player);
                stage = EVENT.EXIT;
            }*/

        }

        public override void Exit()
        {
            base.Exit();
        }
    }

    public class Hides : State
    {
        private GameObject[] obstacles;
        GameObject obstacleDesti;

        public Hides(GameObject _npc, NavMeshAgent _agent, Transform _player)
            : base(_npc, _agent, _player)
        {
            name = STATE.HIDE;
            agent.speed = 4;
            obstacles = GameObject.FindGameObjectsWithTag("Obstacle");
        }

        public override void Enter()
        {
            agent.isStopped = false;
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

            base.Enter();
        }

        public override void Update()
        {
            if (RedNotCloseToBlue())
            {
                nextState = new Patrol(npc, agent, player);
                stage = EVENT.EXIT;
            }
            else
            {
                if (RedLookAtBlue())
                {
                    agent.SetDestination(obstacleDesti.transform.position +
                                         Vector3.Normalize(obstacleDesti.transform.position - player.position) *
                                         agent.speed);
                }
                else
                {
                    nextState = new Idle(npc, agent, player);
                    stage = EVENT.EXIT;
                }
            }
            /*
            // The only place where Update can break out of itself. Set chance of breaking out at 10%.
            if(Input.GetKeyDown(KeyCode.A))
            {
                nextState = new Attack(npc, agent, player);
                stage = EVENT.EXIT; // The next time 'Process' runs, the EXIT stage will run instead, which will then return the nextState.
            }else if (Input.GetKeyDown(KeyCode.P))
            {
                nextState = new Patrol(npc, agent, player);
                stage = EVENT.EXIT;
            }*/
        }

        public override void Exit()
        {
            base.Exit();
        }
    }

    public class RunAway : State
    {
        public RunAway(GameObject _npc, NavMeshAgent _agent, Transform _player)
            : base(_npc, _agent, _player)
        {
            name = STATE.RUNAWAY;
        }

        public override void Enter()
        {
            agent.isStopped = false;
            base.Enter();
        }

        public override void Update()
        {
            Vector3 direction = Vector3.Normalize(npc.transform.position - player.transform.position);
            agent.SetDestination(npc.transform.position + direction);

            /*if (Vector3.Distance(npc.transform.position, destination) < 1)
            {
                destination = agent.speed * npc.transform.position - player.position;
                agent.SetDestination(destination);
            }*/

            if (Input.GetKeyDown(KeyCode.A))
            {
                nextState = new Attack(npc, agent, player);
                stage = EVENT
                    .EXIT; // The next time 'Process' runs, the EXIT stage will run instead, which will then return the nextState.
            }
            else if (Input.GetKeyDown(KeyCode.P))
            {
                nextState = new Patrol(npc, agent, player);
                stage = EVENT.EXIT;
            }
            else if (Input.GetKeyDown(KeyCode.H))
            {
                nextState = new Hides(npc, agent, player);
                stage = EVENT.EXIT;
            }
        }

        public override void Exit()
        {
            base.Exit();
        }
    }
}