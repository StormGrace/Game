using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Falling : MonoBehaviour {
    Controller ControllerScript;
    private float FallingTimer = 0;

    void Start(){ControllerScript = gameObject.GetComponent<Controller>();}

    void FixedUpdate () {OnFall(ControllerScript.IsFalling);}

    public void OnFall(bool IsFalling)
    {
        bool IsDead = false;
       
        if (IsFalling)
        {
            if (FallingTimer >= ControllerScript.MaxFallingTime)
            {
                IsFalling = false;
                Debug.Log("Player is Dead!");
            }

            else
            {
                IsDead = true;
                FallingTimer += Time.deltaTime;
                Debug.Log("Player is Falling!");
            }
        }
    }
}
