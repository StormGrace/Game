using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class Controller : MonoBehaviour
{
    [Header("Player States")]
    public bool OnGround;
    public bool IsFalling;
    public bool IsDead;

    [Header("Player Direction")]
    public bool FacingRight, NeutralDirection;

    [Header("Player Movement")]
    public int MaxSpeed;
    public float AccelRate, DeccelRate, TravelledHeight;
    public int MeasureNormalizer;

    [Header("Player Jump")]
    public float JumpHeight;
    public float JumpSpeed;

    [Header("Player Fall")]
    public LayerMask RayLayerMask;
    public float MaxFallingTime;

    [Header("Debugging Variables")]
    private float FallingTimer = 0;
    public float RayFallDistance = 1.5f;
    private Color RayFallColor = Color.red;

    [Header("General Variables")]
    private GameObject Spawn;
    private Animator CubeAnimation;
    private Rigidbody2D Rb;
    
    [Header("Measurement Variables")]
    private BoxCollider2D Collision;
    private float MoveSpeed;
    private float HeightToFinish;
    private float TravelledDistance;
    private float PickupRadius;
    private float StartingPosition;

    [Header("Vector2 Variables")]
    private Vector2 LastPosition;
    private Vector2 ItemSpeed;
    private Vector2 PlayerPos;
    public float LastHeight;

    void Awake(){
        CubeAnimation = GetComponent<Animator>();
        Rb = GetComponent<Rigidbody2D>();
    }
    void Start(){
        OnGround = true; NeutralDirection = true;
  
        LastPosition = transform.position;
    }

    void LateUpdate() { LastHeight = transform.localPosition.y; }

    void Update() {FallOfEdge();}

    void FixedUpdate(){
        float Horizontal = Input.GetAxisRaw("Horizontal");

        OnFall();
        Movement(Horizontal);
        Animations(Horizontal);
        Measures();
        //Pickup(Vector2.zero, PickupRadius);
    }

    private void Movement(float Horizontal){

        //Check if the Player is ready to move.
        if (Horizontal != 0 && !IsFalling && !IsDead)
        {
            NeutralDirection = false;
            MoveSpeed = Mathf.Lerp(Horizontal * Rb.velocity.x, MaxSpeed, Time.deltaTime * AccelRate); // Acceleration Smoother.
            Rb.velocity = new Vector2(Horizontal * MoveSpeed, Rb.velocity.y);   // Apply the Movement variable to the velocity of the object.

            //[FDP]
            if (Horizontal < 0) { MoveSpeed = MoveSpeed * -1; FacingRight = false; } //Reverse the speed in the opposite and change the current direction.
            else if (Horizontal > 0) { FacingRight = true; }
            else { FacingRight = false; }

        }
        //If the Player stops moving, decrease the speed by smoothing it.
        else
        {
            NeutralDirection = true; FacingRight = false;
            MoveSpeed = Mathf.Lerp(Rb.velocity.x, 0, Time.deltaTime * DeccelRate); // Decceleration Smoother.
            Rb.velocity = new Vector2(MoveSpeed, Rb.velocity.y); // Apply the Movement variable to the velocity of the object.
        }
    }

    private void Animations(float Horizontal)
    {
        CubeAnimation.SetBool("Falling", IsFalling);
        //CubeAnimation.SetBool("Dead", IsDead);

        if (CubeAnimation.GetCurrentAnimatorStateInfo(0).IsName("fall"))
        {
            CubeAnimation.speed = 1 / (MaxFallingTime / 4); //Get the duration needed per one frame for the Falling animation.;

            Debug.Log("Playing the Fall");
        }
        else {CubeAnimation.SetFloat("Input", Horizontal); Debug.Log("NOT Playing the Fall!"); }
    }

    private void FallOfEdge()
    {
        float RayDistance = RayFallDistance * transform.localScale.y; //Change Ray's distance with Player size.

        if (OnGround && !IsFalling && !IsDead)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, RayDistance, RayLayerMask);  

            if (hit.collider != null) {Collision = hit.collider.GetComponent<BoxCollider2D>(); } //Get the BoxCollider of the object the RayCast is hitting.

            //if (hit.collider.gameObject.layer == LayerMask.NameToLayer("BasePlatform")) {bool OnBaseGround; }

            if (((transform.localPosition.x + 0.045) > Collision.bounds.max.x) || (transform.localPosition.x + 0.045) < Collision.bounds.min.x) //Check if the Player is off the Platform collider (edge).
            {
                IsFalling = true; OnGround = false; Rb.freezeRotation = false;

                Rb.inertia = 0.20F; //Add additional rotation when the Player is falling.
                Rb.velocity = new Vector2(Rb.velocity.x + (0.28F * Rb.velocity.normalized.x), Rb.velocity.y); //Add additional velocity to the X axis (incase the Player doesn't fall).
            }

            else {IsFalling = false; Rb.freezeRotation = true; }

            //[FDP]
            Debug.DrawRay(transform.position, Vector2.down * RayDistance, RayFallColor);
        }
    }
    private void OnFall()
    {
        if (IsFalling)
        {
            Physics2D.IgnoreLayerCollision(9, 10, true);

            if (FallingTimer >= MaxFallingTime || OnGround)
            {
                IsFalling = false;

                Debug.Log("Player is Dead!");
            }
            else
            {
                IsDead = true;
                FallingTimer += Time.deltaTime;

                Debug.Log("Timer" + Mathf.Ceil(FallingTimer));
                Debug.Log("Player is Falling!");
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
            if (transform.localPosition.y > LastHeight) { TravelledHeight = Mathf.Abs((transform.localPosition.y) + LastHeight) * MeasureNormalizer; }
        

            //[FDP]
            //Debug.Log("TravelledDistance" + Mathf.Ceil(TravelledDistance));
            Debug.Log("TravelledHeight" + Mathf.Ceil(TravelledHeight));
        }
    }

    /*private void Pickup(Vector2 Center, float PickupRadius)
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
         }

    }*/

    private void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.collider.gameObject.tag == "Ground") {OnGround = true;}
    }

    private void OnCollisionExit2D(Collision2D coll)
    {
        if (IsFalling) {coll.collider.gameObject.layer = LayerMask.NameToLayer("FellFrom");}
        OnGround = false;
    }
}










