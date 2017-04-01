using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Falling : MonoBehaviour {
    private float FallingTimer = 0;
    Controller ControllerScript;

    void Start(){ControllerScript = gameObject.GetComponent<Controller>();}

    void FixedUpdate () {OnFall(ControllerScript.IsFalling);}

    public void OnFall(bool IsFalling)
    {
        bool IsDead = false;

        if (!IsDead)
        {
            if (FallingTimer >= ControllerScript.MaxFallingTime)
            {
                IsFalling = false;
                IsDead = true;
                Debug.Log("Player is Dead!");
            }

            else
            {
                IsDead = false;
                FallingTimer += Time.deltaTime;
                Debug.Log("Timer" + FallingTimer);
                Debug.Log("Player is Falling!");
            }
        }
    }
}
