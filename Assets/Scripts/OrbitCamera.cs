using UnityEngine;

public class OrbitCamera : MonoBehaviour
{
    public Vector3 target = Vector3.zero;
    public float radius = 8f;
    public float height = 5f;
    public float degreesPerSecond = 6f;
    public float startAngle = 0f;

    private float angle;

    void Start()
    {
        angle = startAngle;
    }

    void Update()
    {
        angle += degreesPerSecond * Time.deltaTime;
        float rad = angle * Mathf.Deg2Rad;
        Vector3 offset = new Vector3(Mathf.Cos(rad) * radius, height, Mathf.Sin(rad) * radius);
        transform.position = target + offset;
        transform.LookAt(target + Vector3.up * 1.2f);
    }
}
