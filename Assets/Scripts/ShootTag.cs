using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class ShootTag : MonoBehaviour
{

    public float Distance;
    public Vector2 ScreenPos;
    public GameObject Image;
    public Camera camera;


    public GameObject arrow;


    private void Awake()
    {
        arrow = transform.Find("arrow").gameObject;
    }
    void Update()
    {
        ScreenPos = camera.WorldToScreenPoint(this.transform.position);
        Distance = Mathf.Abs(ScreenPos.x - Screen.width / 2);

    }



    private void OnParticleCollision(GameObject other)
    {

        arrow.SetActive(true);


        Invoke("DD", 2);
    }

    void DD()
    {
        arrow.SetActive(false);
    }
}
