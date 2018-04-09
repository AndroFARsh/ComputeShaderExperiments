using UnityEditor;
using UnityEngine;

public class GpuHistogramTest : BaseGpuHistogramTest{
	protected override IHistogram CreateGpuHistogram()
	{
		var shader = AssetDatabase.LoadAssetAtPath<ComputeShader>("Assets/Histogram/Shader/GpuHistogram.compute");
		return new GpuHistogram(shader);
	}
}
