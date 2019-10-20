using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class Wallrun : MonoBehaviour
{
    private bool canRun;
    private int jumpCount = 0;
    private RigidbodyFirstPersonController cc;
    private Rigidbody rb;
    public float runTime = 2f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        cc = GetComponent<RigidbodyFirstPersonController>();
        cc.advancedSettings.airControl = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (cc.Grounded) {
            jumpCount = 0;
        }

        if (Input.GetKeyDown(KeyCode.E) && !cc.Grounded && jumpCount <=1 && canRun) {

            Debug.Log("wall running");
            jumpCount += 1;
            rb.useGravity = false;
            StartCoroutine(AfterRun());
        }
    
    }

    IEnumerator AfterRun() {
        yield return new WaitForSeconds(runTime);
        canRun = false;
        rb.useGravity = true;
    }

    void OnTriggerEnter(Collider other) {
        if (other.tag == "Wall") {
            Debug.Log("hit wall");
            canRun = true;
        }
    }

    void OnTriggerExit(Collider other) {
        if (other.tag == "Wall") {
            Debug.Log("exit wall");
            canRun = false;
        }
    }
}
