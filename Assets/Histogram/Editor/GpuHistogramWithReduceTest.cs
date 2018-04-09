using UnityEditor;
using UnityEngine;

public class GpuHistogramWithReduceTest : BaseGpuHistogramTest{
	protected override IHistogram CreateGpuHistogram()
	{
		var shader = AssetDatabase.LoadAssetAtPath<ComputeShader>("Assets/Histogram/Shader/GpuHistogramWithReduce.compute");
		return new GpuHistogramWithReduce(shader);
	}
}
