using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FieldOfView : MonoBehaviour {

    public float viewRadius;
    [Range(0,360)]
    public float viewAngle;

    public LayerMask targetMask;
    public LayerMask obstacleMask;

    public bool lostQuarry = false;

    //[HideInInspector]
    public List<Transform> visibleTargets = new List<Transform>();

    public Transform visibleTarget;

    void Start() {
        //StartCoroutine ("FindTargetsWithDelay", .2f);
    }

    void FixedUpdate() {
        //FindVisibleTargets();
        FindVisibleTarget();
    }


    IEnumerator FindTargetsWithDelay(float delay) {
        while (true) {
            yield return new WaitForSeconds (delay);
            FindVisibleTargets ();
        }
    }

    void FindVisibleTargets() {
        visibleTargets.Clear ();
        Collider[] targetsInViewRadius = Physics.OverlapSphere (transform.position, viewRadius, targetMask);

        for (int i = 0; i < targetsInViewRadius.Length; i++) {
            Transform target = targetsInViewRadius [i].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;
            if (Vector3.Angle (transform.forward, dirToTarget) < viewAngle / 2) {
                float dstToTarget = Vector3.Distance (transform.position, target.position);
                if (!Physics.Raycast (transform.position, dirToTarget, dstToTarget, obstacleMask)) {
                    visibleTargets.Add(target);
                    Debug.Log("Target Acquired");
                }
            }
        }
    }

    void FindVisibleTarget() {
        Collider[] targetsInViewRadius = Physics.OverlapSphere (transform.position, viewRadius, targetMask);
        if (targetsInViewRadius.Length == 1) {
            Transform target = targetsInViewRadius [0].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;
            if (Vector3.Angle (transform.forward, dirToTarget) < viewAngle / 2) {
                float dstToTarget = Vector3.Distance (transform.position, target.position);
                if (!Physics.Raycast (transform.position, dirToTarget, dstToTarget, obstacleMask)) {
                    visibleTarget = target;
                    lostQuarry = false;
                    //Debug.Log("Target Acquired");
                } else {
                    visibleTarget = null;
                    lostQuarry = true;
                    Debug.Log("Target Blocked");
                }
            }
        } else {
            if (visibleTarget) {
                visibleTarget = null;
                lostQuarry = true;
                Debug.Log("Target Lost");
            }
        }
    }


    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal) {
        if (!angleIsGlobal) {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad),0,Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}