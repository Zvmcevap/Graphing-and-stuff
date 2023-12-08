using UnityEngine;
using static FunctionLibrary;

public class GPUGraph: MonoBehaviour
{
    const int max_resolution = 3000;

    [SerializeField, Range(10, max_resolution)]
    int resolution = 100;


    [SerializeField]
    FunctionName function;


    [SerializeField, Min(0f)]
    float functionDuration = 1f, transitionDuration = 0f;

    [SerializeField]
    bool flipFunctions = false;

    [SerializeField, Range(0.1f, 10f)]
    float amplitude, freq = 1.0f;

    [SerializeField, Range(-10f, 10f)]
    float timeDilation = 1.0f;

    [SerializeField, Range(0f, 1f)]
    float z_range = 1f;

    [SerializeField]
    Material material;

    [SerializeField]
    Mesh mesh;


    float duration;
    bool transitioning;
    FunctionName currentFunction;

    // Compute Stuff
    ComputeBuffer positionsBuffer;

    [SerializeField]
    ComputeShader computeShader;
    static readonly int
        positionsId = Shader.PropertyToID("_Positions"),
        resolutionId = Shader.PropertyToID("_Resolution"),
        stepId = Shader.PropertyToID("_Step"),
        timeId = Shader.PropertyToID("_Time"),
        ampId = Shader.PropertyToID("_Amp"),
        freqId = Shader.PropertyToID("_Freq"),
        funcAId = Shader.PropertyToID("_FunctionKeyA"),
        funcBId = Shader.PropertyToID("_FunctionKeyB"),
        transId = Shader.PropertyToID("_TransitionProgress");


    void UpdateFunctionOnGPU()
    {
        // Pass Data to the compute shader
        float step = 2f / resolution;
        computeShader.SetInt(resolutionId, resolution);
        computeShader.SetFloat(stepId, step);
        computeShader.SetFloat(timeId, Time.time / timeDilation);
        computeShader.SetFloat(ampId, amplitude);
        computeShader.SetFloat(freqId, freq);
        computeShader.SetInt(funcAId, (int)currentFunction);
        computeShader.SetInt(funcBId, (int)function);

        Debug.Log((int)currentFunction);

        // Transitioning
        if (transitioning)
        {
            computeShader.SetFloat(
                transId,
                Mathf.SmoothStep(0f, 1f, duration / transitionDuration)
                );
        }

        // Set Buffer
        computeShader.SetBuffer(0, positionsId, positionsBuffer);
        int groups = Mathf.CeilToInt(resolution / 8f);

        // Run the beast
        computeShader.Dispatch(0, groups, groups, 1);

        // Draw my friend
        material.SetBuffer(positionsId, positionsBuffer);
        material.SetFloat(stepId, step);

        var bounds = new Bounds(Vector3.zero, Vector3.one * (2f + 2f / resolution) * amplitude);

        Graphics.DrawMeshInstancedProcedural(
            mesh,                   //mesh to draw
            0,                      //submesh to draw
            material,               //material
            bounds,                 //bounds of the draw space
            resolution * resolution   //how many meshes to draw
            );
    }

    // Start is called before the first frame update
    void OnEnable()
    {
        positionsBuffer = new ComputeBuffer(max_resolution * max_resolution, 3 * 4);
    }

    private void OnDisable()
    {
        positionsBuffer.Release();
        positionsBuffer = null;
    }

    // Update is called once per frame

    private void Update()
    {
        duration += Time.deltaTime;
        if (transitioning)
        {
            if (duration >= transitionDuration)
            {
                duration -= transitionDuration;
                transitioning = false;
                currentFunction = function;
            }
        }
        else if (duration > functionDuration && flipFunctions)
        {
            duration -= functionDuration;
            transitioning = true;
            function = GetNextFunctionName(function);

        }
        else if (currentFunction != function)
        {
            duration = 0;
            transitioning = true;
        }

        UpdateFunctionOnGPU();

    }


}
