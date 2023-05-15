using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ParticleCollision : MonoBehaviour
{

    public ParticleSystem HitBack;

    public bool isEnergy;

    public Cinemachine.CinemachineImpulseSource MyInpulse1;

    [SerializeField] Renderer[] characterRenderers;

    float amount = 0;

    void Complete(Renderer renderer)
    {
        renderer.material.DOFloat(0, "_Alpha", .3f).OnComplete(() => amount = 0);
    }


    private void OnParticleCollision(GameObject other)
    {
        //��Ļ�ζ� 
        MyInpulse1.GenerateImpulse();
        if (isEnergy)
        {
            //��������
            CamControl.Instance.RunSliderAdd();

            if (amount < 1)
            {
                amount++;

                foreach (Renderer renderer in characterRenderers)
                {
                    renderer.material.DOFloat(1, "_Alpha", .2f).OnComplete(() => Complete(renderer));
                }

                Debug.Log(111);
            }
        }

        else
        {

            TagManager.Instance.CurrentTag?.Hit();
            //��������
            var shape = HitBack.shape;
            shape.position = HitBack.transform.InverseTransformPoint(TagManager.Instance.CurrentTag.transform.position);

            HitBack.Play();
        }


    }
}
