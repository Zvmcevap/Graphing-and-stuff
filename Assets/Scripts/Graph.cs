using UnityEngine;
using static FunctionLibrary;

public class Graph: MonoBehaviour
{
    [SerializeField]
    Transform pointPrefab;

    [SerializeField, Range(10, 200)]
    int resolution = 200;

    [SerializeField]
    FunctionName function;


    [SerializeField, Min(0f)]
    float functionDuration = 2f, transitionDuration = 2f;

    [SerializeField]
    bool flipFunctions = false;

    [SerializeField, Range(0.1f, 10f)]
    float amplitude, freq = 1.0f;

    [SerializeField, Range(-10f, 10f)]
    float timeDilation = 1.0f;

    [SerializeField, Range(0f, 1f)]
    float z_range = 1f;


    Transform[] points;
    float duration;
    bool transitioning;
    FunctionName transitionFunction;
    FunctionName currentFunction;

    // Start is called before the first frame update
    void Awake()
    {
        points = new Transform[resolution * resolution];
        currentFunction = function;

        float step = 2f / resolution;
        var scale = Vector3.one * step;
        for (int i = 0; i < points.Length; i++)
        {

            Transform point = points[i] = Instantiate(pointPrefab);
            point.localScale = scale;

            point.SetParent(transform, false);
        }
    }

    // Update is called once per frame

    private void Update()
    {
        Debug.Log(transitionFunction);
        Debug.Log(function);
        duration += Time.deltaTime;
        if (transitioning)
        {
            if (duration >= transitionDuration)
            {
                duration -= transitionDuration;
                transitioning = false;
                currentFunction = function;
            }
            else
            {
                UpdateFunctionTransition();
            }
        }
        else if (duration > functionDuration && flipFunctions)
        {
            duration -= functionDuration;
            transitioning = true;
            transitionFunction = function;
            function = GetNextFunctionName(function);

        }
        else if (currentFunction != function)
        {
            duration = 0;
            transitioning = true;
            transitionFunction = currentFunction;
        }
        else
        {
            UpdateFunction();
        }

    }

    void UpdateFunction()
    {
        Function fun = GetFunction(function);
        float time = Time.time / timeDilation;
        float step = 2f / resolution;

        for (int i = 0, x = 0, z = 0; i < points.Length; i++, x++)
        {
            Transform point = points[i];
            Vector3 position = point.position;
            if (x == resolution)
            {
                x = 0;
                z++;
                position.z = (z + 0.5f) * step - 1f;
                position.z *= z_range;
            }


            float u = (x + 0.5f) * step - 1f;
            float v = (z + 0.5f) * step - 1f;
            position = fun(u, v, time, amplitude, freq);
            position.z *= z_range;

            point.localPosition = position;
        }

    }

    void UpdateFunctionTransition()
    {
        Function from = GetFunction(transitionFunction), to = GetFunction(function);
        float progress = duration / transitionDuration;

        float time = Time.time / timeDilation;
        float step = 2f / resolution;

        for (int i = 0, x = 0, z = 0; i < points.Length; i++, x++)
        {
            Transform point = points[i];
            Vector3 position = point.position;
            if (x == resolution)
            {
                x = 0;
                z++;
                position.z = (z + 0.5f) * step - 1f;
                position.z *= z_range;
            }


            float u = (x + 0.5f) * step - 1f;
            float v = (z + 0.5f) * step - 1f;

            point.localPosition = Morph(u, v, time, amplitude, freq, from, to, progress);
        }

    }

}
