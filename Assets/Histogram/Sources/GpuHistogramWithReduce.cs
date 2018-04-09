using System;
using ShaderUtils;
using UnityEngine;

public class GpuHistogramWithReduce : IHistogram
{
    private const string KERNEL_NAME_HISTO      = "CSHistogram";
    private const string KERNEL_NAME_REDUCE     = "CSReduce";
    private const string KERNEL_NAME_DEFRAG     = "CSDefragmentation";
    
    private const string PARAM_GRID             = "g_gridDimen";
    private const string PARAM_BUFF_IN          = "g_in";
    private const string PARAM_BUFF_OUT         = "g_out";
    private const string PARAM_SIZE             = "g_size";
    private const string PARAM_BIN_SIZE         = "g_binSize";
    private const string PARAM_OFFSET           = "g_offset";

    private readonly ComputeShader shader;
    private readonly int kernelIdHisto;
    private readonly int kernelIdReduce;
    private readonly int kernelIdDefrag;
    private readonly int totalThreadsInBlock;
    
    public GpuHistogramWithReduce(ComputeShader s)
    {
        shader = s;
        kernelIdHisto  = shader.FindKernel(KERNEL_NAME_HISTO);
        kernelIdReduce = shader.FindKernel(KERNEL_NAME_REDUCE);
        kernelIdDefrag = shader.FindKernel(KERNEL_NAME_DEFRAG);
        totalThreadsInBlock = shader.TotalThreadsInBlock(kernelIdHisto);
    }

    public int[] Process(int[] data, int bins)
    {
        if (bins > totalThreadsInBlock) throw new ArgumentException("histo base shouldn't be larger than: "+totalThreadsInBlock);
        
        var grid = Utils.TrimToBlock(data.Length, totalThreadsInBlock);
        
        var dataBuffer = new ComputeBuffer(data.Length, sizeof(int));
        var histoBuffer = new ComputeBuffer(bins * grid, sizeof(int));
        var testBuffer = new ComputeBuffer(bins * grid, sizeof(int));
        
        dataBuffer.SetData(data);

        shader.SetInt(PARAM_OFFSET, 0);
        shader.SetInt(PARAM_SIZE, data.Length);
        shader.SetInt(PARAM_BIN_SIZE, bins);
        shader.SetInts(PARAM_GRID, grid, 1, 1);

        shader.SetBuffer(kernelIdHisto, PARAM_BUFF_IN,  dataBuffer);
        shader.SetBuffer(kernelIdHisto, PARAM_BUFF_OUT, histoBuffer);
        shader.Dispatch(kernelIdHisto, grid, 1, 1);

        {
            var tmp = new int[grid * bins];
            histoBuffer.GetData(tmp);
            LogUtils.PrintArray(tmp, "histo {"+grid+"}");

            var indx = -1;
            var t = new int[bins];
            for (var i = 0; i < tmp.Length; ++i)
            {
                if (i % grid == 0) ++indx;
                t[indx] += tmp[i];
            }
            
            LogUtils.PrintArray(t, "histo {"+grid+"}");
        }

        if (grid > 1)
        {
            shader.SetBuffer(kernelIdReduce, PARAM_BUFF_IN,  histoBuffer);
            shader.SetBuffer(kernelIdReduce, PARAM_BUFF_OUT, histoBuffer);
            shader.SetBuffer(kernelIdReduce, "g_test", testBuffer);

            var sizePowerOfTwo  = Utils.NextPowerOfTwo(grid);
            for (var step = 1; step < sizePowerOfTwo; step *= totalThreadsInBlock)
            {
                var size = Utils.TrimToBlock(sizePowerOfTwo, step);
                var gridTotal = Utils.TrimToBlock(size, totalThreadsInBlock);

                shader.SetInt(PARAM_SIZE, Math.Min(size, grid));
                shader.SetInt(PARAM_OFFSET, grid);
                shader.SetInt(PARAM_BIN_SIZE, bins);
                shader.SetInts(PARAM_GRID, gridTotal, bins, 1);
                shader.Dispatch(kernelIdReduce, gridTotal, bins, 1);
                
                {
                    var tmp = new int[grid * bins];
                    histoBuffer.GetData(tmp);
                    LogUtils.PrintArray(tmp, "gpu histo {"+step+"|"+ Math.Min(size, grid)+"|"+totalThreadsInBlock+"}");
//                    
//                    var indx = -1;
//                    for (var i = 0; i < tmp.Length; ++i)
//                    {
//                        if (i % (Math.Min(size, grid) / gridTotal) == 0) indx = i;
//                        if (indx != i) tmp[indx] += tmp[i];
//                    }
//                    LogUtils.PrintArray(tmp, "cpu histo {"+step+"/"+size+"}");
                    
                    testBuffer.GetData(tmp);
                    LogUtils.PrintArray(tmp, "gpu test {"+step+"|"+ Math.Min(size, grid)+"|"+totalThreadsInBlock+"}");
                    
                }
            }

            {
                shader.SetBuffer(kernelIdDefrag, PARAM_BUFF_IN,  histoBuffer);
                shader.SetBuffer(kernelIdDefrag, PARAM_BUFF_OUT, histoBuffer);
                
                var gridTotal = Utils.TrimToBlock(bins, totalThreadsInBlock);
                shader.SetInt(PARAM_SIZE, bins * grid);
                shader.SetInt(PARAM_OFFSET, grid);   
                shader.SetInt(PARAM_BIN_SIZE, bins);
                shader.SetInts(PARAM_GRID, gridTotal, 1, 1);
                shader.Dispatch(kernelIdDefrag, gridTotal, 1, 1);
            }
        }

        var result = new int[bins];
        histoBuffer.GetData(result);
        histoBuffer.Release();
        dataBuffer.Release();
        testBuffer.Release();
        return result;
    }
}
