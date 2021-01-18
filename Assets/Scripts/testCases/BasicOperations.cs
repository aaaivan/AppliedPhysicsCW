using System;
using UnityEngine;
using TMPro;

public class BasicOperations : MonoBehaviour
{
    public TMP_InputField[] v1_inputs;
    public TMP_InputField[] v2_inputs;
    public TMP_InputField scalar_input;

    public TextMeshProUGUI add;
    public TextMeshProUGUI subtract;
    public TextMeshProUGUI scale;
    public TextMeshProUGUI dot;
    public TextMeshProUGUI magnitude;
    public TextMeshProUGUI unitVect;
    public TextMeshProUGUI reflectX;
    public TextMeshProUGUI reflectY;
    public TextMeshProUGUI reflectZ;
    public TextMeshProUGUI zero;

    private MyVector3 v1;
    private MyVector3 v2;
    private float scalar;

    public void Calculate()
    {
        try
        {//parse strings into floating point numbers
            float v1_x = Single.Parse(v1_inputs[0].text);
            float v1_y = Single.Parse(v1_inputs[1].text);
            float v1_z = Single.Parse(v1_inputs[2].text);
            float v2_x = Single.Parse(v2_inputs[0].text);
            float v2_y = Single.Parse(v2_inputs[1].text);
            float v2_z = Single.Parse(v2_inputs[2].text);
            float num = Single.Parse(scalar_input.text);
            v1 = new MyVector3(v1_x, v1_y, v1_z);
            v2 = new MyVector3(v2_x, v2_y, v2_z);
            scalar = num;
        }
        catch (Exception e)
        {//parsing failed
            Debug.LogError(e.Message);
            v1_inputs[0].text = "";
            v1_inputs[1].text = "";
            v1_inputs[2].text = "";
            v2_inputs[0].text = "";
            v2_inputs[1].text = "";
            v2_inputs[2].text = "";
            scalar_input.text = "";
            return;
        }
        //print vectors in the scene
        add.text = MyVector3.Add(v1, v2).Print();
        subtract.text= MyVector3.Subtract(v1, v2).Print();
        scale.text = v1.Scale(scalar).Print();
        dot.text = MyVector3.Dot(v1, v2).ToString("0.00");
        magnitude.text = v1.Magnitude().ToString("0.00");
        unitVect.text = v1.UnitVector().Print();
        reflectX.text = v1.ReflectX().Print();
        reflectY.text = v1.ReflectY().Print();
        reflectZ.text = v1.ReflectZ().Print();
        zero.text = MyVector3.Zero().Print();
    }
}
