using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Bin_Obj_Delete_Project.Models
{
    public class GlobalMouseHook
    {
        // [Win32 API] 함수 선언 1
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

        // [Win32 API] 함수 선언 2
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        // [Win32 API] 함수 선언 3
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        // [Win32 API] 함수 선언 4
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);
        private const int WH_MOUSE_LL = 14;
        private const int WM_LBUTTONDOWN = 0x0201;
        private const int WM_RBUTTONDOWN = 0x0204;
        private readonly LowLevelMouseProc _proc;
        private IntPtr _hookID = IntPtr.Zero;

        public GlobalMouseHook()
        {
            _proc = HookCallback;
        }

        /// <summary>
        /// 1. [마우스 후킹] 적용
        /// 2. [마우스 클릭] 차단
        /// 3. [마우스 입력] 차단
        /// </summary>
        public void HookMouse()
        {
            _hookID = SetHook(_proc);
        }

        /// <summary>
        /// 1. [마우스 후킹] 해제
        /// 2. [마우스 클릭] 가능
        /// 3. [마우스 입력] 가능
        /// </summary>
        public void UnhookMouse()
        {
            _ = UnhookWindowsHookEx(_hookID);
        }

        private IntPtr SetHook(LowLevelMouseProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_MOUSE_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
            }

        }

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && (wParam == (IntPtr)WM_LBUTTONDOWN || wParam == (IntPtr)WM_RBUTTONDOWN))
            {
                return (IntPtr)1; // 마우스 클릭을 차단 (1을 반환)
            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

    }

}
