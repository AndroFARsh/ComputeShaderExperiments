using System;
using UnityEngine;

namespace ShaderUtils
{
    public static class ComputeShaderExt
    {
        public static int TotalThreadsInBlock(this ComputeShader shader, int kernelId)
        {
            uint x;
            uint y;
            uint z;
            shader.GetKernelThreadGroupSizes(kernelId, out x, out y, out z);
      
            return Math.Max((int) (x * y * z), 1);
        }
    }
}