using System;
using System.Runtime.InteropServices;
using System.Windows;

namespace NoelPush.Services
{
    public class CUploader
    {
        [DllImport("NoelPushC.dll", CallingConvention = CallingConvention.Cdecl)]
        unsafe static extern char* Upload(byte* data);

        public unsafe void Upload(byte[] picture)
        {
            byte* data;
            fixed (byte* bptr = picture)
            {
                data = bptr;
            }

            var a = Marshal.PtrToStringAnsi((IntPtr)Upload(data));
            MessageBox.Show(a);
        }
    }
}
