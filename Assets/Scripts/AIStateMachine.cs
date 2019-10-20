using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIStateMachine : MonoBehaviour
{
    public enum AIState {
        Stationary,
        Moving
        //TODO more? states…
    };

    private VelocityReporter reporter;
    private MinionAI minion;
    public AIState aiState;
    // Use this for initialization
    void Start () {
        minion = GetComponent<MinionAI>();
        aiState = AIState.Stationary;
    }

    void Update () {
        //state transitions that can happen from any state might happen here
        //such as:
        //if(inView(enemy) && (ammoCount == 0) &&
        // closeEnoughForMeleeAttack(enemy))
        // aiState = AIState.AttackPlayerWithMelee;
        //Assess the current state, possibly deciding to change to a different state
        GameObject[] waypoints = minion.waypoints;
        int currWaypoint = minion.currWaypoint;
        // if (currWaypoint >= (waypoints.Length - 1)) {
        //         currWaypoint = 0;
        // } else {
        //         currWaypoint++;
        // }
        switch (aiState) {
            case AIState.Stationary:
                //if(ammoCount == 0)
                // aiState = AIState.GoToAmmoDepot;
                //else
                // SteerTo(nextWaypoint);
                if (currWaypoint >= 0 && currWaypoint < waypoints.Length) {
                    reporter = waypoints[currWaypoint].GetComponent<VelocityReporter>();
                    if (reporter != null) {
                        aiState = AIState.Moving;
                    }
                }
                break;
            case AIState.Moving:

                if (currWaypoint >= 0 && currWaypoint < waypoints.Length) {
                    reporter = waypoints[currWaypoint].GetComponent<VelocityReporter>();
                    if (reporter == null) {
                        aiState = AIState.Stationary;
                    }
                }
                break;

            //... TODO handle other states
            default:
                break;
        }

    }
}