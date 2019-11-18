using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LightLineOfSight: MonoBehaviour{
    public bool foundSomething;
    private bool maybeFoundSomething;
    public GameObject collisionObject;
    public LayerMask layerMask;
    public float timeTillLost = 1.0f;
    private Collider collider;
    private GameObject parent;
    private bool maybeLost;

    void Start()
    {
        collider = GetComponent<Collider>();
        parent = this.transform.parent.gameObject;
        parent = parent.transform.gameObject;
    }

    void FixedUpdate() {
        RaycastHit hit;
        int layerMask = 1<<8;
        if (maybeFoundSomething) {
            float distance = Vector3.Distance(parent.transform.position, collisionObject.transform.position);
            if (Physics.Raycast(parent.transform.position, parent.transform.TransformDirection(Vector3.forward), out hit, distance, layerMask)){
                //Debug.DrawRay(parent.transform.position, parent.transform.TransformDirection(Vector3.forward) * distance, Color.red, 5.0f);         
                foundSomething = false;
                collisionObject = null;
            } else {
                Bounds bound = collisionObject.GetComponent<Renderer>().bounds;
                Vector3 center = bound.center;
                Vector3 normal = center.normalized;
                Debug.Log("collision");
                //Debug.DrawRay(parent.transform.position, max - parent.transform.position, Color.green, 5.0f);
                foundSomething = true;
            }
            maybeFoundSomething = false;
        }

    }

    void OnTriggerEnter(Collider c) {
        if(c.gameObject.CompareTag("Detectable")) {
            //Debug.Log(c.GetComponent<Collider>().name);
            GameObject gameObject = c.gameObject;
            if (gameObject != null) {
                if (!foundSomething) {
                    float distance = Vector3.Distance(parent.transform.position, gameObject.transform.position);
                    //Debug.DrawRay(parent.transform.position, parent.transform.TransformDirection(Vector3.forward) * distance, Color.black, 5.0f);
                    
                    maybeFoundSomething = true;
                    collisionObject = gameObject;
                } else {
                    //making sure you don't switch targets
                    if (collisionObject == gameObject) {
                        maybeFoundSomething = true;
                    }                   
                }
                Debug.Log("maybe a collision");
                //collider.enabled = false;
                //StartCoroutine(afterTrigger());
            }
            maybeLost = false;
        }
    }

    IEnumerator afterTrigger() {
        yield return new WaitForSeconds(timeTillLost);
        if (maybeLost) {
            //check if it still has not been found
            collisionObject = null;
            foundSomething = false;
            maybeFoundSomething = false;
            Debug.Log("moving to last seen point");
        }
    }

    void OnTriggerExit(Collider c) {
        if (c.gameObject.CompareTag("Detectable") && foundSomething) {
            Debug.Log("maybe lost quarry");
            maybeLost = true;
            foundSomething = false;
            StartCoroutine(afterTrigger());
        }
    }
}