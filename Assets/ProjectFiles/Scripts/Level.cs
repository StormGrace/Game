using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    private BoxCollider2D PlayerSize, PlatformSize;
    private GameObject Spawn, Player, Platform, FirstPlatform;

    private Controller ControllerScript;
    private Effector2D PlatformEffector;

    private int Height, DeletationCounter;
    private float PlatformPosition;
    private bool Clean;

    [Header("Generation Options")]
    public Transform Platforms;
    public int PlatformsPerSection;
    public float PlatformGap;
    public float SectionStep = 1000;
    public float GenerationStep;
    public float DeletionStep;
    public float MaxPlatformSize = 100;
    private float MinPlatformSize;
    public int SizeVariation = 100;
 
    List<GameObject> PlatformForDeletation;

    void Start()
    {
        Spawn = GameObject.FindGameObjectWithTag("Spawn");
        Player = GameObject.FindGameObjectWithTag("Player");
        Platform = GameObject.FindGameObjectWithTag("Platform");

        ControllerScript = Player.GetComponent<Controller>();
        PlayerSize = Player.GetComponent<BoxCollider2D>();

        Platforms.gameObject.AddComponent<BoxCollider2D>().usedByEffector = true;
        PlatformSize = Platforms.gameObject.GetComponent<BoxCollider2D>();

        PlatformSize.gameObject.layer = LayerMask.NameToLayer("Platform");
        PlatformSize.gameObject.tag = "Ground";

        Platforms.gameObject.AddComponent<PlatformEffector2D>().useOneWay = true;
        PlatformEffector = Platforms.gameObject.GetComponent<PlatformEffector2D>();
 
        MinPlatformSize = PlayerSize.bounds.size.x;
        PlatformPosition = Spawn.transform.position.y;

        PlatformForDeletation = new List<GameObject>();
    }

    void Update()
    {

        if (Height % GenerationStep  == 0)
        {
            GeneratePlatforms();
            Clean = false;
        }
        else if (PlatformForDeletation.Capacity > 12 && Clean == false)
        {
            for (int i = 1; i < PlatformsPerSection; i++)
            {
                Destroy(PlatformForDeletation[i - 1]);
            }

            PlatformForDeletation.RemoveRange(0, 3);
            Clean = true;
        }
        else
        {
            if ((Height - 1) + 10 < ControllerScript.TravelledHeight)
            {
                Height = ((int)(ControllerScript.TravelledHeight) / 10) * 10;
            }
        }

        Debug.Log("HEIGHT" + Height);
    }

    public void GeneratePlatforms()
    {
        Vector2 NewPosition = Vector2.zero;
        //Create a certain number of Platforms per a specified PlatformStep.
        for (int i = 1; i < PlatformsPerSection; i++)
        {
            Transform TempTrans = null; float TempPos, TempSize;

            TempPos = Random.Range(-PlatformGap, PlatformGap);
            TempSize = Random.Range(MinPlatformSize, MaxPlatformSize * SizeVariation / 100);

            NewPosition = new Vector2(TempPos, PlatformPosition + PlatformGap * i);
            TempTrans = Instantiate(Platforms, NewPosition, Quaternion.identity);  //Get the Platform Instance per 1 cycle.

            TempTrans.gameObject.transform.localScale = new Vector2(TempSize, 1);
            PlatformForDeletation.Add(TempTrans.gameObject); //Store all the Platform Instances in a List container.
        }
        Height += 1;
        PlatformPosition = NewPosition.y; //Save the Last Position for the Next Iteration Call.  
    }
}


