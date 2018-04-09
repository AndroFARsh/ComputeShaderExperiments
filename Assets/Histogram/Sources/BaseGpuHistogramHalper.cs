using System.Diagnostics;
using ShaderUtils;
using UnityEngine;

public abstract class BaseGpuHistogramHalper : MonoBehaviour {

	[Header("In play mode pres 'Enter' to calculate histogram with base 8")]   
	[SerializeField] private ComputeShader shader;
	[SerializeField] private FillArrayType type = FillArrayType.Sorted;
	[SerializeField] private int 		   size = 512;
	[SerializeField] private int 		   maxValue = 10;
	[SerializeField] private int 		   binsValue = 10;
	[SerializeField] private bool 		   printArray;
	
	private int[] 						   data;
	private IHistogram 					   histogram;
	
	// Use this for initialization
	private void Start () {
		histogram = CeateHistogram(shader);
	}

	protected abstract IHistogram CeateHistogram(ComputeShader shader);

	// Update is called once per frame
	private void Update () {
		if (!Input.GetKeyDown(KeyCode.Return)) return;

		IntArray(ref data, size, type, maxValue);
		if (printArray) LogUtils.PrintArray(data, histogram.GetType().Name+" In: ");
            
		var sw = Stopwatch.StartNew();
		var histo = histogram.Process(data, binsValue);
		sw.Stop();

		LogUtils.PrintArray(histo, histogram.GetType().Name+"(" + sw.ElapsedMilliseconds + "ms):");
		if (printArray) LogUtils.PrintArray(Utils.Histogram(data, binsValue), histogram.GetType().Name+"Serial:");
	}
	
	private static void IntArray(ref int[] initial, int size, FillArrayType type, int maxValue)
	{
		initial = (initial == null || initial.Length != size) ? new int[size] : initial;
		Utils.FillArray(initial, type, maxValue);
	}
}
