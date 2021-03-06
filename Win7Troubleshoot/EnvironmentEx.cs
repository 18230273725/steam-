// https://www.codeproject.com/questions/641314/check-64-or-32-bit-in-net-3-5
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Win7Troubleshoot
{
    public static class EnvironmentEx
    {
        [DllImport("kernel32", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
        internal extern static IntPtr LoadLibrary(string libraryName);

        [DllImport("kernel32", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
        internal extern static IntPtr GetProcAddress(IntPtr hwnd, string procedureName);

        delegate bool IsWow64ProcessDelegate([In] IntPtr handle, [Out] out bool isWow64Process);

        public static bool Is64BitOperatingSystem
        {
            get
            {
                if (IntPtr.Size == 8 || (IntPtr.Size == 4 && Is32BitProcessOn64BitProcessor()))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        static IsWow64ProcessDelegate GetIsWow64ProcessDelegate()
        {
            IntPtr handle = LoadLibrary("kernel32");

            if (handle != IntPtr.Zero)
            {
                IntPtr fnPtr = GetProcAddress(handle, "IsWow64Process");

                if (fnPtr != IntPtr.Zero)
                {
                    return (IsWow64ProcessDelegate)Marshal.GetDelegateForFunctionPointer((IntPtr)fnPtr, typeof(IsWow64ProcessDelegate));
                }
            }

            return null;
        }

        static bool Is32BitProcessOn64BitProcessor()
        {
            IsWow64ProcessDelegate fnDelegate = GetIsWow64ProcessDelegate();

            if (fnDelegate == null)
            {
                return false;
            }

            bool retVal = fnDelegate.Invoke(Process.GetCurrentProcess().Handle, out var isWow64);

            if (retVal == false)
            {
                return false;
            }

            return isWow64;
        }
    }
}