using NUnit.Framework;
using ShaderUtils;

public abstract class BaseGpuHistogramTest {
	private IHistogram gpuHist;

	protected abstract IHistogram CreateGpuHistogram();
	
	[SetUp]
	public void SetUp()
	{
		gpuHist = CreateGpuHistogram();
	}
	
	[Test]
	public void Histogram_Sorted_256()
	{
		Test(256, 8, FillArrayType.Sorted);
	}
	
	[Test]
	public void Histogram_Reversed_256()
	{
		Test(256, 8, FillArrayType.Reversed);
	}
	
	[Test]
	public void Histogram_RandomNonNegative_256()
	{
		Test(256, 8, FillArrayType.RandomNonNegative);
	}
	
	[Test]
	public void Histogram_Random_256()
	{
		Test(256, 8, FillArrayType.Random);
	}
	
	[Test]
	public void Histogram_Sorted_512()
	{
		Test(512, 8, FillArrayType.Sorted);
	}

	[Test]
	public void Histogram_Sorted_10()
	{
		Test(10, 10, FillArrayType.Sorted);
	}
	
	[Test]
	public void Histogram_Random_10()
	{
		Test(10, 8, FillArrayType.Random);
	}
	
	[Test]
	public void Histogram_Constant_1000000()
	{
		Test(1000000, 10, FillArrayType.Constant);
	}

	[Test]
	public void Histogram_RandomNonNegative_1000000()
	{
		Test(1000000, 10, FillArrayType.RandomNonNegative);
	}
	
	private void Test(int length, int bins, FillArrayType type)
	{
		// setup:
		var data = new int[length];
		Utils.FillArray(data, type);
		
		// when:
		var cpuResult = Utils.Histogram(data, bins);
		var gpuResult = gpuHist.Process(data, bins);
		
		// then:
		Assert.AreEqual(gpuResult.Length, bins);
		Assert.AreEqual(Utils.Reduce(gpuResult), length);
		
		for (var i = 0; i < bins; ++i)
			Assert.AreEqual(gpuResult[i], cpuResult[i], "not eq at="+i);
	}
}
