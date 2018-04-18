using System;
using ShaderUtils;
using UnityEngine;

public class GpuHistogramWithReduce : IHistogram
{
    private const string KERNEL_NAME_HISTO      = "CSHistogram";
    private const string KERNEL_NAME_REDUCE     = "CSReduce";
    
    private const string PARAM_GRID             = "g_gridDimen";
    private const string PARAM_BUFF_IN          = "g_in";
    private const string PARAM_BUFF_OUT         = "g_out";
    private const string PARAM_SIZE             = "g_size";
    private const string PARAM_BIN_SIZE         = "g_binSize";
    private const string PARAM_OFFSET           = "g_offset";

    private readonly ComputeShader shader;
    private readonly int kernelIdHisto;
    private readonly int kernelIdReduce;
    private readonly int totalThreadsInBlock;
    
    public GpuHistogramWithReduce(ComputeShader s)
    {
        shader = s;
        kernelIdHisto  = shader.FindKernel(KERNEL_NAME_HISTO);
        kernelIdReduce = shader.FindKernel(KERNEL_NAME_REDUCE);
        totalThreadsInBlock = shader.TotalThreadsInBlock(kernelIdHisto);
    }

    public int[] Process(int[] data, int bins)
    {
        if (bins > totalThreadsInBlock) throw new ArgumentException("histo base shouldn't be larger than: "+totalThreadsInBlock);
        
        var grid = Utils.TrimToBlock(data.Length, totalThreadsInBlock);
        var sizePowerOfTwo  = Utils.NextPowerOfTwo(grid);
        
        var dataBuffer = new ComputeBuffer(data.Length, sizeof(int));
        var histoBuffer = new ComputeBuffer(bins * sizePowerOfTwo, sizeof(int));
        
        dataBuffer.SetData(data);

        shader.SetInt(PARAM_OFFSET, 0);
        shader.SetInt(PARAM_SIZE, data.Length);
        shader.SetInt(PARAM_BIN_SIZE, bins);
        shader.SetInts(PARAM_GRID, sizePowerOfTwo, 1, 1);

        shader.SetBuffer(kernelIdHisto, PARAM_BUFF_IN,  dataBuffer);
        shader.SetBuffer(kernelIdHisto, PARAM_BUFF_OUT, histoBuffer);
        shader.Dispatch(kernelIdHisto, sizePowerOfTwo, 1, 1);

        if (grid > 1)
        {
            shader.SetBuffer(kernelIdReduce, PARAM_BUFF_IN,  histoBuffer);
            shader.SetBuffer(kernelIdReduce, PARAM_BUFF_OUT, histoBuffer);
            for (var step = 1; step < sizePowerOfTwo; step *= totalThreadsInBlock)
            {
                var size = Utils.TrimToBlock(sizePowerOfTwo, step);
                var gridTotal = Utils.TrimToBlock(size, totalThreadsInBlock);
                
                shader.SetInt(PARAM_SIZE, size);
                shader.SetInt(PARAM_OFFSET, Math.Min(size, totalThreadsInBlock));
                shader.SetInts(PARAM_GRID, gridTotal, bins, 1);
                shader.Dispatch(kernelIdReduce, gridTotal, bins, 1);
            }
        }

        var result = new int[bins];
        histoBuffer.GetData(result);
        histoBuffer.Release();
        dataBuffer.Release();
        return result;
    }
}
