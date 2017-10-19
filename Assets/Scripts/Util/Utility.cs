using UnityEngine;

public class Util
{
    
    public static float RandAngle()
    {
        return Random.Range(0f, Mathf.PI * 2f);
    }
    
    public static Vector2 RandPos(Vector2 center, float radius)
    {
        float angle = RandAngle();
        float r = Random.Range(0f, 1f);
        r = Mathf.Sqrt(r);
        r *= radius;
        return Calc.ApplyRotationAngle(angle, Vector2.up) * r + center;
    }
}