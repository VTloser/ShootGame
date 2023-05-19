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
    }


    void Update()
    {
        //Debug.Log(Quaternion.Angle(A.rotation, B.rotation));

        //C.rotation = Quaternion.AngleAxis(Value_1, Dir);

        Debug.Log(Quaternion.Dot(A.rotation, B.rotation));

        //C.rotation = Quaternion.FromToRotation(A.position, B.position);

        if (Input.GetKey(KeyCode.A))
        {
            A.rotation = Quaternion.FromToRotation(Vector3.forward, B.position - A.position);
        }
        if (Input.GetKey(KeyCode.B))
        {
            A.rotation = Quaternion.LookRotation(B.position - A.position ,Vector3.forward );
        }
        

        // A.rotation = Quaternion.Inverse(B.rotation);

        // A.rotation = Quaternion.Lerp(A.rotation, B.rotation, Time.deltaTime);

        //A.rotation = Quaternion.LookRotation(A.position - B.position, Vector3.up);


        //A.rotation = Quaternion.RotateTowards(A.rotation, B.rotation, Time.deltaTime*10);



        //Quaternion a = new Quaternion();
        //a.SetFromToRotation(Vector3.forward, B.position -A.position);
        //A.position = Vector3.Lerp(A.position, B.position, Time.deltaTime * 1);
        //A.rotation = a;


    }
}
