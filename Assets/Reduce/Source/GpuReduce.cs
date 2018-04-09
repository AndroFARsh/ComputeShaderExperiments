using System;
using ShaderUtils;
using UnityEngine;

public class GpuReduce
{
   private const string KERNEL_NAME = "CSReduce";

   private const string PARAM_GRID = "GridDimen";
   private const string PARAM_DATA = "Data";
   private const string PARAM_SIZE = "Size";

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

      shader.SetBuffer(kernelId, PARAM_DATA, dataBuffer);
      for (var step = 1; step < length; step *= totalThreadsInBlock)
      {
         var size = length / step + (length % step != 0 ? 1 : 0);
         var gridTotal = RequiredGroups(size);
         
         shader.SetInt(PARAM_SIZE, size);   
         shader.SetInts(PARAM_GRID, gridTotal, 1, 1);
         shader.Dispatch(kernelId, gridTotal, 1, 1);
      }
      
      var result = new int[1];
      dataBuffer.GetData(result);
      dataBuffer.Release();
      return result[0];
   }

   private int RequiredGroups(int length)
   {
      return (length % totalThreadsInBlock != 0) ? ((length / totalThreadsInBlock) + 1) : (length / totalThreadsInBlock);
   }
}