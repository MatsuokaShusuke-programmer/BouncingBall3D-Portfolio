using Unity.VisualScripting;
using UnityEngine;

public class GroundMove : MonoBehaviour
{
    public float speed = 2.0f;
    public float distance = 12f;

    private Vector3 startPos;
    public Rigidbody rb;

    public enum axis
    {
        X, Y, Z
    }
    public axis  Axis = axis.X;
    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float ping = Mathf.PingPong(Time.time * speed, distance);
        Vector3 newPos = startPos;

        switch (Axis)
        {
            case axis.X:
                newPos.x = startPos.x + ping;
                break;
            case axis.Y:
                newPos.y = startPos.y + ping;
                break;
            case axis.Z:
                newPos.z = startPos.z + ping;
                break;
        }
        rb.MovePosition(newPos);
    }

    private void OnDestroy()
    {
        Debug.Log("z", this);
    }
}
