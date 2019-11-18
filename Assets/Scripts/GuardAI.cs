using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UnityEngine.AI.NavMeshAgent))]
[RequireComponent(typeof (ThirdPersonCharacterNoCrouch))]
public class GuardAI : MonoBehaviour
{
    //public GameObject enemy;
    public GameObject[] waypoints;
    public int currWaypoint = -1;
    public float walk = 0.7f;
    public float run = 1.3f;
    public bool isStationary;



    private float temp = 0f; 
    private Animator anim;
    private UnityEngine.AI.NavMeshAgent myNavMeshAgent;
    private AIStateMachine stateMachine;
    private float previousYRotate;
    private ThirdPersonCharacterNoCrouch character;
    private Vector3 lastSeen;
    private FieldOfView fov;

    [Range(1,179)]
    public float rotation = 90f;

    private Vector3 origin;
    private Vector3 startForward;
    private Vector3 leftBound;
    private Vector3 rightBound;
    private float direction;
    private bool pause = false;
    // Start is called before the first frame update
    void Start()
    {
        fov = GetComponent<FieldOfView>();
        stateMachine = GetComponent<AIStateMachine>();
        anim = GetComponent<Animator>();
        myNavMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        character = GetComponent<ThirdPersonCharacterNoCrouch>();
        if (!isStationary) {
            setNextWaypoint();
        } else {
            //get rotation bounds for stationary guard
            Debug.Log(transform.forward);
            Debug.Log(Quaternion.Euler(0, rotation, 0) * transform.forward);
            Debug.Log(Quaternion.Euler(0, -rotation, 0) * transform.forward);
            startForward = transform.forward;
            leftBound = Quaternion.Euler(0, -rotation, 0) * transform.forward;
            rightBound = Quaternion.Euler(0, rotation, 0) * transform.forward;
            direction = 1f;
            origin = transform.position;
        }

        previousYRotate = this.transform.eulerAngles.y;
    }

    // void FixedUpdate() {
    //     RaycastHit hit;
    //     int layerMask = 1<<8;
    //     float distance;
    //     distance = Vector3.Distance(transform.position, enemy.transform.position);
    //     if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, distance, layerMask)){
    //         Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * distance, Color.red, 5.0f);         
    //     } else {
    //         Bounds bound = enemy.GetComponent<Renderer>().bounds;
    //         Vector3 center = bound.center;
    //         Vector3 normal = center.normalized;

    //         Vector3 targetDir = enemy.transform.position - transform.position;
    //         float angleToPlayer = (Vector3.Angle(targetDir, transform.forward));
    //         if (angleToPlayer >= -22.5 && angleToPlayer <= 22.5 && distance <= 7.5f) { // 180° FOV
    //             Debug.DrawRay(transform.position, center - transform.position, Color.green, 5.0f);
    //             Debug.Log("Player in sight!");
    //         }
    //         //Debug.DrawRay(transform.position, max - transform.position, Color.green, 5.0f);
                       
    //     }
    // }

