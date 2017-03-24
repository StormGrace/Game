using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eyes : MonoBehaviour
{
    public Camera cam;

    public Vector2 EyesPivot;
    public GameObject EyesObject;
    public GameObject EyesPivotObj;

    void Start()
    {}

    // Update is called once per frame
    void Update()
    {
        Vector3 Mouse = Input.mousePosition;
        Mouse.z = 2;
     
        Vector2 EyesCenter = new Vector2(EyesObject.transform.position.x, EyesObject.transform.position.y);
        float Angle = Vector2.Angle(cam.ScreenToWorldPoint(Mouse), Vector2.left);

        EyesPivot = new Vector2(Mathf.Sin(Angle),Mathf.Cos(Angle));
        EyesPivotObj.transform.position = EyesPivot;
 
       
        Debug.Log("Mice Angle" + Angle);
     
    }
}
