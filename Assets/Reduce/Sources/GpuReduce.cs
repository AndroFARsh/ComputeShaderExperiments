using System;
using ShaderUtils;
using UnityEngine;

public class GpuReduce
{
   private const string KERNEL_NAME = "CSReduce";

   private const string PARAM_GRID = "g_gridDimen";
   private const string PARAM_IN   = "g_in";
   private const string PARAM_OUT  = "g_out";
   private const string PARAM_SIZE = "g_size";
   
   private readonly ComputeShader shader;
   private readonly int kernelId;
   private readonly int totalThreadsInBlock;

   public GpuReduce(ComputeShader s)
   {
      shader = s;
      kernelId = shader.FindKernel(KERNEL_NAME);
      totalThreadsInBlock = shader.TotalThreadsInBlock(kernelId);
   }

   public int Reduce(int[] data)
   {
      var length = Utils.NextPowerOfTwo(data.Length);
      var dataBuffer = new ComputeBuffer(length, sizeof(int));
      dataBuffer.SetData(data);

      shader.SetBuffer(kernelId, PARAM_IN, dataBuffer);
      shader.SetBuffer(kernelId, PARAM_OUT, dataBuffer);
      for (var step = 1; step < length; step *= totalThreadsInBlock)
      {
         var size = Utils.TrimToBlock(length, step);
         var gridTotal = Utils.TrimToBlock(size, totalThreadsInBlock);
         
         shader.SetInt(PARAM_SIZE, size);   
         shader.SetInts(PARAM_GRID, gridTotal, 1, 1);
         shader.Dispatch(kernelId, gridTotal, 1, 1);
      }
      
      var result = new int[length];
      dataBuffer.GetData(result);
      dataBuffer.Release();
      return result[0];
   }
}