using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class VNDetectedPoint
{
    public string pointName;
    public double x;
    public double y;
    public double confidence;


    public VNDetectedPoint(string _pointName, string _x, string _y, string _confidence)
    {
        this.pointName = _pointName;
        this.x = double.Parse(_x);
        this.y = double.Parse(_y);
        this.confidence = double.Parse(_confidence);
    }
}