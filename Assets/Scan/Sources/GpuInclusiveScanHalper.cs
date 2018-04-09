using UnityEngine;

public class GpuInclusiveScanHalper : BaseGpuScanHalper
{
    protected override IScan CreateScanner(ComputeShader shader)
    {
        return new GpuInclusiveScan(shader);
    }
}
