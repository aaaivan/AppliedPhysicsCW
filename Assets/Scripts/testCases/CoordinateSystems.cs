using System;
using UnityEngine;
using TMPro;

public class CoordinateSystems : MonoBehaviour
{
    public TMP_InputField[] cartesian_inputs;
    public TMP_InputField[] cylindrical_inputs;
    public TMP_InputField[] spherical_inputs;

    MyVector3 vectCartesian= MyVector3.Zero();
    MyVector3_Cylindrical vectCylindrical= MyVector3_Cylindrical.Zero();
    MyVector3_Spherical vectSpherical = MyVector3_Spherical.Zero();

    public void ConvertFromCartesian()
    {
        try
        {//parse strings into floating point numbers
            float x = Single.Parse(cartesian_inputs[0].text);
            float y = Single.Parse(cartesian_inputs[1].text);
            float z = Single.Parse(cartesian_inputs[2].text);
            vectCartesian = new MyVector3(x, y, z);
        }
        catch (Exception e)
        {//parsing failed
            Debug.LogError(e.Message);
            cartesian_inputs[0].text = "";
            cartesian_inputs[1].text = "";
            cartesian_inputs[2].text = "";
            return;
        }
        //print vectors to the scene
        vectCylindrical = vectCartesian.ToCylindrical();
        vectSpherical = vectCartesian.ToSpherical();
        PrintVectors();
    }

    public void ConvertFromCylindrical()
    {
        try
        {//parse strings into floating point numbers
            float r = Single.Parse(cylindrical_inputs[0].text);
            if (r < 0)
            {
                throw new Exception("Only positive radii allowed!");
            }
            float azim = Single.Parse(cylindrical_inputs[1].text) * (float)Math.PI / 180;
            float y = Single.Parse(cylindrical_inputs[2].text);
            vectCylindrical = new MyVector3_Cylindrical(r, azim, y);
        }
        catch (Exception e)
        {//parsing failed
            Debug.LogError(e.Message);
            cylindrical_inputs[0].text = "";
            cylindrical_inputs[1].text = "";
            cylindrical_inputs[2].text = "";
            return;
        }
        //print vectors to the scene
        vectCartesian = vectCylindrical.ToCartesian();
        vectSpherical = vectCylindrical.ToSpherical();
        PrintVectors();
    }

    public void ConvertFromSherical()
    {
        try
        {//parse strings into floating point numbers
            float r = Single.Parse(spherical_inputs[0].text);
            if (r < 0)
            {
                throw new Exception("Only positive radii allowed!");
            }
            float azim = Single.Parse(spherical_inputs[1].text) * (float)Math.PI / 180;
            float polar = Single.Parse(spherical_inputs[2].text) * (float)Math.PI / 180;
            vectSpherical = new MyVector3_Spherical(r, azim, polar);
        }
        catch (Exception e)
        {//parsing failed
            Debug.LogError(e.Message);
            spherical_inputs[0].text = "";
            spherical_inputs[1].text = "";
            spherical_inputs[2].text = "";
            return;
        }
        //print vectors to the scene
        vectCartesian = vectSpherical.ToCartesian();
        vectCylindrical = vectSpherical.ToCylindrical();
        PrintVectors();
    }

    private void PrintVectors()
    {//print vectors to the scene
        cartesian_inputs[0].text = vectCartesian.getX().ToString("0.000");
        cartesian_inputs[1].text = vectCartesian.getY().ToString("0.000");
        cartesian_inputs[2].text = vectCartesian.getZ().ToString("0.000");

        cylindrical_inputs[0].text = vectCylindrical.getRadius().ToString("0.000");
        cylindrical_inputs[1].text = vectCylindrical.getAzmuthalDeg().ToString("0.000");
        cylindrical_inputs[2].text = vectCylindrical.getY().ToString("0.000");

        spherical_inputs[0].text = vectSpherical.getRadius().ToString("0.000");
        spherical_inputs[1].text = vectSpherical.getAzmuthalDeg().ToString("0.000");
        spherical_inputs[2].text = vectSpherical.getPolarDeg().ToString("0.000");
    }

    //draw rays in the scene view
    private void Update()
    {
        Debug.DrawRay(Vector3.zero, new Vector3(vectCartesian.getX(), vectCartesian.getY(), vectCartesian.getZ()), Color.black);
    }
}
