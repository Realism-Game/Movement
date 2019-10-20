using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UnityEngine.AI.NavMeshAgent))]
public class MinionAI : MonoBehaviour
{
    public GameObject[] waypoints;
    public int currWaypoint = -1;
    private Animator anim;
    private UnityEngine.AI.NavMeshAgent myNavMeshAgent;
    private AIStateMachine stateMachine;
    private float previousYRotate;
    private int stationary;
    // Start is called before the first frame update
    void Start()
    {
        stateMachine = GetComponent<AIStateMachine>();
        anim = GetComponent<Animator>();
        myNavMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        setNextWaypoint();
        previousYRotate = this.transform.eulerAngles.y;
    }

    // Update is called once per frame
    void Update()
    {
        anim.SetFloat("vely", myNavMeshAgent.velocity.magnitude / myNavMeshAgent.speed);
        if (!myNavMeshAgent.pathPending) {
            float curYRotate = this.transform.eulerAngles.y;
            if ((curYRotate - previousYRotate) <= 0.001) {
                stationary++;
            } else {
                stationary = 0;
            }
            previousYRotate = curYRotate;
            if (myNavMeshAgent.remainingDistance == 0 || stationary >= 1000) {
                stationary = 0;
                setNextWaypoint();
            } else if (stateMachine.aiState == AIStateMachine.AIState.Moving) {
                stationary = 0;
                if ((myNavMeshAgent.remainingDistance - myNavMeshAgent.stoppingDistance) >= 0.25f) {
                    Vector3 destination = getMovingWaypointDestination();
                    myNavMeshAgent.SetDestination(destination);
                }
            }
        }
    }

    private Vector3 getMovingWaypointDestination() {
            //Debug.Log(myNavMeshAgent.remainingDistance - myNavMeshAgent.stoppingDistance);
            GameObject g = waypoints[this.currWaypoint];
            Vector3 destination = g.transform.position;
            //Debug.Log("waypoint "+destination);
            if (stateMachine.aiState == AIStateMachine.AIState.Moving  && currWaypoint != 0) {
                //predict position
                VelocityReporter reporter = g.GetComponent<VelocityReporter>();
                /**
                dist = (target.pos - agent.pos).Length()

                lookAheadT = Dist/agent.maxSpeed

                futureTarget = target.pos + lookAheadT  * target.velocity
                **/
                float distance = (destination - this.transform.position).magnitude;
                float lookAheadT = distance / myNavMeshAgent.speed;
                Vector3 futureTarget = destination + lookAheadT * reporter.velocity;
                destination = futureTarget;
                /**
                Debug.Log("distance "+distance);
                Debug.Log("lookAheadT " + lookAheadT);
                Debug.Log("minion speed "+myNavMeshAgent.speed);
                Debug.Log("waypoint speed "+reporter.velocity.magnitude);
                Debug.Log("waypoint velocity "+reporter.velocity);
                Debug.Log("waypoint rawvelocity "+reporter.rawVelocity);
                **/
                //Debug.Log("futureTarget "+futureTarget);
            }
            return destination;
    }

    private void setNextWaypoint() {
        try {
            if (waypoints == null || waypoints.Length == 0) {
                throw new System.IndexOutOfRangeException();
            }

            if (this.currWaypoint >= (waypoints.Length - 1)) {
                this.currWaypoint = 0;
            } else {
                this.currWaypoint++;
            }
            //Debug.Log(currWaypoint);
            GameObject g = waypoints[this.currWaypoint];
            Vector3 destination = g.transform.position;
            //Debug.Log("waypoint "+destination);
            if (stateMachine.aiState == AIStateMachine.AIState.Moving && currWaypoint != 0) {
                destination = getMovingWaypointDestination();
            }
            myNavMeshAgent.SetDestination(destination);
        } catch (System.IndexOutOfRangeException e) {
            Debug.Log(e.Message);
            // Set IndexOutOfRangeException to the new exception's InnerException.
            //throw new System.ArgumentOutOfRangeException("index parameter is out of range.", e);
        } catch (System.ArgumentOutOfRangeException e) {
            Debug.Log(e.Message);
        }

    }
}
