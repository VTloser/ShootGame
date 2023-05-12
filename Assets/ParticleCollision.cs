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
        //��Ļ�ζ� 
        MyInpulse1.GenerateImpulse();
        if (isEnergy)
        {
            //��������
            CamControl.Instance.RunSliderAdd();
        }

        else
        {
            TagManager.Instance.CurrentTag.Hit();
            //��������
            var shape = HitBack.shape;
            shape.position = HitBack.transform.InverseTransformPoint(TagManager.Instance.CurrentTag.transform.position);
            HitBack.Play();
        }


    }
}
