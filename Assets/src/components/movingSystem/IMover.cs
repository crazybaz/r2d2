using UnityEngine;

public interface IMover
{
    void MoveToPosition(float x, float y, float z);
    void MoveToPosition(Vector3 position);
}