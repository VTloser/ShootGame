using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class CamControl : MonoBehaviour
{
    public ParticleSystem correctArrowEmission;
    public TagManager tagManager;
    public Transform Hand;

    public Animator animator;

    public static CamControl Instance;



    public float _curentSpeed;

    public float MoveSpeed = 5;
    public float FastMoveSpeed = 10;

    public float RotationSpeed = 5;

    public int SaticJumpDelay = 400;
    public int RunJumpDelay = 300;


    public bool CanAirJump = false;

    public Camera camera;

    PlatyState platyState = PlatyState.None;
    Rigidbody Rigidbody;

    public Slider RunSlider;

    private void Awake()
    {
        Rigidbody = GetComponent<Rigidbody>();
        Instance = this;

        _curentSpeed = MoveSpeed;
    }


    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            correctArrowEmission.transform.position = tagManager.CurrentTag.transform.position;
            var shape = correctArrowEmission.shape;
            shape.position = correctArrowEmission.transform.InverseTransformPoint(Hand.position);
            correctArrowEmission.Play();
        }

        else if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            animator.SetBool("Slice", true);

        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            animator.SetBool("Slice", false);   
        }

        // this.transform.position += new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        MoveControl();

        JumpControl();

    }

    int _jumpDelay;
    async private void JumpControl()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _jumpDelay = SaticJumpDelay;
            if ((platyState & PlatyState.InAir) == PlatyState.InAir && !CanAirJump)
            {
                return;
            }

            if ((platyState & PlatyState.InAir) == PlatyState.InAir && CanAirJump) //空中二段跳
            {
                Rigidbody.Sleep();
                CanAirJump = false;
                //animator.SetTrigger("Jump");
                this.GetComponent<Rigidbody>().AddForce(Vector3.up * 300);
                return;
            }

            
            if ((platyState & PlatyState.InRun) == PlatyState.InRun) //跑步中跳跃
            {
                _jumpDelay = RunJumpDelay;
            }

            platyState = platyState | PlatyState.InAir;

            animator.SetTrigger("Jump");
            await Task.Delay(_jumpDelay);
            this.GetComponent<Rigidbody>().AddForce(Vector3.up * 300);
        }
    }

    private void MoveControl()
    {
        animator.SetBool("Run", false);
        platyState = platyState & ~PlatyState.InRun;
        if (Input.GetAxisRaw("Vertical") != 0 || Input.GetAxisRaw("Horizontal") != 0)
        {
            this.transform.Translate(Vector3.forward * Time.deltaTime * _curentSpeed);
            animator.SetBool("Run", true);

            platyState = platyState | PlatyState.InRun;
        }

        var _input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        _input = _input.normalized;

        if(_input != Vector3.zero)
        {
            var matrix = Matrix4x4.Rotate(Quaternion.Euler(0, camera.transform.eulerAngles.y, 0)); ;
            _input = matrix.MultiplyPoint3x4(_input);            //给输入乘上相机旋转

            var relative = (this.transform.position + _input) - this.transform.position;
            var rot = Quaternion.LookRotation(relative, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(this.transform.rotation, rot, RotationSpeed * Time.deltaTime);
        }

    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            if ((platyState & PlatyState.InAir) == PlatyState.InAir)
            {
                platyState = platyState & ~PlatyState.InAir;
                animator.SetTrigger("OnGround");
                CanAirJump = true;
            }
        }
    }



    public void RunSliderAdd()
    {
        RunSlider.value += 0.02f;
    }
}


public enum PlatyState
{
    None = 0,

    InAir = 2,

    InRun = 4,

}