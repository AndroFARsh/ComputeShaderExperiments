using ShaderUtils;
using UnityEngine;

public class GpuExclusiveScan: IScan
{
   private const string KERNEL_NAME_SCAN       = "CSExclusiveScan";
   private const string KERNEL_NAME_STORE_SUM  = "CSStoreBlockSum";
   private const string KERNEL_NAME_ADD_SUM    = "CSAddBlockSum";

   private const string PARAM_GRID             = "g_gridDimen";
   private const string PARAM_DATA_IN          = "g_in";
   private const string PARAM_DATA_OUT         = "g_out";
   private const string PARAM_SIZE             = "g_size";

   private readonly ComputeShader shader;
   private readonly int kernelIdScan;
   private readonly int kernelIdStoreSum;
   private readonly int kernelIdAddSum;
   private readonly int totalThreadsInBlock;

   public GpuExclusiveScan(ComputeShader s)
   {
      shader = s;
      kernelIdScan = shader.FindKernel(KERNEL_NAME_SCAN);
      kernelIdStoreSum = shader.FindKernel(KERNEL_NAME_STORE_SUM);
      kernelIdAddSum = shader.FindKernel(KERNEL_NAME_ADD_SUM);
      totalThreadsInBlock = shader.TotalThreadsInBlock(kernelIdScan);
   }

   public int[] Scan(int[] data)
   {
      var result = new int[data.Length];
      var length = Utils.NextPowerOfTwo(data.Length);
      
      var gridTotal = Utils.TrimToBlock(length, totalThreadsInBlock);
      var dataBuffer = new ComputeBuffer(length + gridTotal, sizeof(int));
      dataBuffer.SetData(data);

      shader.SetBuffer(kernelIdScan, PARAM_DATA_IN, dataBuffer);
      shader.SetBuffer(kernelIdScan, PARAM_DATA_OUT, dataBuffer);
      
      shader.SetInt(PARAM_SIZE, length);   
      shader.SetInts(PARAM_GRID, gridTotal, 1, 1);
      shader.Dispatch(kernelIdScan, gridTotal, 1, 1);

      HandleSumBuffer(dataBuffer, length);
      
      dataBuffer.GetData(result);
      dataBuffer.Release();
      return result;
   }

   private void HandleSumBuffer(ComputeBuffer dataBuffer, int length)
   {
      if (length < totalThreadsInBlock) return;
      
      var size = Utils.TrimToBlock(length, totalThreadsInBlock);
      var gridTotal = Utils.TrimToBlock(size, totalThreadsInBlock);
      var sumBuffer = new ComputeBuffer(size + gridTotal, sizeof(int));

      StoreBlockSum(dataBuffer, length, gridTotal, sumBuffer);
      ScanSumBuffer(size, gridTotal, sumBuffer);

      HandleSumBuffer(sumBuffer, size);
      AddSumToData(dataBuffer, length, gridTotal, sumBuffer);
      sumBuffer.Release();
   }

   private void AddSumToData(ComputeBuffer dataBuffer, int length, int gridTotal, ComputeBuffer sumBuffer)
   {
      shader.SetInt(PARAM_SIZE, length);
      shader.SetInts(PARAM_GRID, gridTotal, 1, 1);

      shader.SetBuffer(kernelIdAddSum, PARAM_DATA_IN, sumBuffer);
      shader.SetBuffer(kernelIdAddSum, PARAM_DATA_OUT, dataBuffer);
      shader.Dispatch(kernelIdAddSum, gridTotal, 1, 1);
   }

   private void ScanSumBuffer(int size, int gridTotal, ComputeBuffer sumBuffer)
   {
      shader.SetInt(PARAM_SIZE, size);
      shader.SetInts(PARAM_GRID, gridTotal, 1, 1);

      shader.SetBuffer(kernelIdScan, PARAM_DATA_IN, sumBuffer);
      shader.SetBuffer(kernelIdScan, PARAM_DATA_OUT, sumBuffer);
      shader.Dispatch(kernelIdScan, gridTotal, 1, 1);
   }

   private void StoreBlockSum(ComputeBuffer dataBuffer, int size, int gridTotal, ComputeBuffer sumBuffer)
   {
      shader.SetInt(PARAM_SIZE, size);
      shader.SetInts(PARAM_GRID, gridTotal, 1, 1);

      shader.SetBuffer(kernelIdStoreSum, PARAM_DATA_IN, dataBuffer);
      shader.SetBuffer(kernelIdStoreSum, PARAM_DATA_OUT, sumBuffer);
      shader.Dispatch(kernelIdStoreSum, gridTotal, 1, 1);
   }
}