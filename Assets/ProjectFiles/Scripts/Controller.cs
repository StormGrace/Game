using UnityEngine;
using System;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class Controller : MonoBehaviour
{
    Falling FallingScript;

    [Header("Player Movement")]
    public bool IsFalling;
    public bool OnGround;
    public float MaxFallingTime;
    public int MaxSpeed;
    public float AccelRate;
    public float DeccelRate;

    [Header("Player Jump")]
    public float JumpHeight;
    public float JumpSpeed;

    [Header("Player Direction")]
    public bool FacingRight;
    public bool NeutralDirection;

    [Header("Debugging Variables")]
    public float RayFallDistance = 1.5f;
    public Color RayFallColor = Color.red;

    [Header("Private Variables")]
    private GameObject FinishObj;
    private GameObject StartObj;

    [Header("General Variables")]
    private Animator CubeAnimation;
    private Rigidbody2D Rb;

    [Header("Measurement Variables")]
    private BoxCollider2D Collision;
    private float MoveSpeed;
    private float HeightToFinish;
    private float TravelledDistance;
    private float PickupRadius;

    [Header("Vector2 Variables")]
    private Vector2 LastPosition;
    private Vector2 StartPoint;
    private Vector2 ItemSpeed;
    private Vector2 PlayerPos;

    void Awake()
    {
        FallingScript = GetComponent<Falling>();
        CubeAnimation = GetComponent<Animator>();
        Rb = GetComponent<Rigidbody2D>();
    }
    void Start()
    {
        FinishObj = GameObject.FindGameObjectWithTag("Finish");
        StartObj = GameObject.FindGameObjectWithTag("Spawn");

        OnGround = true; NeutralDirection = true;

        StartPoint = StartObj.transform.position;
        LastPosition = transform.position;
        PlayerPos = transform.position;
    }

    void Update(){FallOfEdge();}

    void FixedUpdate()
    {
        float Horizontal = Input.GetAxisRaw("Horizontal");
        float Vertical = Input.GetAxisRaw("Vertical");

        Movement(Horizontal);
        Animations(Horizontal, IsFalling);
        Pickup(Vector2.zero, PickupRadius);

    }
    private void Movement(float Horizontal)
    {

        if (Horizontal != 0 && !IsFalling)
        {
            NeutralDirection = false;
            MoveSpeed = Mathf.Lerp(Horizontal * Rb.velocity.x, MaxSpeed, Time.deltaTime * AccelRate); // Acceleration Smoother.
            Rb.velocity = new Vector2(Horizontal * MoveSpeed, Rb.velocity.y);   // Apply the Movement variable to the velocity of the object.

            //[FDP]
            if (Horizontal < 0) { MoveSpeed = MoveSpeed * -1; FacingRight = false; } //Reverse the speed in the opposite and change the current direction.
            else if (Horizontal > 0) { FacingRight = true; }
            else { FacingRight = false; }

        }
        else
        {
            NeutralDirection = true;
            MoveSpeed = Mathf.Lerp(Rb.velocity.x, 0, Time.deltaTime * DeccelRate); // Decceleration Smoother.
            Rb.velocity = new Vector2(MoveSpeed, Rb.velocity.y); // Apply the Movement variable to the velocity of the object.
        }
    }

    private void Animations(float Horizontal, bool IsFalling)
    {
        CubeAnimation.SetFloat("Input", Horizontal);
        CubeAnimation.SetBool("Falling", IsFalling);
    }

    private void FallOfEdge()
    {
        float RayDistance = RayFallDistance * transform.localScale.y; //Change Ray's distance with Player's size.

        if (OnGround)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, RayDistance, 9 << LayerMask.NameToLayer("Platform"));

            if (hit.collider != null) { Collision = hit.collider.GetComponent<BoxCollider2D>(); }

            if ((transform.localPosition.x + 0.05) > Collision.bounds.max.x || (transform.localPosition.x + 0.05) < Collision.bounds.min.x)
            {
                IsFalling = true; OnGround = false; Rb.freezeRotation = false;

                Rb.inertia = 1; //Add additional rotation when the Player is falling.
                Rb.velocity = new Vector2(Rb.velocity.x + 0.1f, Rb.velocity.y); //Add velocity to X axis (incase it doesn't fall).

                FallingScript.OnFall(IsFalling); //[FC] Call the Falling Function from the script named "Falling".
            }
            else
            {
                IsFalling = false; Rb.freezeRotation = true;
            }

        }
        //[FDP]
        Debug.DrawRay(transform.position, Vector2.down * RayDistance, RayFallColor);
    }

    private void Pickup(Vector2 Center, float PickupRadius)
    {
        Center = transform.position;
        Collider2D[] ItemColl = Physics2D.OverlapCircleAll(Center, PickupRadius, 1 << LayerMask.NameToLayer("Items"));

        float TempX;

        for (var i = 0; i < ItemColl.Length; i++)
        {
            for (var j = i + 1; j < ItemColl.Length; j++)
            {

                if (ItemColl[j].transform.position.x > ItemColl[i].transform.position.x)
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










