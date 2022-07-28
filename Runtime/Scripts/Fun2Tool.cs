using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Fun2
{
    
    public class ToolKit
    {
        internal delegate void LogCallBack(int level, System.IntPtr msg, int len);
        internal static LogCallBack logger = new LogCallBack(LogFunc);
        private static void LogFunc(int level, System.IntPtr msg, int len)
        {
            string format = $"<color=green>KmaxXR</color> {Marshal.PtrToStringAnsi(msg, len)}";
            switch (level)
            {
                case 0:
                    Debug.Log(format);
                    break;
                case 1:
                    Debug.LogWarning(format);
                    break;
                case 2:
                    Debug.LogError(format);
                    break;
                default:
                    break;
            }
        }

        public static void ActiveLog(bool enable = true)
        {
            SetLogger(enable ? logger : null);
        }
        const string DllName = "fun2.tool";
        [DllImport(DllName)]
        internal static extern void SetLogger(LogCallBack logger);
        [DllImport(DllName)]
        internal static extern void TestLogger();
        [DllImport(DllName, CharSet = CharSet.Auto)]
        public extern static void SaveTexture(System.IntPtr tex, string path);
    }

}
