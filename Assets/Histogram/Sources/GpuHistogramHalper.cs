using UnityEngine;

public class GpuHistogramHalper : BaseGpuHistogramHalper {
    protected override IHistogram CeateHistogram(ComputeShader shader)
    {
        return new GpuHistogram(shader);
    }
}
