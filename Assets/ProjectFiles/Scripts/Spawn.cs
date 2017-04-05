using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn : MonoBehaviour {
    private GameObject Player;

    void Awake() {Player = (GameObject)Instantiate(Resources.Load("Player"), transform.position, Quaternion.identity); }	
}
