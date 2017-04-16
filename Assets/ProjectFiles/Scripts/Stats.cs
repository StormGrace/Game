using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Stats : MonoBehaviour {
    private Controller ControllerScript;
    private GameObject Player;

    public Text HeightTxt;
 
    private void Start () {
        Player = GameObject.FindGameObjectWithTag("Player");
        ControllerScript = Player.GetComponent<Controller>();
    }

	private void LateUpdate () { if (ControllerScript.transform.hasChanged){  StartCoroutine(UpdateWait()); transform.hasChanged = false;  }}

    IEnumerator UpdateWait()
    {
        HeightTxt.text = "" + ControllerScript.TravelledHeight.ToString("000 m");
        yield return null;
    }
}
