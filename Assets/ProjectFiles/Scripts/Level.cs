using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour {

    private BoxCollider2D PlayerSize, PlatformLength;
    private GameObject Player, Platform;

    public Transform Platforms;
    private Controller ControllerScript;

    private float I = 0;

    private float PlatformDirection;
    public float PlatformGap = 3;
    private float SectionStep = 1000;
    private float PlatFormStep = 100;
    private float NumOfPlatforms;

    private int Height;

	void Start ()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        Platform = GameObject.FindGameObjectWithTag("Platform");
      
        ControllerScript = Player.GetComponent<Controller>();
        PlayerSize = Player.GetComponent<BoxCollider2D>();

        PlatformGap = (PlayerSize.bounds.size.x * PlatformGap);
    }
    void Update()
    {
        if (Height % PlatFormStep == 0)
        {
            GeneratePlatforms();
        }
        else
        {
            if ((Height - 1) + 100 < ControllerScript.TravelledHeight)
            {
                Height = ((int)(ControllerScript.TravelledHeight) / 100) * 100;
            }
        }

    }
    public void GeneratePlatforms()
    {
    
        Debug.Log("In Platform"); Height += 1;

         I++;
         Instantiate(Platforms, new Vector2(Random.Range(-PlatformGap, PlatformGap), PlatformGap * I), Quaternion.identity);
    }
}
