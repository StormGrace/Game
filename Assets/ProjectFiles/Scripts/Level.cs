using UnityEngine;
using System.Collections.Generic;

public class Level : MonoBehaviour
{
    List<GameObject> PlatformForDeletation;

    private BoxCollider2D PlayerSize, PlatformSize;
    private GameObject Spawn, Player, Platform, Background;
    public GameObject BasePlatforms;

    private Controller ControllerScript;
    private Effector2D PlatformEffector;
    
    [Header("Generation Options")]
    public Transform Platforms;

    public int PlatformsPerSection;
    public int PlatformGap;
    public float SectionStep;
    public float GenerationStep;
    public float DeletionStep;
    public float MaxPlatformSize ;
    private float MinPlatformSize;
    public int SizeVariation;

    public int Height;
    private int DeletationCounter;
    private float PlatformPosition;
    private bool Clean, Base;

    private void Start()
    {
        Spawn = GameObject.FindGameObjectWithTag("Spawn");
        Player = GameObject.FindGameObjectWithTag("Player");
        Platform = GameObject.FindGameObjectWithTag("Platform");
        Background = GameObject.FindGameObjectWithTag("Background");
       
        ControllerScript = Player.GetComponent<Controller>();
        PlayerSize = Player.GetComponent<BoxCollider2D>();

        Platforms.gameObject.AddComponent<BoxCollider2D>().usedByEffector = true;
        PlatformSize = Platforms.gameObject.GetComponent<BoxCollider2D>();
        PlatformSize.size = new Vector2(PlatformSize.size.x, PlatformSize.size.y - 0.4F);

        PlatformSize.gameObject.layer = LayerMask.NameToLayer("Platform");
        PlatformSize.gameObject.tag = "Ground";

        Platforms.gameObject.AddComponent<PlatformEffector2D>().useOneWay = true;
        PlatformEffector = Platforms.gameObject.GetComponent<PlatformEffector2D>();
 
        MinPlatformSize = PlayerSize.bounds.size.x;
        PlatformPosition = Spawn.transform.position.y;

        PlatformForDeletation = new List<GameObject>();
    }

    private void Update()
    {
        if (Height % SectionStep == 0 && Height > 0) 
            GenerateBase();
        
        else if (Height % GenerationStep == 0) 
            GeneratePlatforms();  

        else if (PlatformForDeletation.Capacity > 12 && Clean == false) 
            CleanPlatforms();  

        else
        {
            if ((Height - 1) + PlatformGap < ControllerScript.TravelledHeight)
            {
                Height = ((int)(ControllerScript.TravelledHeight) / PlatformGap) * PlatformGap;
            }
        }
    }

    private void GenerateBase()
    {
        Destroy(BasePlatforms);
   
        Vector2 BasePosition = new Vector2(0, PlatformPosition + PlatformGap);
        PlatformPosition = BasePosition.y;
        BasePlatforms = Instantiate(PrefabManager.Instance.PlatformPrefab, BasePosition, Quaternion.identity);
        Height += 1;
    }

    private void GeneratePlatforms()
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

            PlatformPosition = NewPosition.y; //Save the Last Position for the Next Iteration Call.  

        Height += 1;
        Clean = false;
    }

    private void CleanPlatforms()
    {
        for (int i = 1; i < PlatformsPerSection; i++)
        {
                Destroy(PlatformForDeletation[i - 1]);
        }

        PlatformForDeletation.RemoveRange(0, 3);
        Clean = true;
    }

}



