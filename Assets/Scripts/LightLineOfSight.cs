using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LightLineOfSight: MonoBehaviour{
    void OnTriggerEnter(Collider c) {
        if(c.attachedRigidbody != null) {
            GameObject gameObject = c.attachedRigidbody.gameObject;
            if (gameObject != null) {
                //Debug.Log("collision");
            }
        }
    }
}