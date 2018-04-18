
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ShaderUtils
{
    public enum FillArrayType
    {
        Random,
        RandomNonNegative,
        Reversed,
        DubleReversed,
        Sorted,
        Constant
    }

    public static class Utils
    {
        private static readonly System.Random randNum = new System.Random();

        public static void FillArray(int[] array, FillArrayType type, int maxValue = 10, int defaultValue = 1)
        {
            for (var i = 0; i < array.Length; ++i)
            {
                switch (type)
                {
                    case FillArrayType.Constant:
                        array[i] = defaultValue;
                        break;
                    case FillArrayType.Random:
                        array[i] = randNum.Next(-maxValue, maxValue);
                        break;
                    case FillArrayType.RandomNonNegative:
                        array[i] = randNum.Next(0, maxValue);
                        break;
                    case FillArrayType.Reversed:
                        array[i] = array.Length - i;
                        break;
                    case FillArrayType.DubleReversed:
                        array[i] = array.Length - (i % 2 == 0 ? i : i - 1);
                        break;
                    case FillArrayType.Sorted:
                    default:
                        array[i] = i;
                        break;
                }
            }
        }

        public static Quaternion Normalize(this Quaternion q)
        {
            var length = Mathf.Sqrt(q.x * q.x + q.y * q.y + q.z * q.z + q.w * q.w);
            return new Quaternion(q.x / length, q.y / length, q.z / length, q.w / length);
        }

        public static int To1D(Vector3Int index, Vector3Int dimen)
        {
            return index.z * dimen.x * dimen.y + index.y * dimen.x + index.x;
        }

        public static Vector3Int To3D(int index, Vector3Int dimen)
        {
            var z = index / (dimen.x * dimen.y);
            index -= z * dimen.x * dimen.y;
            var y = index / dimen.x;
            var x = index % dimen.x;
            return new Vector3Int(x, y, z);
        }
        
        public static int Reduce(IEnumerable<int> data)
        {
            return data.Sum();
        }
        
        public static int[] InclusiveScan(int[] data)
        {
            var sum = 0;
            var result = new int[data.Length];
            for (var i = 0; i < data.Length; ++i)
            {
                sum += data[i];
                result[i] = sum;
            }
            return result;
        }
        
        public static int[] ExclusiveScan(int[] data)
        {
            var sum = 0;
            var result = new int[data.Length];
            for (var i = 0; i < data.Length; ++i)
            {
                result[i] = sum;
                sum += data[i];
            }
            return result;
        }
        
        public static int[] Histogram(int[] data, int binsCount)
        {
            var bins = new int[binsCount];
            for (var i = 0; i < data.Length; ++i)
            {
                var index = System.Math.Abs(data[i] % binsCount);
                ++bins[index];
            }
            return bins;
        }
        
        public static int NextPowerOfTwo(int v)
        {
            v--;
            v |= v >> 1;
            v |= v >> 2;
            v |= v >> 4;
            v |= v >> 8;
            v |= v >> 16;
            v++;
            return v;
        }

        public static int TrimToBlock(int length, int block)
        {
            return (length % block != 0) ? ((length / block) + 1) : (length / block);
        }
    }
}