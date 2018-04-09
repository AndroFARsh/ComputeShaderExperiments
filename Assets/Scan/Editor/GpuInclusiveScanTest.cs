using ShaderUtils;
using UnityEngine;
using UnityEditor;

public class GpuInclusiveScanTest : BaseGpuScanTest {
	protected override IScan CreateGpuScan()
	{
		var shader = AssetDatabase.LoadAssetAtPath<ComputeShader>("Assets/Scan/Shader/GpuInclusiveScan.compute");
		return new GpuInclusiveScan(shader);
	}

	protected override IScan CreateCpuScan()
	{
		return new CpuInclusiveScan();
	}

	private class CpuInclusiveScan : IScan
	{
		public int[] Scan(int[] data)
		{
			return Utils.InclusiveScan(data);
		}
	}
}
