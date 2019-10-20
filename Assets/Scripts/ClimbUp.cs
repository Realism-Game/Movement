using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class ClimbUp : MonoBehaviour
{

    private bool canClimb;
    private Rigidbody rb;
    private RigidbodyFirstPersonController cc;
    public Animator anim;
    public Camera regularCam;
    public GameObject gameObject;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        cc = GetComponent<RigidbodyFirstPersonController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (canClimb && Input.GetKeyDown(KeyCode.E)) {
            cc.enabled = false;
            rb.isKinematic = true;
            anim.SetTrigger("Climb");
            StartCoroutine(afterClimb());
        }
    }

    IEnumerator afterClimb() {
        yield return new WaitForSeconds(1f);
        cc.enabled = true;
        rb.isKinematic = false;
        this.transform.position = gameObject.transform.position;
    }

    void OnTriggerEnter(Collider other) {
        if (other.tag == "Climb") {
            canClimb = true;
        }
    }

    void OnTriggerExit(Collider other) {
        if (other.tag == "Climb") {
            canClimb = false;
        }
    }
}
