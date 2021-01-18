using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DirectionalVector : MonoBehaviour
{
    public TMP_InputField[] pos1_inputs;
    public TMP_InputField[] pos2_inputs;
    public TMP_InputField radius_input;

    public TextMeshProUGUI directionalUnitVect;
    public TextMeshProUGUI closerThanRadius;

    private MyVector3 pos1 = MyVector3.Zero();
    private MyVector3 pos2 = MyVector3.Zero();
    private float radius;

    public void Calculate()
    {
        try
        {//parse strings into floating point numbers
            float pos1_x = Single.Parse(pos1_inputs[0].text);
            float pos1_y = Single.Parse(pos1_inputs[1].text);
            float pos1_z = Single.Parse(pos1_inputs[2].text);
            float pos2_x = Single.Parse(pos2_inputs[0].text);
            float pos2_y = Single.Parse(pos2_inputs[1].text);
            float pos2_z = Single.Parse(pos2_inputs[2].text);
            float num = Single.Parse(radius_input.text);
            if (num < 0)
            {
                throw new Exception("Only positive radii allowed!");
            }
            pos1 = new MyVector3(pos1_x, pos1_y, pos1_z);
            pos2 = new MyVector3(pos2_x, pos2_y, pos2_z);
            radius = num;
        }
        catch (Exception e)
        {//parsing failed
            Debug.LogError(e.Message);
            pos1_inputs[0].text = "";
            pos1_inputs[1].text = "";
            pos1_inputs[2].text = "";
            pos2_inputs[0].text = "";
            pos2_inputs[1].text = "";
            pos2_inputs[2].text = "";
            radius_input.text = "";
            return;
        }
        //printe vectors to the scene
        directionalUnitVect.text = MyVector3.DirectionalUnitVector(pos1, pos2).Print();
        closerThanRadius.text = MyVector3.CloserThanRadius(pos1, pos2, radius).ToString();
    }

    //draw rays in the scene view
    private void Update()
    {
        Debug.DrawRay(Vector3.zero, new Vector3(pos1.getX(), pos1.getY(), pos1.getZ()), Color.black);
        Debug.DrawRay(Vector3.zero, new Vector3(pos2.getX(), pos2.getY(), pos2.getZ()), Color.black);
    }
}
