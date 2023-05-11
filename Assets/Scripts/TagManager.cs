using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;
using UnityEngine.UI;

public class TagManager : MonoBehaviour
{
    public Image Image;
    public List<ShootTag> ALLTags = new List<ShootTag>();
    public List<ShootTag> UseTags = new List<ShootTag>();
    public Camera camera;

    public static TagManager Instance;

    private void Awake()
    {
        Instance = this;

        ALLTags = FindObjectsOfType<ShootTag>().ToList();

        foreach (var item in ALLTags)
        {
            item.camera = camera;
        }
    }


    float Min = 200; //  ÏñËØµ¥Î»Îó²î
    Vector2 Pos;
    public ShootTag CurrentTag;
    ShootTag LastCurrentTag;
    void Update()
    {
        foreach (var item in ALLTags)
        {
            if (Vector3.Dot(camera.transform.forward, (item.transform.position - camera.transform.position).normalized) < Mathf.Cos(camera.fieldOfView / 2 * Mathf.Deg2Rad))
            {
                if (UseTags.Contains(item))
                    UseTags.Remove(item);
            }
            else
            {
                if (!UseTags.Contains(item))
                    UseTags.Add(item);
            }
        }

        foreach (var item in UseTags)
        {
            if (item.Distance < Min)
            {
                Min = item.Distance;
                Pos = item.ScreenPos;
                CurrentTag = item;
            }
        }

        if (Min != 200)
        {
            Image.gameObject.SetActive(true);
            Image.transform.position = Pos;

            //Force_Throw.transform.LookAt(CurrentTag.transform.position);


        }
        else
        {
            Image.gameObject.SetActive(false);
            CurrentTag = null;
        }

        Min = 200;

        if (LastCurrentTag != CurrentTag)
        {
            Image.transform.localScale = Vector2.one * 4;
            Image.color = new Color(Image.color.r, Image.color.g, Image.color.b, 0);
            Image.transform.rotation = Quaternion.identity;

            Image.transform.DORotate(Vector3.forward * 180, 0.15f);
            Image.DOColor(new Color(Image.color.r, Image.color.g, Image.color.b, 1), 0.15f);
            Image.transform.DOScale(Vector2.one, 0.15f);
        }
        LastCurrentTag = CurrentTag;

    } 

    private void LateUpdate()
    {
        Image.transform.position = Pos;
    }

    private void FixedUpdate()
    {
        Image.transform.position = Pos;
    }



}
