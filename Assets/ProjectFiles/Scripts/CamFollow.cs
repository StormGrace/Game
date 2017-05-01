using UnityEngine;

public class CamFollow : MonoBehaviour
{
    private Controller ControllerScript;
    private Renderer BackgroundRenderer;

    public GameObject Background;
    public Camera Cam;

    public float MaxZoomOut;
    public float ZoomSpeed;
 
    public Transform Player;

    public bool SmoothFollow = true;
    public bool ZoomOut = false;

    public float FollowSpeed;

    public Vector3 CameraOffset;
    private Vector3 Vel = Vector3.zero;

    private void Start()
    {
        ControllerScript = Player.GetComponent<Controller>();
        BackgroundRenderer = Background.gameObject.GetComponent<Renderer>();
    }

    private void Update()
    {
        ScrollBackground();

        if (ZoomOut)
        {
            Cam.orthographicSize = Mathf.Lerp(Cam.orthographicSize, MaxZoomOut, ZoomSpeed * Time.deltaTime);
            Background.transform.localScale = new Vector3(Camera.main.orthographicSize * 2 * Camera.main.aspect, Camera.main.orthographicSize * 2 + 0.5F, 1);
        }
        else
        {
            Cam.orthographicSize = Mathf.Lerp(Cam.orthographicSize, 4, ZoomSpeed * Time.deltaTime);
        }
        
    }
    private void FixedUpdate()
    {
            if (ZoomOut && SmoothFollow && Player.transform.hasChanged && !ControllerScript.IsDead)  
            {
                if (ControllerScript.IsFalling) { FollowSpeed /= 2; } // Increase the Camera's FollowSpeed on Player Fall.

                Vector3 PlayerPos = Player.transform.position + CameraOffset;    
                Vector3 CameraFollow = Vector3.SmoothDamp(transform.position, new Vector3(Mathf.Clamp(PlayerPos.x, -5, 5), PlayerPos.y, PlayerPos.z), ref Vel, (FollowSpeed * Time.smoothDeltaTime));
                transform.position = CameraFollow;
            }
    }

    private void ScrollBackground()
    {
        float Reduced = 0;

        if (BackgroundRenderer.material.mainTextureOffset.y < 0.7F)
        {
            if (ControllerScript.TravelledHeight >= 110)
            {
                Reduced += ControllerScript.TravelledHeight / 1000;
            }
            else { Reduced = -1 + ControllerScript.TravelledHeight / 100; }

            BackgroundRenderer.material.mainTextureOffset = new Vector2(0, Mathf.Clamp(Reduced, -1, 0.7F));
        }
    }
}
