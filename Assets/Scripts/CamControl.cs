using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamControl : MonoBehaviour
{
    public ParticleSystem correctArrowEmission;
    public TagManager tagManager;
    public Transform Hand;

    public Animator animator;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            this.GetComponent<Rigidbody>().AddForce(Vector3.up * 200);
        }
        if (Input.GetMouseButtonDown(1))
        {
            correctArrowEmission.transform.position = tagManager.CurrentTag.transform.position;
            var shape = correctArrowEmission.shape;
            shape.position = correctArrowEmission.transform.InverseTransformPoint(Hand.position);
            correctArrowEmission.Play();
        }


        PlatyAction platyAction = PlatyAction.Idle;
        if (Input.GetKeyDown(KeyCode.J))
        {
            animator.SetTrigger("Jump");
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            animator.SetBool("Run", true);
        }
        else if (Input.GetKeyUp(KeyCode.W))
        {
            animator.SetBool("Run", false);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            animator.SetBool("Slice", true);
        }
        else if (Input.GetKeyUp(KeyCode.S))
        {
            animator.SetBool("Slice", false);
        }

       // this.transform.position += new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
    }

    private void AnimatorCollor(PlatyAction platyAction)
    {
        switch (platyAction)
        {
            case PlatyAction.Idle:
                animator.Play("Unarmed Idle 01");
                break;
            case PlatyAction.Run:
                animator.Play("Fast Run");
                break;
            case PlatyAction.FastRun:
                animator.Play("Fast Run");
                break;
            case PlatyAction.Jump:
                animator.Play("Jumping Up");
                break;
            case PlatyAction.Slice:
                animator.Play("Running Slide");
                break;
            default:
                break;
        }

    }


}

public interface BaseState
{
    void HandleInput(ref PlatyAction state, GameObject Player);
}


public enum PlatyAction
{
    Idle,
    Run,
    FastRun,
    Jump,
    Slice,
}

public enum PlatyState
{
    InGroud,
    InAir,
}