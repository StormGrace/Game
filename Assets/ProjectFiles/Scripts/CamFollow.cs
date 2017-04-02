using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamFollow : MonoBehaviour
{
    public Camera Cam;

    public float MaxZoomOut;
    public float ZoomSpeed;

    public Transform Player;

    public bool SmoothFollow = true;
    public bool StartLevel = false;

    public float FollowSpeed;

    public Vector3 CameraOffset;
    private Vector3 Vel = Vector3.zero;

    void Start()
    {
       

    }
    private void Update()
    {
        if (StartLevel)
        {
            Cam.orthographicSize = Mathf.Lerp(Cam.orthographicSize, MaxZoomOut, ZoomSpeed * Time.deltaTime);
        }
        else
        {
            Cam.orthographicSize = Mathf.Lerp(Cam.orthographicSize, 4, ZoomSpeed * Time.deltaTime);
        }
    }
    void FixedUpdate()
    {
        if (StartLevel)
        {
            if (SmoothFollow)
            {
                Vector3 PlayerPos = Player.transform.position + CameraOffset;
                Vector3 CameraFollow = Vector3.SmoothDamp(transform.position, PlayerPos, ref Vel, (FollowSpeed * Time.smoothDeltaTime));
                transform.position = CameraFollow;
            }
            else
            {

            }
        }
    }

}
