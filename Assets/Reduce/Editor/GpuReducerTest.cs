using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using ShaderUtils;

public class GpuReducerTest {
	
	private GpuReduce reducer;

	[SetUp]
	public void SetUp()
	{
		var shader = AssetDatabase.LoadAssetAtPath<ComputeShader>("Assets/Reduce/Shaders/GpuReduce.compute");
		reducer = new GpuReduce(shader);
	}
	
	[Test]
	public void Reduce_Sorted_256()
	{
		Test(256, FillArrayType.Sorted);
	}
	
	[Test]
	public void Reduce_Reversed_256()
	{
		Test(256, FillArrayType.Reversed);
	}
	
	[Test]
	public void Reduce_RandomNonNegative_256()
	{
		Test(256, FillArrayType.RandomNonNegative);
	}
	
	[Test]
	public void Reduce_Random_256()
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
	public void Reduce_Constant_1000000()
	{
		Test(1000000, FillArrayType.Constant);
	}
	
	[Test]
	public void Reduce_Random_1000000()
	{
		Test(1000000, FillArrayType.Random);
	}
	
	private void Test(int length, FillArrayType type)
	{
		// setup:
		var data = new int[length];
		Utils.FillArray(data, type);
		
		// when:
		var result = reducer.Reduce(data);
		
		// then:
		Assert.AreEqual(result, Utils.Reduce(data));
	}
}
