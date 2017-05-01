using UnityEngine;
using UnityEngine.UI;

public class Stats : MonoBehaviour
{
    private Controller ControllerScript;
    private GameObject Player;

    public Text Height;
    public Text TimerCounter;
    public Text Coins;
    public Text GameOver;

    public static int NumOfCoins;
    private float Seconds = 0, WaitTime = 0;

    private void Start()
    {
        GameOver.text = "";
        Player = GameObject.FindGameObjectWithTag("Player");
        ControllerScript = Player.GetComponent<Controller>();
    }

    private void LateUpdate()
    {
        if (ControllerScript.transform.hasChanged)
        {
            Height.text = "" + ControllerScript.TravelledHeight.ToString("000 m");
            Coins.text = " : " + NumOfCoins.ToString();
        }
    }

    private void FixedUpdate()
    {
        if (ControllerScript.IsFalling == false && ControllerScript.IsDead == false)
        {
            Seconds += Time.deltaTime;
            TimerCounter.text = Mathf.Floor(Seconds / 60).ToString("00 ") + " : " + Mathf.Floor(Seconds % 60).ToString(" 00");
        }
        else
        {
            GameOver.text = "GAME OVER!";
            WaitTime += Time.deltaTime;

            if (WaitTime >= 5){Application.Quit();}
        }
    }
}