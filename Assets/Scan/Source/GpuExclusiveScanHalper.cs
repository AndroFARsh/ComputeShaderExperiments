using UnityEngine;

public class GpuExclusiveScanHalper : BaseGpuScanHalper
{
    protected override IScan CreateScanner(ComputeShader shader)
    {
        return new GpuExclusiveScan(shader);
    }
}
