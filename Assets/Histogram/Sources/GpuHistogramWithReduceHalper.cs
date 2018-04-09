using UnityEngine;

public class GpuHistogramWithReduceHalper : BaseGpuHistogramHalper {
    protected override IHistogram CeateHistogram(ComputeShader shader)
    {
        return new GpuHistogramWithReduce(shader);
    }
}
