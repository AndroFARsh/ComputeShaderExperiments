using System.Diagnostics;
using ShaderUtils;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class GpuReduceHalper : MonoBehaviour
{
	[Header("In play mode pres 'Enter' to reduce")]   
	[SerializeField] private ComputeShader shader;
	[SerializeField] private FillArrayType type = FillArrayType.Sorted;
	[SerializeField] private int 		   size = 512;
	[SerializeField] private int 		   maxValue = 10;
	[SerializeField] private int 		   constValue = 1;
	[SerializeField] private bool 		   printArray;
	
	private int[] 						   data;
	private GpuReduce 					   reduser;
	
	// Use this for initialization
	private void Start () {
		reduser = new GpuReduce(shader);
	}
	
	// Update is called once per frame
	private void Update () {
		if (!Input.GetKeyDown(KeyCode.Return)) return;

		IntArray(ref data, size, type, maxValue, constValue);
		if (printArray) LogUtils.PrintArray(data, "In: ");
            
		var sw = Stopwatch.StartNew();
		var reduce = reduser.Reduce(data);
		sw.Stop();

		Debug.Log("Reduce: gpu="+reduce + " " + sw.ElapsedMilliseconds + "ms");
		if (printArray) Debug.Log("Reduce: cpu="+Utils.Reduce(data));
	}
	
	private static void IntArray(ref int[] initial, int size, FillArrayType type, int maxValue, int constValue)
	{
		initial = (initial == null || initial.Length != size) ? new int[size] : initial;
		Utils.FillArray(initial, type, type == FillArrayType.Constant ? constValue : maxValue);
	}
}