    // Update is called once per frame
    void Update()
    {
        // if (Input.GetKey(KeyCode.P)){
        //     Debug.Log("velocity: " + myNavMeshAgent.velocity);
        //     Debug.Log("acceleration: " + myNavMeshAgent.acceleration);
        //     Debug.Log("speed: " + myNavMeshAgent.speed);
        // }

        if (stateMachine.aiState == AIStateMachine.AIState.Normal) {
            myNavMeshAgent.speed = walk;
        } else {
            myNavMeshAgent.speed = run;
        }
        //anim.SetFloat("Forward", myNavMeshAgent.velocity.magnitude / myNavMeshAgent.speed);
        if (!isStationary) {
            if (!myNavMeshAgent.pathPending) {

                if (myNavMeshAgent.remainingDistance == 0) {
                    setNextWaypoint();
                } else if (stateMachine.aiState == AIStateMachine.AIState.Pursuit) {
                    if ((myNavMeshAgent.remainingDistance - myNavMeshAgent.stoppingDistance) >= 0.25f) {
                        Vector3 destination = getMovingWaypointDestination();
                        myNavMeshAgent.SetDestination(destination);
                    }
                } else if (stateMachine.aiState == AIStateMachine.AIState.LostQuarry && lastSeen != new Vector3(-100f, -100f, -100f)) {
                    Debug.Log("Last Seen: " + lastSeen);
                    myNavMeshAgent.SetDestination(lastSeen);
                    lastSeen = new Vector3(-100f, -100f, -100f);
                }
                if (myNavMeshAgent.remainingDistance > myNavMeshAgent.stoppingDistance)
                    character.Move(myNavMeshAgent.desiredVelocity, false, false);
                else
                    character.Move(Vector3.zero, false, false);
                
            }
        } else {
            //Debug.Log(transform.right * Time.deltaTime * .01f);
            //Vector3 vector = Quaternion.Euler(0, 1, 0) * transform.right;
            if (stateMachine.aiState == AIStateMachine.AIState.Normal) {
                if (transform.position != origin && !myNavMeshAgent.hasPath) {
                    Debug.Log("yo");
                    myNavMeshAgent.SetDestination(origin);
                } else if (Vector3.Distance(transform.position, origin) <= .02f) {
                    if (Vector3.Distance(transform.forward, rightBound) <= .02f) {
                        direction = -1f;
                        Debug.Log("right");
                    } else if (Vector3.Distance(transform.forward, leftBound) <= .02f) {
                        direction = 1f;
                        Debug.Log("left");                
                    } else if (Vector3.Distance(transform.forward, startForward) <= .02f) {
                        Debug.Log("forward");
                    }

                    character.Move(direction * transform.right, false, false);   
                }
             
            } else if (stateMachine.aiState == AIStateMachine.AIState.Pursuit) {

                if ((myNavMeshAgent.remainingDistance - myNavMeshAgent.stoppingDistance) >= 0.25f || !myNavMeshAgent.hasPath) {
                    Vector3 destination = getMovingWaypointDestination();
                    myNavMeshAgent.SetDestination(destination);
                }

                if (myNavMeshAgent.remainingDistance > myNavMeshAgent.stoppingDistance)
                    character.Move(myNavMeshAgent.desiredVelocity, false, false);
                else
                    character.Move(Vector3.zero, false, false);
            }

        }

    }

    private IEnumerator delay(float time, bool b) {
        yield return new WaitForSeconds (time);
        b = false;
    }

    private Vector3 getMovingWaypointDestination() {
            //Debug.Log(myNavMeshAgent.remainingDistance - myNavMeshAgent.stoppingDistance);
            GameObject g = fov.visibleTarget.gameObject;
            Vector3 destination = g.transform.position;
            lastSeen = destination;
            //Debug.Log("waypoint "+destination);
            if (stateMachine.aiState == AIStateMachine.AIState.Pursuit) {
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
                //Debug.Log("moving waypoint "+destination);
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
            if (stateMachine.aiState == AIStateMachine.AIState.Pursuit && currWaypoint != 0) {
                destination = getMovingWaypointDestination();
            }
            // if (fov.visibleTarget) {
            //     destination = fov.visibleTarget.transform.position;
            // }
            //Debug.Log("normal waypoint "+destination);
            //Debug.Log("setting next");
            fov.lostQuarry = false;
            myNavMeshAgent.SetDestination(destination);
        } catch (System.IndexOutOfRangeException e) {
            Debug.Log(e.Message);
            // Set IndexOutOfRangeException to the new exception's InnerException.
            //throw new System.ArgumentOutOfRangeException("index parameter is out of range.", e);
        } catch (System.ArgumentOutOfRangeException e) {
            Debug.Log(e.Message);
        }
    }

    void OnCollisionEnter(Collision c) {
        if(c.gameObject.CompareTag("Detectable")) {
            // GameObject gameObject = this.gameObject.transform.GetChild(0).gameObject;
            // int i = 0;
            // while (gameObject.name != "Light") {
            //     i++;
            //     gameObject = this.gameObject.transform.GetChild(i).gameObject;
            // }
            // i = 0;
            // gameObject = gameObject.transform.GetChild(0).gameObject;
            // while(gameObject.name != "Cone") {
            //     i++;
            //     gameObject = gameObject.transform.GetChild(0).gameObject;
            // }
            if (c.gameObject != null) {
                fov.visibleTarget = null;
                // los.foundSomething = false;
                // los.collisionObject = null;
                Destroy(c.gameObject);
                Collider collider = gameObject.GetComponent<Collider>();
                collider.enabled = true;
            }
        }
    }
}
