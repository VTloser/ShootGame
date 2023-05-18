using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rot : MonoBehaviour
{

    public Transform A;
    public Transform B;


    public Transform C;


    [Range(0, 360)]
    public float Value_1;

    public Vector3 Dir;


    private void Awake()
    {
        A.rotation = Quaternion.FromToRotation(Vector3.zero, B.position.normalized);
    }


    void Update()
    {
        //Debug.Log(Quaternion.Angle(A.rotation, B.rotation));


        //C.rotation = Quaternion.AngleAxis(Value_1, Dir);

        Debug.Log(Quaternion.Dot(A.rotation, B.rotation));



        //C.rotation = Quaternion.FromToRotation(A.position, B.position);
    }
}
