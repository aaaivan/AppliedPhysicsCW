using System;
using UnityEngine;
using TMPro;

public class PointOnLine : MonoBehaviour
{
    public TMP_InputField[] vect_inputs;
    public TMP_InputField[] point_inputs;
    public TMP_InputField[] direction_inputs;
    public TMP_InputField scalar_input;

    public TextMeshProUGUI distanceFromLine;
    public TextMeshProUGUI isOnLine;

    private MyVector3 vector= MyVector3.Zero();
    private Line line= new Line(MyVector3.Zero(), MyVector3.Zero());
    private float tolerance;

    public void Calculate()
    {
        try
        {//parse strings into floating point numbers
            float v_x = Single.Parse(vect_inputs[0].text);
            float v_y = Single.Parse(vect_inputs[1].text);
            float v_z = Single.Parse(vect_inputs[2].text);
            float p_x = Single.Parse(point_inputs[0].text);
            float p_y = Single.Parse(point_inputs[1].text);
            float p_z = Single.Parse(point_inputs[2].text);
            float d_x = Single.Parse(direction_inputs[0].text);
            float d_y = Single.Parse(direction_inputs[1].text);
            float d_z = Single.Parse(direction_inputs[2].text);
            if (d_x == 0 && d_y == 0 && d_z == 0)
            {
                throw new Exception("The direction cannot be be zero!");
            }
            float num = Single.Parse(scalar_input.text);
            if (num < 0)
            {
                throw new Exception("Only positive tolerance allowed!");
            }
            vector = new MyVector3(v_x, v_y, v_z);
            line = new Line(new MyVector3(d_x, d_y, d_z), new MyVector3(p_x, p_y, p_z));
            tolerance = num;
        }
        catch (Exception e)
        {//parsing failed
            Debug.LogError(e.Message);
            vect_inputs[0].text = "";
            vect_inputs[1].text = "";
            vect_inputs[2].text = "";
            point_inputs[0].text = "";
            point_inputs[1].text = "";
            point_inputs[2].text = "";
            direction_inputs[0].text = "";
            direction_inputs[1].text = "";
            direction_inputs[2].text = "";
            scalar_input.text = "";
            return;
        }
        //printe vectors to the scene
        distanceFromLine.text = vector.DistanceFromLine(line).ToString("0.000");
        isOnLine.text = vector.IsOnLine(line, tolerance).ToString();
    }

    //draw rays in scene view
    private void Update()
    {
        MyVector3 p = line.getPointOnLine();
        MyVector3 dir = line.getDirection();
        Debug.DrawRay(new Vector3(p.getX(), p.getY(), p.getZ()), 1000 * new Vector3(dir.getX(), dir.getY(), dir.getZ()), Color.magenta);
        Debug.DrawRay(new Vector3(p.getX(), p.getY(), p.getZ()), -1000 * new Vector3(dir.getX(), dir.getY(), dir.getZ()), Color.magenta);
        Debug.DrawRay(Vector3.zero, new Vector3(vector.getX(), vector.getY(), vector.getZ()), Color.black);
    }
}
