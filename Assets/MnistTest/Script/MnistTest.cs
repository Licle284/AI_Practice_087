using UnityEngine;


using Unity.Barracuda;
using UnityEngine.UI;


sealed class MnistTest : MonoBehaviour
{
    public NNModel _model;
    public ComputeShader _preprocess;
    public ComputeShader _postprocess;

    public RawImage _labelImage;

    ComputeBuffer _scores;

    public PaintController paintController;

    void Start()
    {

    }

    public void ExecuteInference()
    {
        Texture2D sourceImage = paintController.GetPaintedTexture();

        // Invoke the preprocessing compute kernel.
        using var buffer = new ComputeBuffer(28 * 28, sizeof(float));
        _preprocess.SetTexture(0, "Input", sourceImage);
        _preprocess.SetBuffer(0, "Output", buffer);
        _preprocess.Dispatch(0, 28 / 4, 28 / 4, 1);

        // Run the MNIST model.
        using var worker = ModelLoader.Load(_model).CreateWorker();
        using (var input = new Tensor(1, 28, 28, 1, buffer))
            worker.Execute(input);

        // Retrieve the results into a temporary render texture.
        var rt = RenderTexture.GetTemporary(10, 1, 0, RenderTextureFormat.RFloat);
        using (var tensor = worker.PeekOutput().Reshape(new TensorShape(1, 1, 10, 1)))
            tensor.ToRenderTexture(rt);

        // Invoke the postprocessing compute kernel.
        _scores = new ComputeBuffer(10, sizeof(float));
        _postprocess.SetTexture(0, "Input", rt);
        _postprocess.SetBuffer(0, "Output", _scores);
        _postprocess.Dispatch(0, 1, 1, 1);

        RenderTexture.ReleaseTemporary(rt);

        // Output display
        _labelImage.material.SetBuffer("_Scores", _scores);
    }

    public void ResetInference()
    {
        _scores.Dispose();
        paintController.ResetTexture();

    }
}
