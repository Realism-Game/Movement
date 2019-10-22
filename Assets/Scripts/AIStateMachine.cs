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
    private GuardAI guard;
    public AIState aiState;
    private GameObject lightObject;
    private GameObject coneObject;
    // Use this for initialization
    void Start () {
        guard = GetComponent<GuardAI>();
        aiState = AIState.Stationary;
        lightObject = this.gameObject.transform.GetChild(0).gameObject;
        int i = 0;
        while (lightObject.name != "Light") {
            i++;
            lightObject = this.gameObject.transform.GetChild(i).gameObject;
        }
        i = 0;
        coneObject = lightObject.transform.GetChild(0).gameObject;
        while(coneObject.name != "Cone") {
            i++;
            coneObject = lightObject.transform.GetChild(0).gameObject;
        }
    }

    void Update () {
        //state transitions that can happen from any state might happen here
        //such as:
        //if(inView(enemy) && (ammoCount == 0) &&
        // closeEnoughForMeleeAttack(enemy))
        // aiState = AIState.AttackPlayerWithMelee;
        //Assess the current state, possibly deciding to change to a different state
        LightLineOfSight los = coneObject.GetComponent<LightLineOfSight>();
        switch (aiState) {
            case AIState.Stationary:
                if (los.foundSomething) {
                    aiState = AIState.Moving;
                }
                break;
            case AIState.Moving:
                if (!los.foundSomething) {
                    aiState = AIState.Stationary;
                }
                break;

            //... TODO handle other states
            default:
                break;
        }

    }
}