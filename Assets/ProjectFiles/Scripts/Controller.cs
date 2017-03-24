using UnityEngine;
using System;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class Controller : MonoBehaviour
{
    [Header("Player Movement")]
    public bool OnGround;
    public int MaxSpeed;
    public float AccelRate;
    public float DeccelRate;
    
    [Header("Player Direction")]
    public bool FacingRight;
    public bool NeutralDirection;

    [Header("Private Variables")]
    private GameObject FinishObj;
    private GameObject StartObj;
    public GameObject FlagObj;
    public GameObject FLAG;

    [Header("General Variables")]
    private Animator CubeAnimation;
    private Rigidbody2D Rb;
   
    [Header("Measurement Variables")]
    private float MoveSpeed;
    private float HeightToFinish;
    private float TravelledDistance;
    public float PickupRadius;

    private Vector2 LastPosition;
    private Vector2 StartPoint;
    private Vector2 ItemSpeed;

    private Vector2 FlagPoint ;
    private Vector2 PlayerPos;

    void Awake() {Rb = GetComponent<Rigidbody2D>(); CubeAnimation = GetComponent<Animator>();}

    void Start(){
        FinishObj = GameObject.FindGameObjectWithTag("Finish");
        StartObj = GameObject.FindGameObjectWithTag("Spawn");

        NeutralDirection = true; FacingRight = false;
        

        StartPoint = StartObj.transform.position;
        LastPosition = transform.position;
        PlayerPos = transform.position;
        FlagPoint = FlagObj.transform.localPosition; 
    }

    void Update()
    {
        MeasurementFactors();
    }

    void FixedUpdate()
    {
        float Horizontal = Input.GetAxisRaw("Horizontal");
        float Vertical = Input.GetAxisRaw("Vertical");

        FlagPointer();
        Movement(Horizontal);
        PickupArea(Vector2.zero, PickupRadius);
        Animations(Horizontal);
      
    }

    private void FlagPointer()
    {
         float RotationSpeed = 3f;

         Vector2 A = new Vector2(FlagObj.transform.localPosition.x, transform.position.y);
         Vector2 B = transform.position;
         Vector2 C = FlagObj.transform.localPosition;

         Quaternion TargetAngle;

         float Angle = Mathf.Sin(((Vector2.Distance(A, B))/(Vector2.Distance(C, B))));

         if(B.x < C.x){Angle = Angle * -1;}

         TargetAngle = Quaternion.Euler(new Vector3(0, 0, (Angle * Mathf.Rad2Deg)));
         FLAG.transform.rotation = Quaternion.Slerp(FLAG.transform.rotation, TargetAngle, RotationSpeed * Time.deltaTime);

        /*FDP:

        Debug.Log("Angle is: " + Mathf.Ceil(((Angle * Mathf.Rad2Deg))));

        Debug.DrawLine(FlagObj.transform.localPosition, new Vector2(FlagObj.transform.localPosition.x,transform.position.y), Color.red);
        Debug.DrawLine(new Vector2(FlagObj.transform.localPosition.x, transform.position.y), new Vector2(transform.position.x, transform.position.y), Color.red);
        Debug.DrawLine(new Vector2(transform.position.x, transform.position.y), new Vector2(FlagObj.transform.localPosition.x, FlagObj.transform.localPosition.y), Color.red); 
        */
    }

    private void IsOnGround()
    {
        
    }

    private void Movement(float Horizontal)
    { 

        if (Horizontal != 0)
        {
            NeutralDirection = false;
            MoveSpeed = Mathf.Lerp(Horizontal * Rb.velocity.x, MaxSpeed, Time.deltaTime * AccelRate); // Acceleration Smoother.
            Rb.velocity = new Vector2(Horizontal * MoveSpeed, Rb.velocity.y);   // Apply the Movement variable to the velocity of the object.

            //FDP 
            if (Horizontal < 0) { MoveSpeed = MoveSpeed * -1; FacingRight = false;}
            else if (Horizontal > 0){FacingRight = true;}
            else { FacingRight = false;} 

        }
        else
        {
            NeutralDirection = true;
            MoveSpeed = Mathf.Lerp(Rb.velocity.x, 0.0F, Time.deltaTime * DeccelRate); // Decceleration Smoother.
            Rb.velocity = new Vector2(MoveSpeed, Rb.velocity.y); // Apply the Movement variable to the velocity of the object.
        }
    }

    private void Animations(float Horizontal)
    {
        CubeAnimation.SetFloat("Input", Horizontal);
    }
 
    private float MeasurementFactors()
    {
        
        HeightToFinish = ((FinishObj.transform.position.y + FinishObj.transform.position.x) - (transform.localPosition.y + transform.localPosition.x) * 10);
        TravelledDistance += (new Vector2(transform.position.x, transform.position.y) - LastPosition).magnitude;

        LastPosition = transform.position;
        return HeightToFinish;
    }

    private void PickupArea(Vector2 Center, float PickupRadius)
    {
        Center = transform.position;
        Collider2D[] ItemColl = Physics2D.OverlapCircleAll(Center, PickupRadius, 1 << LayerMask.NameToLayer("Items"));
        
        float TempX;

        for (var i = 0; i < ItemColl.Length; i++)
        {
            for (var j = i + 1; j < ItemColl.Length; j++)
            {
       
                if (ItemColl[j].transform.position.x  > ItemColl[i].transform.position.x)
                {
                    Debug.Log("Array" + j);

                    TempX = ItemColl[i].transform.position.x;
                    ItemColl[i].transform.position = new Vector2(ItemColl[j].transform.position.x, 0);
                    ItemColl[j].transform.position = new Vector2(TempX, 0);

                }
            }
        }
       /* foreach(Collider2D Items in ItemColl)
        {
            Vector2.SmoothDamp(new Vector2(Items.transform.position.x, Items.transform.position.y), new Vector2(transform.position.x, transform.position.y), 
            ref ItemSpeed, 1, 100, Time.deltaTime);
        }*/
          
        }

    }



 






