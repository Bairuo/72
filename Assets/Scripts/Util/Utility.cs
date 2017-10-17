using UnityEngine;

public class Util
{
    public static Vector2 RandPos(Vector2 center, float radius)
    {
        float angle = Random.Range(0f, Mathf.PI * 2f);
        float r = Random.Range(0f, radius);
        r = Mathf.Sqrt(r);
        return Calc.ApplyRotationAngle(angle, Vector2.up) * r + center;
    }
}