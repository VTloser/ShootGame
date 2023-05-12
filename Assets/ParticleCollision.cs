using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ParticleCollision : MonoBehaviour
{

    public ParticleSystem HitBack;

    public bool isEnergy;

    public Cinemachine.CinemachineImpulseSource MyInpulse1;

    private void OnParticleCollision(GameObject other)
    {
        //屏幕晃动 
        MyInpulse1.GenerateImpulse();
        if (isEnergy)
        {
            //增加能量
            CamControl.Instance.RunSliderAdd();
        }

        else
        {
            TagManager.Instance.CurrentTag.Hit();
            //返回粒子
            var shape = HitBack.shape;
            shape.position = HitBack.transform.InverseTransformPoint(TagManager.Instance.CurrentTag.transform.position);
            HitBack.Play();
        }


    }
}
