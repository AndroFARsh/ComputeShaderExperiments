using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using ShaderUtils;

public class GpuReducerTest {
	
	private GpuReduce reducer;

	[SetUp]
	public void SetUp()
	{
		var shader = AssetDatabase.LoadAssetAtPath<ComputeShader>("Assets/Reduce/Shader/GpuReduce.compute");
		reducer = new GpuReduce(shader);
	}
	
	[Test]
	public void Reduce_Sorted_256()
	{
		// setup:
		var data = new int[256];
		Utils.FillArray(data, FillArrayType.Sorted);
		
		// when:
		var result = reducer.Reduce(data);

		// then:
		Assert.AreEqual(result, Utils.Reduce(data));
	}
	
	[Test]
	public void Reduce_Reversed_256()
	{
		// setup:
		var data = new int[256];
		Utils.FillArray(data, FillArrayType.Reversed);
		
		// when:
		var result = reducer.Reduce(data);

		// then:
		Assert.AreEqual(result, Utils.Reduce(data));
	}
	
	[Test]
	public void Reduce_RandomNonNegative_256()
	{
		// setup:
		var data = new int[256];
		Utils.FillArray(data, FillArrayType.RandomNonNegative);
		
		// when:
		var result = reducer.Reduce(data);

		// then:
		Assert.AreEqual(result, Utils.Reduce(data));
	}
	
	[Test]
	public void Reduce_Random_256()
	{
		// setup:
		var data = new int[256];
		Utils.FillArray(data, FillArrayType.Random);
		
		// when:
		var result = reducer.Reduce(data);

		// then:
		Assert.AreEqual(result, Utils.Reduce(data));
	}
	
	[Test]
	public void Reduce_Sorted_512()
	{
		// setup:
		var data = new int[512];
		Utils.FillArray(data, FillArrayType.Sorted);
		
		// when:
		var result = reducer.Reduce(data);

		// then:
		Assert.AreEqual(result, Utils.Reduce(data));
	}

	[Test]
	public void Reduce_Sorted_10()
	{
		// setup:
		var data = new int[10];
		Utils.FillArray(data, FillArrayType.Sorted);
		
		// when:
		var result = reducer.Reduce(data);

		// then:
		Assert.AreEqual(result, Utils.Reduce(data));
	}
	
	[Test]
	public void Reduce_Constant_1000000()
	{
		// setup:
		var data = new int[1000000];
		Utils.FillArray(data, FillArrayType.Constant, 1);
		
		// when:
		var result = reducer.Reduce(data);

		// then:
		Assert.AreEqual(result, 1000000);
	}
}
