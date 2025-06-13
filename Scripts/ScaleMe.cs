using UnityEngine;
public class ScaleMe : MonoBehaviour
{
    public float angularVelocity = 10;
    Vector2 minMaxScale = new Vector2(.5f, 2);
    float angle;
    void Update()
    {
        angle += angularVelocity * Time.deltaTime;
        angle %= 360;
        var scale = minMaxScale.x + Mathf.Abs(minMaxScale.y * Mathf.Sin(Mathf.Deg2Rad * angle));
        transform.localScale = Vector3.one * scale;
    }
}