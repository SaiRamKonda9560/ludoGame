using UnityEngine;

public class path : MonoBehaviour
{
    public bool isSafe;
    public Vector3 position
    {
        get { return transform.position; }
        set { transform.position = value; }
    }
}
