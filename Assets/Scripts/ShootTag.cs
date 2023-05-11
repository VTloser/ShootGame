using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class ShootTag : MonoBehaviour
{

    public float Distance;
    public Vector2 ScreenPos;

    public Camera camera;


    private GameObject arrow;

    private ParticleSystem particleSystem;

    private void Awake()
    {
        arrow = transform.Find("arrow").gameObject;
        particleSystem = transform.GetComponentInChildren<ParticleSystem>();
    }
    void Update()
    {
        ScreenPos = camera.WorldToScreenPoint(this.transform.position);
        Distance = Mathf.Abs(ScreenPos.x - Screen.width / 2);

    }


    public void Hit()
    {

        arrow.SetActive(true);
        particleSystem.Play();

        Invoke("DD", 2);
    }



    void DD()
    {
        arrow.SetActive(false);
    }
}
