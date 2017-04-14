using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Stats : MonoBehaviour {
    private Controller ControllerScript;
    private GameObject Player;

    public Text HeightTxt;
    public Text TimeTxt;
 
    private void Start () {
        Player = GameObject.FindGameObjectWithTag("Player");
        ControllerScript = Player.GetComponent<Controller>();
    }

	private void LateUpdate () { if (ControllerScript.transform.hasChanged){  StartCoroutine(UpdateWait(3)); transform.hasChanged = false;  }}

    IEnumerator UpdateWait(int Delay)
    {
        HeightTxt.text = "" + ControllerScript.TravelledHeight.ToString("000 m");
        yield return null;
    }

    /*IEnumerator UpdateNow()
    {
        TimeTxt.text = "TIME: " + Time.timeSinceLevelLoad;
        yield return null;
    }*/
}
