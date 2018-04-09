using ShaderUtils;
using UnityEngine;

public class GpuHistogram : IHistogram
{
    private const string KERNEL_NAME_HISTO      = "CSHistogram";
    
    private const string PARAM_GRID             = "g_gridDimen";
    private const string PARAM_DATA             = "g_data";
    private const string PARAM_HISTO            = "g_histo";
    private const string PARAM_SIZE             = "g_size";
    private const string PARAM_BIN_SIZE         = "g_binSize";

    private readonly ComputeShader shader;
    private readonly int kernelIdHisto;
    private readonly int kernelIdReduce;
    private readonly int totalThreadsInBlock;

    public GpuHistogram(ComputeShader s)
    {
        shader = s;
        kernelIdHisto = shader.FindKernel(KERNEL_NAME_HISTO);
        totalThreadsInBlock = shader.TotalThreadsInBlock(kernelIdHisto);
    }

    public int[] Process(int[] data, int bins)
    {
        if (bins > totalThreadsInBlock) throw new System.ArgumentException("histo base shouldn't be larger than: "+totalThreadsInBlock);
        
        var binsBlocks = Utils.TrimToBlock(data.Length, bins);
        var dataBuffer = new ComputeBuffer(data.Length, sizeof(int));
        var histoBuffer = new ComputeBuffer(bins, sizeof(int));
        dataBuffer.SetData(data);

        shader.SetInts(PARAM_GRID, binsBlocks, 1, 1);
        shader.SetInt(PARAM_SIZE, data.Length);
        shader.SetInt(PARAM_BIN_SIZE, bins);
        
        shader.SetBuffer(kernelIdHisto, PARAM_DATA, dataBuffer);
        shader.SetBuffer(kernelIdHisto, PARAM_HISTO, histoBuffer);
        shader.Dispatch(kernelIdHisto, binsBlocks, 1, 1);

        var result = new int[bins];
        histoBuffer.GetData(result);
        histoBuffer.Release();
        dataBuffer.Release();
        return result;
    }
}
