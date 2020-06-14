using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using static TrackOnMouse.Utility.WinApi.Kernel32;
using static TrackOnMouse.Utility.WinApi.User32;

namespace TrackOnMouse.Utility
{
    /// <summary>
    ///     Class for intercepting low level Windows mouse hooks.
    /// </summary>
    public class MouseHook
    {
        /// <summary>
        ///     Function to be called when defined even occurs
        /// </summary>
        /// <param name="mouseStruct"><see cref="MSLLHOOKSTRUCT"/> mouse structure</param>
        public delegate void MouseHookCallback(MSLLHOOKSTRUCT mouseStruct);

        private static MouseHook _hook;
        private static bool _isInstalled;
        private MouseHookHandler _hookHandler;

        /// <summary>
        ///     Low level mouse hook's ID
        /// </summary>
        private IntPtr _hookId = IntPtr.Zero;

        private MouseHook()
        {
        }

        public static MouseHook GetInstance()
        {
            return _hook ??= new MouseHook();
        }

        /// <summary>
        ///     Destructor. Unhook current hook
        /// </summary>
        ~MouseHook()
        {
            Uninstall();
        }

        /// <summary>
        ///     Install low level mouse hook
        /// </summary>
        public void Install()
        {
            if (_isInstalled)
                return;

            _hookHandler = HookFunc;
            _hookId = SetHook(_hookHandler);
            _isInstalled = true;
        }

        /// <summary>
        ///     Remove low level mouse hook
        /// </summary>
        public void Uninstall()
        {
            if (!_isInstalled)
                return;

            UnhookWindowsHookEx(_hookId);
            _hookId = IntPtr.Zero;
            _isInstalled = false;
        }

        /// <summary>
        ///     Sets hook and assigns its ID for tracking
        /// </summary>
        /// <param name="proc">Internal callback function</param>
        /// <returns>Hook ID</returns>
        private static IntPtr SetHook(MouseHookHandler proc)
        {
            using ProcessModule module = Process.GetCurrentProcess().MainModule;
            return SetWindowsHookEx(WH_MOUSE_LL, proc, GetModuleHandle(module?.ModuleName), 0);
        }

        /// <summary>
        ///     Callback function
        /// </summary>
        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        private IntPtr HookFunc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            // Parse system messages
            if (nCode >= 0)
            {
                if (MouseMessages.WM_LBUTTONDOWN == (MouseMessages) wParam)
                    LeftButtonDown?.Invoke(
                        (MSLLHOOKSTRUCT) Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT)));
                if (MouseMessages.WM_LBUTTONUP == (MouseMessages) wParam)
                    LeftButtonUp?.Invoke(
                        (MSLLHOOKSTRUCT) Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT)));
                if (MouseMessages.WM_RBUTTONDOWN == (MouseMessages) wParam)
                    RightButtonDown?.Invoke(
                        (MSLLHOOKSTRUCT) Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT)));
                if (MouseMessages.WM_RBUTTONUP == (MouseMessages) wParam)
                    RightButtonUp?.Invoke(
                        (MSLLHOOKSTRUCT) Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT)));
                if (MouseMessages.WM_MOUSEMOVE == (MouseMessages) wParam)
                    MouseMove?.Invoke(
                        (MSLLHOOKSTRUCT) Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT)));
                if (MouseMessages.WM_MOUSEWHEEL == (MouseMessages) wParam)
                    MouseWheel?.Invoke(
                        (MSLLHOOKSTRUCT) Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT)));
                if (MouseMessages.WM_LBUTTONDBLCLK == (MouseMessages) wParam)
                    DoubleClick?.Invoke(
                        (MSLLHOOKSTRUCT) Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT)));
                if (MouseMessages.WM_MBUTTONDOWN == (MouseMessages) wParam)
                    MiddleButtonDown?.Invoke(
                        (MSLLHOOKSTRUCT) Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT)));
                if (MouseMessages.WM_MBUTTONUP == (MouseMessages) wParam)
                    MiddleButtonUp?.Invoke(
                        (MSLLHOOKSTRUCT) Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT)));
            }

            return CallNextHookEx(_hookId, nCode, wParam, lParam);
        }

        /// <summary>
        ///     Internal callback processing function
        /// </summary>
        public delegate IntPtr MouseHookHandler(int nCode, IntPtr wParam, IntPtr lParam);

        #region Events

        public event MouseHookCallback LeftButtonDown;
        public event MouseHookCallback LeftButtonUp;
        public event MouseHookCallback RightButtonDown;
        public event MouseHookCallback RightButtonUp;
        public event MouseHookCallback MouseMove;
        public event MouseHookCallback MouseWheel;
        public event MouseHookCallback DoubleClick;
        public event MouseHookCallback MiddleButtonDown;
        public event MouseHookCallback MiddleButtonUp;

        #endregion
    }
}