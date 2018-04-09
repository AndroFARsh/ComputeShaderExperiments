using System.Diagnostics;
using ShaderUtils;
using UnityEngine;

public abstract class BaseGpuScanHalper : MonoBehaviour
{
	[Header("In play mode pres 'Enter' to reduce")]   
	[SerializeField] private ComputeShader shader;
	[SerializeField] private FillArrayType type = FillArrayType.Sorted;
	[SerializeField] private int 		   size = 512;
	[SerializeField] private int 		   maxValue = 10;
	[SerializeField] private int 		   constValue = 1;
	[SerializeField] private bool 		   printArray;
	
	private int[] 						   data;
	private IScan 					   	   scanner;
	
	// Use this for initialization
	private void Start () {
		scanner = CreateScanner(shader);
	}

	protected abstract IScan CreateScanner(ComputeShader shader);
	
	// Update is called once per frame
	private void Update () {
		if (!Input.GetKeyDown(KeyCode.Return)) return;

		IntArray(ref data, size, type, maxValue, constValue);
		if (printArray) LogUtils.PrintArray(data, "In: ");
            
		var sw = Stopwatch.StartNew();
		var scan = scanner.Scan(data);
		sw.Stop();

		LogUtils.PrintArray(scan, "Scan(" + sw.ElapsedMilliseconds + "ms):");
	}
	
	private static void IntArray(ref int[] initial, int size, FillArrayType type, int maxValue, int constValue)
	{
		initial = (initial == null || initial.Length != size) ? new int[size] : initial;
		Utils.FillArray(initial, type, type == FillArrayType.Constant ? constValue : maxValue);
	}
}
