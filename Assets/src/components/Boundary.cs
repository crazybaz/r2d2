using UnityEngine;

[System.Serializable]
public class Boundary
{
    public float xMin, xMax, zMin, zMax;

    public bool IsOutside(Vector3 point)
    {
        return point.x < xMin || point.x > xMax || point.z < zMin || point.z > zMax;
    }

    public string Stringify()
    {
        return "xMin=" + xMin + ", xMax=" + xMax + ", zMin=" + zMin + ", zMax=" + zMax;
    }

    public float width
    {
        get { return xMax - xMin; }
    }

    public float height
    {
        get { return zMax - zMin; }
    }
}