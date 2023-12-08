using UnityEngine;
using static UnityEngine.Mathf;

public static class FunctionLibrary
{

    public delegate Vector3 Function(float x, float z, float t, float a, float f);

    public enum FunctionName { sinWave, sinSquareWave, multiSinWave, multiSquareWave, Ripple, rippleSquare, Sphere, SphereWave, Thorus };
    static Function[] functions = { sinWave, sinSquareWave, multiSinWave, multiSquareWave, Ripple, rippleSquare, Sphere, SphereWave, Thorus };


    // 2D
    public static Function GetFunction(FunctionName name)
    {
        return functions[(int)name];
    }

    public static FunctionName GetNextFunctionName(FunctionName name)
    {
        return (int)name < functions.Length - 1 ? name + 1 : 0;
    }


    public static Vector3 sinWave(float x, float z, float t, float a, float f)
    {
        Vector3 p;
        p.y = Sin(PI * (x + t) * f) * a;
        p.x = x; p.z = z;
        return p;
    }

    public static Vector3 sinSquareWave(float x, float z, float t, float a, float f)
    {
        Vector3 p;
        p.y = Sin(PI * (x + z + t) * f) * a;
        p.x = x; p.z = z;

        return p;

    }

    public static Vector3 multiSinWave(float x, float z, float t, float a, float f)
    {
        Vector3 p;

        p = sinWave(x, z, t * 0.5f, a, f);
        f *= 2f;
        a *= .5f;
        p += sinWave(x, z, t, a, f);

        p.y *= (2f / 3f);
        p.x = x; p.z = z;

        return p;
    }

    public static Vector3 multiSquareWave(float u, float z, float t, float a, float f)
    {
        Vector3 p;
        p.y = sinWave(u, z, t * 0.5f, a, f).y;
        p.y += sinWave(z, z, t, a * 0.5f, f * 2).y;
        p.y *= sinWave(u + z, z, t * 0.25f, a, f).y;

        p.y *= (1f / 2.5f);

        p.x = u; p.z = z;

        return p;
    }

    public static Vector3 Ripple(float u, float v, float t, float a, float f)
    {
        float d = Abs(u);
        Vector3 p;
        p.x = u;
        p.y = Sin(PI * (f * d - t)) * a;
        p.y /= 1f + 10f * d;
        p.z = v;
        return p;
    }
    public static Vector3 rippleSquare(float x, float z, float t, float a, float f)
    {
        Vector3 p;
        float d = Sqrt(x * x + z * z);

        p.y = sinWave(d, z, t, a, f).y;
        p.y /= (1f + 10f * d);

        p.x = x;
        p.z = z;
        return p;
    }

    // 3D


    public static Vector3 Sphere(float u, float v, float t, float a, float f)
    {
        Vector3 p;
        float r = 0.9f + a / 100f * Sin(PI * (6f * u + 4f * v + t));

        float s = r * Cos(0.5f * PI * v);

        p.x = s * Sin(f * PI * u);
        p.y = r * Sin(f * PI * 0.5f * v);
        p.z = s * Cos(f * PI * u);


        return p;
    }

    public static Vector3 SphereWave(float u, float v, float t, float a, float f)
    {
        Vector3 p;
        float r = 0.9f + a / 100f * Sin(PI * (6f * u + 4f * v + t));

        float s = r * Cos(0.5f * PI * v);



        p.x = s * Sin(f * PI * u);
        p.y = r * Sin(f * PI * 0.5f * v);
        p.z = s * Cos(f * PI * u);

        float x = sinWave(p.x, v, t, 0.5f, 1f).y;
        float z = sinWave(p.z, v, t, 0.25f, 1f).y;

        p.x += x;
        p.z += x;


        return p;
    }

    public static Vector3 Thorus(float u, float v, float t, float a, float f)
    {
        float r1 = 0.7f + (a - 0.1f) * 0.1f * Sin(PI * (6f * u + 0.5f * t));
        float r2 = 0.15f + (f - 0.1f) * 0.05f * Sin(PI * (8f * u + 4f * v + 2f * t));

        float s = r1 + r2 * Cos(0.5f * PI * v);
        Vector3 p;
        p.x = s * Sin(PI * u);
        p.y = r2 * Sin(PI * v);
        p.z = s * Cos(PI * u);

        return p;
    }

    public static Vector3 Morph(
        float u, float v, float t, float a, float f, Function from, Function to, float progress
        )
    {
        return Vector3.LerpUnclamped(from(u, v, t, a, f), to(u, v, t, a, f), SmoothStep(0f, 1f, progress));
    }

}
