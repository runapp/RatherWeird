﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DirtyInvocation
{
    public static class WindowInvocation
    {
        [DllImport("user32.dll", SetLastError = true)]
        static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);

        [DllImport("user32.dll")]
        static extern bool ClipCursor(ref RECT lpRect);

        [DllImport("user32.dll")]
        static extern bool GetClipCursor(out RECT lpRect);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern long GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
        private static extern int SetWindowLong32(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
        private static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, int nIndex, long dwNewLong);

        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        public static extern IntPtr SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int y, int cx, int cy,
            uint wFlags);


        // This static method is required because legacy OSes do not support
        // SetWindowLongPtr
        public static IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, long dwNewLong)
        {
            if (IntPtr.Size == 8)
                return SetWindowLongPtr64(hWnd, nIndex, dwNewLong);
            
            return new IntPtr(SetWindowLong32(hWnd, nIndex, (int)dwNewLong));
        }


        public static void LockToProcess(Process process)
        {
            long style = GetWindowLong(process.MainModule.BaseAddress, (int)GwlIndex.GWL_STYLE);
            Console.WriteLine($"Style: {style}");

            RECT procRect;
            GetWindowRect(process.MainWindowHandle, out procRect);

            ClipCursor(ref procRect);

        }

        public static void DropBorder(Process process)
        {
            long style = GetWindowLong(process.MainWindowHandle, (int)GwlIndex.GWL_STYLE);

            style &= ~((long) WindowStyles.WS_CAPTION | (long) WindowStyles.WS_MAXIMIZE | (long) WindowStyles.WS_MINIMIZE |
                       (long) WindowStyles.WS_SYSMENU);

            SetWindowLongPtr(process.MainWindowHandle, (int) GwlIndex.GWL_STYLE, style);
        }

        public static void ResizeWindow(Process process)
        {
            // TODO: Drop this reference to winforms and PInvoke it?
            Screen currentOccupiedScreen = Screen.FromHandle(process.MainWindowHandle);
            RECT procRect;
            GetWindowRect(process.MainWindowHandle, out procRect);

            int width = procRect.Right - procRect.Left;
            int height = procRect.Bottom - procRect.Top;

            SetWindowPos(process.MainWindowHandle, 0, currentOccupiedScreen.Bounds.X, currentOccupiedScreen.Bounds.Y,
                width, height, (uint)WindowSizing.SWP_FRAMECHANGED);
        }
    }

    public enum GwlIndex
    {
        GWL_EXSTYLE = -20,
        GWL_HINSTANCE = -6,
        GWL_HWNDPARENT = -8,
        GWL_ID = -12,
        GWL_STYLE = -16,
        GWL_USERDATA = -21,
        GWL_WNDPROC = -4,
    }

    public enum MONITOR : uint
    {
        MONITOR_DEFAULTTONULL= 0,
        MONITOR_DEFAULTTOPRIMARY = 1,
        MONITOR_DEFAULTTONEAREST = 2
    }

    public enum WindowSizing : uint
    {
        SWP_FRAMECHANGED = 0x0020
    }

    public enum WindowStyles : long
    {
        WS_BORDER = 0x00800000L,
        WS_MINIMIZE = 0x20000000,
        WS_MAXIMIZE = 0x01000000,
        WS_CAPTION = 0x00C00000,
        WS_SYSMENU = 0x00080000,
        WS_THICKFRAME = 0x00040000
    }

    public struct RECT
    {
        #region Variables.
        /// <summary>
        /// Left position of the rectangle.
        /// </summary>
        public int Left;
        /// <summary>
        /// Top position of the rectangle.
        /// </summary>
        public int Top;
        /// <summary>
        /// Right position of the rectangle.
        /// </summary>
        public int Right;
        /// <summary>
        /// Bottom position of the rectangle.
        /// </summary>
        public int Bottom;
        
        #endregion

        #region Constructor.
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="left">Horizontal position.</param>
        /// <param name="top">Vertical position.</param>
        /// <param name="right">Right most side.</param>
        /// <param name="bottom">Bottom most side.</param>
        public RECT(int left, int top, int right, int bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        public override string ToString()
        {
            return $"Left: {Left}\n" +
                   $"Right: {Right}\n" +
                   $"Top: {Top}\n" +
                   $"Bottom: {Bottom}\n";

        }

        #endregion
    }

}