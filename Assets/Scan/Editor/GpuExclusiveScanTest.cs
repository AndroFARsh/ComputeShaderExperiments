using ShaderUtils;
using UnityEngine;
using UnityEditor;

public class GpuExclusiveScanTest : BaseGpuScanTest {
	protected override IScan CreateGpuScan()
	{
		var shader = AssetDatabase.LoadAssetAtPath<ComputeShader>("Assets/Scan/Shader/GpuExclusiveScan.compute");
		return new GpuExclusiveScan(shader);
	}

	protected override IScan CreateCpuScan()
	{
		return new CpuExclusiveScan();
	}

	private class CpuExclusiveScan : IScan
	{
		public int[] Scan(int[] data)
		{
			return Utils.ExclusiveScan(data);
		}
	}
}
