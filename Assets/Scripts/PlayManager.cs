using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Animations.Rigging;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine.Events;
using System.Linq;
using UnityEditor.Animations;
using UnityEngine.Animations;
using UnityEditor;

public class PlayManager : MonoBehaviour
{
    public ParticleSystem correctArrowEmission;
    public ParticleSystem MissArrowEmission;

    public Animator animator;

    public static PlayManager Instance;

    public float _curentSpeed;

    public float MoveSpeed = 10;
    public float FastMoveSpeed = 30;

    public float RotationSpeed = 5;

    public int SaticJumpDelay = 400;
    public int RunJumpDelay = 300;

    public bool CanAirJump = false;

    public Camera camera;

    public Material Speedmaterial;

    public MultiAimConstraint Aim;
    public TwoBoneIKConstraint Left_Ready;
    public TwoBoneIKConstraint Right_Ready;
    public TwoBoneIKConstraint Right_Forece;
    public MultiParentConstraint Row_Praent;
    public MultiPositionConstraint Row_;

    public GameObject AimTag;

    PlatyState platyState = PlatyState.None;

    Rigidbody Rigidbody;

    public Slider RunSlider;
    Slider ShootSlider;
    public float ShootSlider_faultTolerance = 0.1f;
    bool RowFinish;


    int _aAddSpeed = Animator.StringToHash("AddSpeed");
    int _aSlice = Animator.StringToHash("Slice");
    int _aJump = Animator.StringToHash("Jump");
    int _aDoubleJump = Animator.StringToHash("DoubleJump");
    int _aMoveSpeed = Animator.StringToHash("MoveSpeed");
    int _aOnGround = Animator.StringToHash("OnGround");


    private void Start()
    {
        Instance = this;
        Rigidbody = GetComponent<Rigidbody>();
        _curentSpeed = MoveSpeed;
        ShootSlider = TagManager.Instance.Image.GetComponent<Slider>();


        InputSystem.Instance._MouseButtonDown = Shoot_Ready;
        InputSystem.Instance._MouseButton = Shoot_During;
        InputSystem.Instance._MouseButtonUp = Shoot_Finish;

        InputSystem.Instance._LeftShiftDown = RunSliderBegin;
        InputSystem.Instance._LeftShiftUp = RunSliderEnd;
    }


    private void Update()
    {

        if (TagManager.Instance.CurrentTag != null)
        {
            AimTag.transform.position = TagManager.Instance.CurrentTag.transform.position;
        }

        OnGroundRay();

        MoveControl();

        JumpControl();

    }



    private void OnAnimatorMove()
    {
        Rigidbody.velocity = new Vector3(animator.velocity.x, Rigidbody.velocity.y, animator.velocity.z);

        if (platyState.Judge(PlatyState.InAir))
        {
            Rigidbody.velocity += _input * _curentSpeed;
        }
        if (platyState.Judge(PlatyState.InSlice))
        {
            Rigidbody.velocity += _input * _curentSpeed * 1.5f * animator.GetFloat(_aAddSpeed);
        }
    }


    private void Shoot_Ready()
    {
        RowFinish = false;

        if (TagManager.Instance.CurrentTag != null)
            Aim.weight = 1;
        Right_Forece.weight = 0;

        DOTween.To(() => 0f, x =>
        {
            if (!RowFinish)
            {
                var sourceObject = Row_Praent.data.sourceObjects[0];
                sourceObject.weight = 1 - x;
                var sourceObject1 = Row_Praent.data.sourceObjects[1];
                sourceObject1.weight = x;
                Row_Praent.data.sourceObjects = new WeightedTransformArray() { sourceObject, sourceObject1 };
                Left_Ready.weight = x;
                Right_Ready.weight = x;
            }

        }, 1, 0.1f).OnComplete(() =>
        {
            if (!RowFinish)
            {
                Row_.weight = 1;
                DOTween.To(() => 0f, x =>
                {
                    if (!RowFinish)
                    {
                        Right_Forece.weight = x;
                    }
                }, 1, 0.1f);
            }
        });
    }

    private void Shoot_During()
    {
        ShootSlider.value += Time.deltaTime;
    }


    private async void Shoot_Finish()
    {
        RowFinish = true;

        DOTween.To(() => 1f, x =>
        {
            if (RowFinish)
            {
                Aim.weight = x;
                Right_Forece.weight = x;
                Right_Ready.weight = x;
                Row_.weight = x;
            }
        }, 0, 0.1f).OnComplete(() =>
        {
            if (RowFinish)
            {
                DOTween.To(() => 1f, x =>
                {
                    if (RowFinish)
                    {
                        Left_Ready.weight = x;
                    }
                }, 0, 0.1f);
            }
        });

        if (TagManager.Instance.CurrentTag != null)
        {
            if (Mathf.Abs(ShootSlider.value - 0.5f) < ShootSlider_faultTolerance || ShootSlider.value >= 1 - ShootSlider_faultTolerance)
            {
                correctArrowEmission.transform.position = TagManager.Instance.CurrentTag.transform.position;
                var shape = correctArrowEmission.shape;
                shape.position = correctArrowEmission.transform.InverseTransformPoint(Left_Ready.transform.position);
                correctArrowEmission.Play();


                TagManager.Instance.AimTag = TagManager.Instance.CurrentTag;
                TagManager.Instance.UseTags.Remove(TagManager.Instance.AimTag);

                if (platyState.Judge(PlatyState.InRun))
                {
                    Speedmaterial.SetFloat("_Opacity", 1);
                    Speedmaterial.SetFloat("_Amount", 0);

                    Speedmaterial.DOFloat(1, "_Amount", 0.5f).OnComplete(() =>
                    {
                        Speedmaterial.SetFloat("_Opacity", 0);
                    });

                    SpeedADD();

                }

                if (platyState.Judge(PlatyState.InAir) && ShootSlider.value < 0.8f)
                {
                    CanAirJump = true;
                    StartCoroutine(TimeScle(0.2f, 0.5f));
                }
            }
            else
            {
                MissArrowEmission.Play();
            }

            Slider t = Resources.Load<Slider>("Scree_copy");
            Slider tt = GameObject.Instantiate(t, ShootSlider.transform.parent);

            tt.value = ShootSlider.value;
            tt.transform.position = ShootSlider.transform.position;
            tt.transform.localScale = ShootSlider.transform.localScale;
            tt.transform.rotation = ShootSlider.transform.rotation;

            tt.transform.DOScale(Vector3.one * 3, 0.5f).OnComplete(() =>
            {
                Slider p = tt;
                Destroy(p.gameObject);
            });

            DOTween.To(() => 1f, x =>
            {
                tt.transform.GetComponent<CanvasGroup>().alpha = x;
            }, 0, 0.5f);

        }
        else
        {
            MissArrowEmission.Play();
        }
        ShootSlider.value = 0;
    }



