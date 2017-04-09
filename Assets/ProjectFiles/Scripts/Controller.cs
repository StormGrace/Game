﻿using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class Controller : MonoBehaviour
{
    [Header("Player States")]
    public bool OnGround;
    public bool IsFalling;
    public bool IsDead;
    public bool FacingRight; 
    public bool NeutralDirection;

    [Header("Player Movement")]
    public int MaxSpeed;
    public float AccelRate;
    public float DeccelRate;
    public float EdgeOffset;
    public int MeasureNormalizer;

    [Header("Player Jump")]
    public float JumpHeight;
    public float JumpSpeed;
    public float FallingSpeed;
    public int AirControlSpeed;
    public float MaxFallingTime;

    [Header("General Variables")]
    public float TravelledHeight;
    private float MoveSpeed, HeightToFinish, TravelledDistance, LastHeight;
    private Animator CubeAnimation;
    private Rigidbody2D RgdBdy;
    public GameObject Spawn;
    private Vector2 LastPosition;
   
    void Awake(){
        CubeAnimation = GetComponent<Animator>();
        RgdBdy = GetComponent<Rigidbody2D>();
    }

    void Start(){
        OnGround = true;
        NeutralDirection = true;
        LastPosition = transform.position;
    }

    void LateUpdate() { LastHeight = transform.localPosition.y; }

    void Update() {Measures();}

    void FixedUpdate(){
        float Horizontal = Input.GetAxisRaw("Horizontal");

        Movement(Horizontal);
        Jump();
        OnFall();
        Animations(Horizontal);
       
        //Pickup(Vector2.zero, PickupRadius);
    }
    private void Jump()
    {
        if (Input.GetButtonDown("Jump") && OnGround && !IsFalling)
        {
            RgdBdy.velocity = (new Vector2(RgdBdy.velocity.x, 750 * Time.deltaTime));
        }
    }
    private void Movement(float Horizontal){

        //Check if the Player is ready to move.
        if (Horizontal != 0 && !IsFalling && !IsDead)
        {
            NeutralDirection = false;

            if (!OnGround) {MaxSpeed = AirControlSpeed;}else{MaxSpeed = 18;}

            MoveSpeed = Mathf.Lerp(Horizontal * RgdBdy.velocity.x, MaxSpeed, Time.deltaTime * AccelRate); // Acceleration Smoother.
            RgdBdy.velocity = new Vector2(Horizontal * MoveSpeed, RgdBdy.velocity.y);   // Apply the Movement variable to the velocity of the object.

            //[FDP]
            if (Horizontal < 0) { MoveSpeed = MoveSpeed * -1; FacingRight = false; } //Reverse the speed in the opposite and change the current direction.
            else if (Horizontal > 0) { FacingRight = true; }
            else {FacingRight = false; }

        }
        //If the Player stops moving, decrease the speed by smoothing it.
        else
        {
            NeutralDirection = true; FacingRight = false;
            MoveSpeed = Mathf.Lerp(RgdBdy.velocity.x, 0, Time.deltaTime * DeccelRate); // Decceleration Smoother.
            RgdBdy.velocity = new Vector2(MoveSpeed, RgdBdy.velocity.y); // Apply the Movement variable to the velocity of the object.
        }

    }

    private void Animations(float Horizontal)
    {
       //Incase The Falling boolean is true, but the Player isn't falling.
       CubeAnimation.SetBool("Falling", IsFalling);
  
        if (CubeAnimation.GetCurrentAnimatorStateInfo(0).IsName("fall"))
        {
            CubeAnimation.speed = 1 / (MaxFallingTime / 4); //Get the duration needed per one frame for the Falling animation.;

            Debug.Log("Playing the Fall");
        }
        else {CubeAnimation.SetFloat("Input", Horizontal); Debug.Log("NOT Playing the Fall!");}
    }

    private void FallOfEdge(Collision2D coll)
    {
        if (OnGround && !IsFalling && !IsDead)
        {
            BoxCollider2D Collision = null;

            if (coll.collider != null) {Collision = coll.collider.GetComponent<BoxCollider2D>();} //Get the BoxCollider of the object the RayCast is hitting.

            if (((transform.position.x - EdgeOffset) >= Collision.bounds.max.x && RgdBdy.velocity.normalized.x >= 0) || ((transform.position.x + EdgeOffset) <= Collision.bounds.min.x) && RgdBdy.velocity.normalized.x <= 0) //Check if the Player is off the Platform collider (edge).
            {
                IsFalling = true; OnGround = false; RgdBdy.freezeRotation = false;     
                RgdBdy.inertia = 0.18F; //Add additional rotation when the Player is falling.
                RgdBdy.velocity = new Vector2(RgdBdy.velocity.x + (2 * RgdBdy.velocity.normalized.x), RgdBdy.velocity.y); //Add additional velocity to the X axis (incase the Player doesn't fall).
            }
            else {IsFalling = false; RgdBdy.freezeRotation = true;}

            //[FDP]
          
        }
    }
    private void OnFall()
    {
        float FallingTimer = 0;

        if (IsFalling && !IsDead)
        {
            Physics2D.IgnoreLayerCollision(9, 10, true);

            if (FallingTimer >= MaxFallingTime || OnGround)
            {
                IsFalling = false;
                //Debug.Log("Player is Dead!");
            }
            else
            {
                IsDead = true;
                FallingTimer += Time.deltaTime;

                //Debug.Log("Timer" + Mathf.Ceil(FallingTimer));
                //Debug.Log("Player is Falling!");
            }
        }
    }
    private void Measures()                            
    {
        if (!IsFalling && !IsDead)
        {
            //Calculate the distance travelled by adding the LastPosition with the current.
            TravelledDistance += (new Vector2(transform.position.x, transform.position.y) - LastPosition).magnitude * MeasureNormalizer;
            LastPosition = transform.position;

            //Calculate the distance travelled on the Y-axis.
            TravelledHeight= (Vector2.Distance(new Vector2(0, Spawn.transform.position.y), new Vector2(0, transform.position.y))) * MeasureNormalizer; 
                        
            //[FDP]
            //Debug.Log("TravelledDistance" + Mathf.Ceil(TravelledDistance));
            //Debug.Log("TravelledHeight" + Mathf.Ceil(TravelledHeight));
        }
    }
    private void OnCollisionStay2D(Collision2D coll)
    {
        if (coll.collider.gameObject.tag == "Ground" && RgdBdy.velocity.y < 2)
        {
            OnGround = true;
            FallOfEdge(coll);
        }
    }
    private void OnCollisionExit2D(Collision2D coll)
    {
        if (IsFalling) {coll.collider.gameObject.layer = LayerMask.NameToLayer("FellFrom");}
        OnGround = false;
    }
}










