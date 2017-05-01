using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class Controller : MonoBehaviour
{
    public GameObject Spawn;

    [Header("Player States")]
    public bool OnGround;
    public bool IsFalling;
    public bool IsDead;
    public bool FacingRight; 
    public bool NeutralDirection;
    private bool JumpButton;

    [Header("Player Movement")]
    public int MaxSpeed;
    public float AccelRate;
    public float DeccelRate;
    public float EdgeOffset;
    public int MeasureNormalizer;

    [Header("Player Jump")]
    public float JumpPower;
    public float JumpSpeed;
    public int AirControlSpeed;
    public float MaxFallingTime;

    [Header("General Variables")]
    public float TravelledHeight;
    private float MoveSpeed, TravelledDistance, LastHeight;

    //Audio Part.
    private AudioManager AudioManagerScript;
    private AudioSource Audio;
    //

    //Other Variables.
    private BoxCollider2D PlayerSize;
    private GameObject Manager;
    private Animator CubeAnimation;
    private Rigidbody2D RgdBdy;
    private Vector2 LastPosition;
    private float Horizontal;
    //

    private void Awake(){
  
        Manager = GameObject.FindGameObjectWithTag("Manager");
        AudioManagerScript = Manager.GetComponent<AudioManager>();

        Audio = GetComponent<AudioSource>();
        CubeAnimation = GetComponent<Animator>();
        RgdBdy = GetComponent<Rigidbody2D>();
        PlayerSize = GetComponent<BoxCollider2D>();   
    }

    private void Start(){
        OnGround = true;
        NeutralDirection = true;
        LastPosition = transform.position;
    }
    private void LateUpdate() { LastHeight = transform.localPosition.y; }
    private void Update() {
        //Inputs
        Horizontal = Input.GetAxisRaw("Horizontal");
        JumpButton = Input.GetButtonDown("Jump");
        Jump();
        Measures();
    }

    private void FixedUpdate(){
        Movement(Horizontal); OnFall(); Animations(Horizontal);
    }

    private void Movement(float Horizontal){

        //Check if the Player is ready to move.
        if (Horizontal != 0 && !IsFalling && !IsDead)
        {
            NeutralDirection = false;

            if (!OnGround) {MaxSpeed = AirControlSpeed;} else{MaxSpeed = 18;}

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

    private void Jump()
    {
        if (JumpButton && OnGround && !IsFalling)
        {
            AudioManagerScript.PlaySoundEffect(AudioManagerScript.Jump);
            RgdBdy.velocity = (new Vector2(RgdBdy.velocity.x, JumpPower * Time.fixedDeltaTime));
        }
    }

    private void FallOfEdge(Collision2D coll)
    {
        if (!IsFalling && !IsDead)
        {
            BoxCollider2D Collision = null;
            
            if (coll.collider != null) {

                Collision = coll.collider.GetComponent<BoxCollider2D>(); //Get the BoxCollider of the object, the RayCast is hitting

                if(transform.position.y > Collision.bounds.max.y)
                {
                    OnGround = true;
                }
            }  

            if (((transform.position.x - EdgeOffset) >= Collision.bounds.max.x && RgdBdy.velocity.normalized.x >= 0) || ((transform.position.x + EdgeOffset) <= Collision.bounds.min.x) && RgdBdy.velocity.normalized.x <= 0) //Check if the Player is off the Platform collider (edge).
            {
                IsFalling = true; OnGround = false; RgdBdy.freezeRotation = false;     
                RgdBdy.inertia = 0.18F; //Add additional rotation when the Player is falling.
                RgdBdy.velocity = new Vector2(RgdBdy.velocity.x + (2 * RgdBdy.velocity.normalized.x), RgdBdy.velocity.y); //Add additional velocity to the X axis (incase the Player doesn't fall).
              
                Physics2D.IgnoreLayerCollision(9, 15, true);
            }
            else {IsFalling = false; RgdBdy.freezeRotation = true;}

            //[FDP]
        }
    }
    private void OnFall()
    {
        if (IsFalling && !IsDead)
        {
            float FallingTimer = 0;

            if (FallingTimer >= MaxFallingTime || (OnGround && RgdBdy.velocity.normalized.y == -1))
            {
                IsFalling = false;
                IsDead = true;
            }
            else
            {
                FallingTimer += Time.deltaTime;
            }
        }

        Vector2 maxbound = Camera.main.ViewportToWorldPoint(new Vector2(1, transform.position.y));
        Vector2 minbound = Camera.main.ViewportToWorldPoint(new Vector2(0, transform.position.y));

        if (transform.position.x + PlayerSize.bounds.extents.x > maxbound.x )
        {
            transform.position = new Vector2(maxbound.x - PlayerSize.bounds.extents.x, transform.position.y);
        }

        else if (transform.position.x - PlayerSize.bounds.extents.x < minbound.x )
        {
            transform.position = new Vector2(minbound.x + PlayerSize.bounds.extents.x, transform.position.y);
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
            TravelledHeight = (Vector2.Distance(new Vector2(0, Spawn.transform.position.y), new Vector2(0, transform.localPosition.y))) * MeasureNormalizer;                     
        }
    }

    private void Animations(float Horizontal)
    {
        //Incase The Falling boolean is true, but the Player isn't falling.
        CubeAnimation.SetBool("Falling", IsFalling);

        if (CubeAnimation.GetCurrentAnimatorStateInfo(0).IsName("fall"))
        {
            CubeAnimation.speed = 1 / (MaxFallingTime / 4); //Get the duration needed per one frame for the Falling animation.;
        }
        else { CubeAnimation.SetFloat("Input", Horizontal); }
    }

    private void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.layer == LayerMask.NameToLayer("Items"))
        {
            AudioManagerScript.PlaySoundEffect(AudioManagerScript.CoinPickup);
            Destroy(coll.gameObject);
            Stats.NumOfCoins++;
        }
    }
    private void OnCollisionStay2D(Collision2D coll)
    {
        if (coll.collider.gameObject.tag == "Ground")
        {
            FallOfEdge(coll);
        }
    }
    private void OnCollisionExit2D(Collision2D coll)
    {
        if (IsFalling) {coll.collider.gameObject.layer = LayerMask.NameToLayer("FellFrom");}
        OnGround = false;
    }
}










