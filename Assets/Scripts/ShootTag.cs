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

    public bool CanShoot = true;


    public Material material;

    private void Awake()
    {
        arrow = transform.Find("arrow").gameObject;
        particleSystem = transform.GetComponentInChildren<ParticleSystem>();

        material = transform.Find("Quad").GetComponent<MeshRenderer>().material;
    }

    void Update()
    {
        ScreenPos = camera.WorldToScreenPoint(this.transform.position);
        Distance = Mathf.Abs(ScreenPos.x - Screen.width / 2);

        this.transform.LookAt(PlayManager.Instance.transform);
    }


    public void Hit()
    {

        arrow.SetActive(true);
        particleSystem.Play();

        Invoke("DD", 10);

        CanShoot = false;

        material.SetFloat("_EmissionEnabled", 0);
        material.SetColor("_EmissionColor", new Color(10 / 256, 10 / 256, 10 / 256, 1));
    }


    void DD()
    {
        arrow.SetActive(false);
        CanShoot = true;

        material.SetFloat("_EmissionEnabled", 1);
        material.SetColor("_EmissionColor", new Color(1, 1, 1, 1));
    }
}
