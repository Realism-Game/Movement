using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIStateMachine : MonoBehaviour
{
    public enum AIState {
        Normal,
        Pursuit,
        LostQuarry
        //TODO more? states…
    };

    private VelocityReporter reporter;
    private GuardAI guard;
    public AIState aiState;
    public float delay = 2f;
    private FieldOfView fov;
    private AudioSource alertSound;
    private MeshRenderer exclamationPoint;
    private MeshRenderer questionMark;
    // Use this for initialization
    void Start () {
        guard = GetComponent<GuardAI>();
        fov = GetComponent<FieldOfView>();
        aiState = AIState.Normal;
        alertSound = GetComponent<AudioSource>();
        Component[] meshes = GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer mesh in meshes) {
            if (mesh.name == "!") {
                exclamationPoint = mesh;
                Debug.Log(mesh.name);
            } else if (mesh.name == "?") {
                questionMark = mesh;
                Debug.Log(mesh.name);
            }
        }
        //exclamationPoint = GetComponentInChildren<MeshRenderer>();
        //questionMark = GetComponentInChildren<MeshRenderer>();
    }

    void Update () {
        //state transitions that can happen from any state might happen here
        //such as:
        //if(inView(enemy) && (ammoCount == 0) &&
        // closeEnoughForMeleeAttack(enemy))
        // aiState = AIState.AttackPlayerWithMelee;
        //Assess the current state, possibly deciding to change to a different state
        
        
        switch (aiState) {
            case AIState.Normal:
                if (fov.visibleTarget) {
                    aiState = AIState.Pursuit;
                    alertSound.Play(0);
                    exclamationPoint.enabled = true;
                    StartCoroutine(DisableWithDelay(delay, exclamationPoint));
                }
                break;
            case AIState.Pursuit:
                if (!fov.visibleTarget) {
                    if (fov.lostQuarry) {
                        aiState = AIState.LostQuarry;
                        exclamationPoint.enabled = false;
                        questionMark.enabled = true;
                    } else {
                        aiState = AIState.Normal;
                        exclamationPoint.enabled = false;
                    }
                } 
                break;
            case AIState.LostQuarry:
                if (fov.visibleTarget) {
                    aiState = AIState.Pursuit;
                    questionMark.enabled = false;
                } else if (!fov.lostQuarry) {
                    aiState = AIState.Normal;
                    questionMark.enabled = false;
                    exclamationPoint.enabled = false;
                }
                // if (los.foundSomething) {
                //     aiState = AIState.Pursuit;
                // } else if (!los.foundSomething && los.collisionObject == null){
                //     aiState = AIState.Normal;
                // }
                break;                
            //... TODO handle other states
            default:
                break;
        }

    }

    private IEnumerator DisableWithDelay(float delay, MeshRenderer mesh) {
        yield return new WaitForSeconds (delay);
        mesh.enabled = false;
    }
}