using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LightLineOfSight: MonoBehaviour{
    public bool foundSomething;
    public GameObject collisionObject;
    private Collider collider;

    void Start()
    {
        collider = GetComponent<Collider>();
    }

    

    void OnTriggerEnter(Collider c) {
        if(c.gameObject.CompareTag("Detectable")) {
            GameObject gameObject = c.gameObject;
            if (gameObject != null) {
                Debug.Log("collision");
                foundSomething = true;
                collisionObject = gameObject;
                collider.enabled = false;
                //StartCoroutine(afterTrigger());
            }
        }
    }

    IEnumerator afterTrigger() {
        yield return new WaitForSeconds(.01f);
        Destroy(collisionObject);
        foundSomething = false;
        
    }

    void OnTriggerExit(Collider c) {
        foundSomething = false;
        if (c.gameObject.CompareTag("Detectable")) {
            foundSomething = false;
        }

        //StartCoroutine(afterTrigger());
    }
}