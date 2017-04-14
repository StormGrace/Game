using UnityEngine;

public class CamFollow : MonoBehaviour
{
    private Level LevelScript;
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

    private void Awake()
    {
    }
    void Start()
    {
        
    }

    private void Update()
    {
        if (ZoomOut)
        {
            Cam.orthographicSize = Mathf.Lerp(Cam.orthographicSize, MaxZoomOut, ZoomSpeed * Time.deltaTime);
            Background.transform.localScale = new Vector3(Camera.main.orthographicSize * 2 * Camera.main.aspect, Camera.main.orthographicSize * 2, 1);
        }
        else
        {
            Cam.orthographicSize = Mathf.Lerp(Cam.orthographicSize, 4, ZoomSpeed * Time.deltaTime);
        }
        
    }

    void FixedUpdate()
    {
        if (ZoomOut)
        {
            if (SmoothFollow && Player.transform.hasChanged)
            {
                Vector3 PlayerPos = Player.transform.position + CameraOffset;
                Vector3 CameraFollow = Vector3.SmoothDamp(transform.position, PlayerPos, ref Vel, (FollowSpeed * Time.smoothDeltaTime));
                transform.position = CameraFollow;
                Player.transform.hasChanged = false;
            }
        }
    }
}
