using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace ShaderUtils
{
    public static class LogUtils
    {   
        public static void PrintArray<T>(IList<T> data, string title = "", Func<T, string> toString = null,
            int start = 0, int end = -1)
        {
            end = end < start || end < data.Count ? data.Count - 1 : end;

            var builder = new StringBuilder();
            builder.Append(title).Append(" [");
            for (var i=start; i <= end; ++i)
            {
                var value = data[i];
                builder.Append(toString != null ? toString(value) : value.ToString())
                       .Append(", ");

            }
            builder.Append(']');
            Debug.Log(builder.ToString());
        }

        public static void PrintArrayToFile<T>(string file, IList<T> data, string title = "",
            Func<T, string> toString = null, int start = 0, int end = -1)
        {
            end = end < start || end < data.Count ? data.Count - 1 : end;
            //if (File.Exists(file))
            
            var sw = File.AppendText(file); 
            sw.Write(title);
            sw.Write(" [");
            for (var i=start; i <= end; ++i)
            {
                var value = data[i];
            
                sw.Write(toString != null ? toString(value) : value.ToString());
                sw.Write(", ");
            }
            sw.Write("]\n");
            sw.Close();
        }
    }
}