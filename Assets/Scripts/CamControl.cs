using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class CamControl : MonoBehaviour
{
    public ParticleSystem correctArrowEmission;
    public ParticleSystem MissArrowEmission;


    public TagManager tagManager;
    public Transform Hand;

    public Animator animator;

    public static CamControl Instance;



    public float _curentSpeed;

    public float MoveSpeed = 10;
    public float FastMoveSpeed = 30;

    public float RotationSpeed = 5;

    public int SaticJumpDelay = 400;
    public int RunJumpDelay = 300;


    public bool CanAirJump = false;

    public Camera camera;

    public Material Speedmaterial;

    PlatyState platyState = PlatyState.None;
    Rigidbody Rigidbody;

    public Slider RunSlider;

    private void Awake()
    {
        Rigidbody = GetComponent<Rigidbody>();
        Instance = this;

        _curentSpeed = MoveSpeed;

        slider = tagManager.Image.GetComponent<Slider>();
    }

    Slider slider;
    private async void Update()
    {
        if (Input.GetMouseButton(1))
        {
            slider.value += Time.deltaTime;
        }

        if (Input.GetMouseButtonUp(1))
        {
            if (slider.value > 0.4f && slider.value <= 0.6f || slider.value >= 0.9f)
            {

                correctArrowEmission.transform.position = tagManager.CurrentTag.transform.position;
                var shape = correctArrowEmission.shape;
                shape.position = correctArrowEmission.transform.InverseTransformPoint(Hand.position);
                correctArrowEmission.Play();

                Speedmaterial.SetFloat("_Opacity", 1);
                Speedmaterial.SetFloat("_Amount", 0);

                Speedmaterial.DOFloat(1, "_Amount", 0.5f).OnComplete(() =>
                {
                    Speedmaterial.SetFloat("_Opacity", 0);
                });

                SpeedADD();

                if ((platyState & PlatyState.InAir) == PlatyState.InAir && slider.value < 0.8)
                {
                    Time.timeScale = 0.2f;
                    await Task.Delay(500);
                    Time.timeScale = 1;

                    CanAirJump = true;
                }

            }
            else
            {
                MissArrowEmission.Play();
            }

            Slider t =  Resources.Load<Slider>("Scree_copy");
            Slider tt = GameObject.Instantiate(t, slider.transform.parent);

            tt.value = slider.value;

            tt.transform.position = slider.transform.position;
            tt.transform.localScale = slider.transform.localScale;
            tt.transform.rotation = slider.transform.rotation;

            tt.transform.DOScale(Vector3.one * 3, 0.5f).OnComplete(()=>
            {
                Slider p = tt;
                Destroy(p.gameObject);
            });

            DOTween.To(() => 1f, x =>
            {
                tt.transform.GetComponent<CanvasGroup>().alpha = x;
            }, 0, 0.5f);

            slider.value = 0;
        }



        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if ((platyState & PlatyState.InAir) != PlatyState.InAir)
            {
                if (RunSlider.value > 0.2f)
                {
                    animator.SetBool("Slice", true);
                    RunSlider.value -= 0.2f;

                    Speedmaterial.SetFloat("_Opacity", 1);
                    Speedmaterial.SetFloat("_Amount", 0);

                    Speedmaterial.DOFloat(1, "_Amount", 0.5f).OnComplete(() =>
                    {
                        Speedmaterial.SetFloat("_Opacity", 0);
                    });
                }
            }

        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            if ((platyState & PlatyState.InAir) != PlatyState.InAir)
            {
                animator.SetBool("Slice", false);
            }
        }



        // this.transform.position += new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        MoveControl();

        JumpControl();

    }

    public async void SpeedADD()
    {
        _curentSpeed += 10;
        await Task.Delay(500);
        _curentSpeed -= 10;


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