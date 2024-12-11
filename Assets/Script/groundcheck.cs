using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class groundcheck : MonoBehaviour
{
    PlayerLogic logicmovement;
    // private void OnTriggerEnter(Collider other)
    // {
    //     Debug.Log("Touch The Ground");
    // }
    // Start is called before the first frame update
    void Start()
    {
        logicmovement = this.GetComponentInParent<PlayerLogic>();
    }
    private void OnTriggerEnter(Collider other)
    {
        logicmovement.groundedchanger();
        Debug.Log("Touch The Ground");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
