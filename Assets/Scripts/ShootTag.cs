using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootTag : MonoBehaviour
{

    public float Distance;
    public Vector2 ScreenPos;
    public GameObject Image;
    public Camera camera;

    void Update()
    {
        ScreenPos = camera.WorldToScreenPoint(this.transform.position);
        Distance = Mathf.Abs(ScreenPos.x - Screen.width / 2);

    }
}
