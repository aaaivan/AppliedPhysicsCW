using System;
using UnityEngine;
using TMPro;

public class PointOnPlane : MonoBehaviour
{
    public TMP_InputField[] vect_inputs;
    public TMP_InputField[] plane_inputs;
    public TMP_InputField scalar_input;

    public TextMeshProUGUI distanceFromPlane;
    public TextMeshProUGUI isOnPlane;

    private MyVector3 vector= MyVector3.Zero();
    private Plane plane = Plane.XZ();
    private float tolerance;

    public void Calculate()
    {
        try
        {//parse strings into floating point numbers
            float v_x = Single.Parse(vect_inputs[0].text);
            float v_y = Single.Parse(vect_inputs[1].text);
            float v_z = Single.Parse(vect_inputs[2].text);
            float plane_a = Single.Parse(plane_inputs[0].text);
            float plane_b = Single.Parse(plane_inputs[1].text);
            float plane_c = Single.Parse(plane_inputs[2].text);
            float plane_d = Single.Parse(plane_inputs[3].text);
            if (plane_a==0 && plane_b == 0 && plane_c == 0)
            {
                throw new Exception("Coefficients cannot all be zero!");
            }
            float num = Single.Parse(scalar_input.text);
            if (num < 0)
            {
                throw new Exception("Only positive tolerance allowed!");
            }
            vector = new MyVector3(v_x, v_y, v_z);
            plane = new Plane(plane_a, plane_b, plane_c, plane_d);
            tolerance = num;
        }
        catch (Exception e)
        {//parsing failed
            Debug.LogError(e.Message);
            vect_inputs[0].text = "";
            vect_inputs[1].text = "";
            vect_inputs[2].text = "";
            plane_inputs[0].text = "";
            plane_inputs[1].text = "";
            plane_inputs[2].text = "";
            plane_inputs[3].text = "";
            scalar_input.text = "";
            return;
        }
        //printe vectors to the scene
        distanceFromPlane.text = vector.DistanceFromPlane(plane).ToString("0.000");
        isOnPlane.text = vector.IsOnPlane(plane, tolerance).ToString();
    }

    //draw rays in the scene
    private void Update()
    {
        int lines = 36;
        MyVector3 normal = plane.Normal();
        MyVector3 p = MyVector3.Subtract( vector, normal.Scale(vector.DistanceFromPlane(plane)) );
        for (int i=0; i<lines; i++)
        {
            Vector3 v = new Vector3((float)Math.Cos(Math.PI * 2 * i / lines), 0, (float)Math.Sin(Math.PI * 2 * i / lines));
            v= Quaternion.FromToRotation(Vector3.up, new Vector3(normal.getX(), normal.getY(), normal.getZ()))*v;
            Debug.DrawRay(new Vector3(p.getX(), p.getY(), p.getZ()), 1000 * v, Color.magenta);
        }
        Debug.DrawRay(Vector3.zero, new Vector3(vector.getX(), vector.getY(), vector.getZ()), Color.black);
    }
}
