using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LightLineOfSight: MonoBehaviour{
    public bool foundSomething;
    private bool maybeFoundSomething;
    public GameObject collisionObject;
    public LayerMask layerMask;
    private Collider collider;
    private GameObject parent;

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
                Debug.DrawRay(parent.transform.position, parent.transform.TransformDirection(Vector3.forward) * distance, Color.red, 5.0f);         
                foundSomething = false;
                maybeFoundSomething = false;
                collisionObject = null;
            } else {
                Debug.Log("collision");
                foundSomething = true;
                maybeFoundSomething = false;
                //collider.enabled = false;
            }
        }

    }

    void OnTriggerEnter(Collider c) {
        if(c.gameObject.CompareTag("Detectable")) {
            //Debug.Log(c.GetComponent<Collider>().name);
            GameObject gameObject = c.gameObject;
            if (gameObject != null) {
                float distance = Vector3.Distance(parent.transform.position, gameObject.transform.position);
                //Debug.DrawRay(parent.transform.position, parent.transform.TransformDirection(Vector3.forward) * distance, Color.black, 5.0f);
                Debug.Log("maybe a collision");
                maybeFoundSomething = true;
                collisionObject = gameObject;
                //collider.enabled = false;
                //StartCoroutine(afterTrigger());
            }
        }
    }

    IEnumerator afterTrigger() {
        yield return new WaitForSeconds(.01f);
        Destroy(collisionObject);
        foundSomething = false;
        
    }

    // void OnTriggerExit(Collider c) {
    //     foundSomething = false;
    //     if (c.gameObject.CompareTag("Detectable")) {
    //         foundSomething = false;
    //     }

    //     //StartCoroutine(afterTrigger());
    // }
}