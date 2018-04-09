using NUnit.Framework;
using ShaderUtils;

public abstract class BaseGpuScanTest
{
	private IScan gpuScan;
	private IScan cpuScan;

	protected abstract IScan CreateGpuScan();
	protected abstract IScan CreateCpuScan();
	
	[SetUp]
	public void SetUp()
	{
		gpuScan = CreateGpuScan();
		cpuScan = CreateCpuScan();
	}
	
	[Test]
	public void Scan_Sorted_256()
	{
		Test(256, FillArrayType.Sorted);
	}
	
	[Test]
	public void Scan_Reversed_256()
	{
		Test(256, FillArrayType.Reversed);
	}
	
	[Test]
	public void Scan_RandomNonNegative_256()
	{
		Test(256, FillArrayType.RandomNonNegative);
	}
	
	[Test]
	public void Scan_Random_256()
	{
		Test(256, FillArrayType.Random);
	}
	
	[Test]
	public void Reduce_Sorted_512()
	{
		Test(512, FillArrayType.Sorted);
	}

	[Test]
	public void Reduce_Sorted_10()
	{
		Test(10, FillArrayType.Sorted);
	}
	
	[Test]
	public void Scan_Constant_1000000()
	{
		Test(1000000, FillArrayType.Constant);
	}
	
	private void Test(int length, FillArrayType type)
	{
		// setup:
		var data = new int[length];
		Utils.FillArray(data, type);
		
		// when:
		var gpuResult = gpuScan.Scan(data);
		
		// then:
		Assert.AreEqual(gpuResult.Length, length);
		
		var cpuResult = cpuScan.Scan(data);
		for (var i = 0; i < length; ++i)
			Assert.AreEqual(gpuResult[i], cpuResult[i], "not eq at="+i);
	}
}