    IEnumerator TimeScle(float Level, float Duration)
    {
        float OriginalTimeScle = Time.timeScale;
        Time.timeScale = Level;
        yield return new WaitForSecondsRealtime(Duration);
        Time.timeScale = OriginalTimeScle;
    }

    private void RunSliderBegin()
    {
        if (!platyState.Judge(PlatyState.InAir))
        {
            if (RunSlider.value > 0.2f)
            {

                platyState.ADD(PlatyState.InSlice);

                animator.SetBool(_aSlice, true);
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


    private void RunSliderEnd()
    {
        platyState.Sub(PlatyState.InSlice);
        animator.SetBool(_aSlice, false);
    }





    public async void SpeedADD()
    {
        animator.SetFloat(_aAddSpeed, animator.GetFloat(_aAddSpeed) + 1);
        _curentSpeed += 10;
        await Task.Delay(1500);
        _curentSpeed -= 10;
        animator.SetFloat(_aAddSpeed, animator.GetFloat(_aAddSpeed) - 1);
    }


    int _jumpDelay;
    async private void JumpControl()
    {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            _jumpDelay = SaticJumpDelay;

            animator.SetBool(_aSlice, false);
            if (platyState.Judge(PlatyState.InAir) && !CanAirJump)
            {
                return;
            }

            if (platyState.Judge(PlatyState.InAir) && CanAirJump) //空中二段跳
            {
                Rigidbody.Sleep();
                CanAirJump = false;
                animator.SetTrigger(_aDoubleJump);
                this.GetComponent<Rigidbody>().AddForce(Vector3.up * 300);
                return;
            }


            if (platyState.Judge(PlatyState.InRun)) //跑步中跳跃
            {
                _jumpDelay = RunJumpDelay;
            }

            platyState.ADD(PlatyState.InAir);

            animator.SetTrigger(_aJump);
            await Task.Delay(_jumpDelay);
            this.GetComponent<Rigidbody>().AddForce(Vector3.up * 300);
        }
    }

    Vector3 _input;
    float _MoveSpeed;
    private void MoveControl()
    {

        if (InputSystem.Instance._Input.x != 0 || InputSystem.Instance._Input.y != 0)
        {

            _MoveSpeed += InputSystem.Instance._Input.x;
            platyState.ADD(PlatyState.InRun);
        }

        if (InputSystem.Instance._Input.x <= 0.125f)
        {
            _MoveSpeed = 0;
            platyState.Sub(PlatyState.InRun);

            animator.SetBool(_aSlice, false);
        }

        _MoveSpeed = Mathf.Clamp(_MoveSpeed, 0, 4.45f);

        animator.SetFloat(_aMoveSpeed, _MoveSpeed);


        _input = new Vector3(InputSystem.Instance._Input.y, 0, InputSystem.Instance._Input.x).normalized;


        if (_input != Vector3.zero)
        {




            if (platyState.Judge(PlatyState.InSlice))
            {
                _input = transform.forward;
            }
            else
            {
                var relative = (this.transform.position + _input) - this.transform.position;
                var rot = Quaternion.LookRotation(relative, Vector3.up);
                transform.rotation = Quaternion.RotateTowards(this.transform.rotation, rot, RotationSpeed * Time.deltaTime);
            }
        }
    }


    public LayerMask LayerMask;
    public float DisOffest = 0.15f;
    public void OnGroundRay()
    {

        if (platyState.Judge(PlatyState.InAir) && Rigidbody.velocity.y < -1)
        {
            Debug.DrawLine(this.transform.position + Vector3.up * 0.1f, this.transform.position + Vector3.down * (DisOffest + 0.1f));
            if (Physics.Raycast(this.transform.position + Vector3.up * 0.1f, Vector3.down, DisOffest + 0.1f, LayerMask))
            {
                platyState.Sub(PlatyState.InAir);
                animator.SetTrigger(_aOnGround);
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

    InSlice = 8

}

public static class PlatyStateHelper
{

    public static void ADD(ref this PlatyState state, PlatyState _AddState)
    {
        state |= _AddState;
    }

    public static void Sub(ref this PlatyState state, PlatyState _SubState)
    {
        state &= ~_SubState;
    }

    public static bool Judge(ref this PlatyState state, PlatyState _JudgeState)
    {
        if ((state & _JudgeState) == _JudgeState)
            return true;
        else
            return false;
    }
}