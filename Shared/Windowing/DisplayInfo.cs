// Copyright (c) Microsoft Corporation. All rights reserved.

using System.Runtime.InteropServices;
using Microsoft.UI.Xaml;
using Windows.Foundation;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Gdi;
using Windows.Win32.UI.HiDpi;
using WinRT.Interop;

namespace Shared
{
    public class DisplayInfo
    {
        private readonly HMONITOR monitor;
        private readonly MONITORINFO monitorInfo;

        public static DisplayInfo GetDisplayInfoForElement(UIElement element)
        {
            var window = WindowHelper.GetWindowForElement(element);
            if (window == null)
            {
                return null;
            }

            var hwnd = (HWND)WindowNative.GetWindowHandle(window);
            var hMonitor = PInvoke.MonitorFromWindow(hwnd, MONITOR_FROM_FLAGS.MONITOR_DEFAULTTONEAREST);
            MONITORINFO mi = default(MONITORINFO);
            mi.cbSize = (uint)Marshal.SizeOf(mi);
            bool success = PInvoke.GetMonitorInfo(hMonitor, ref mi);
            if (!success)
            {
                return null;
            }

            return new DisplayInfo(hMonitor, mi);
        }

        private DisplayInfo(HMONITOR monitor, MONITORINFO monitorInfo)
        {
            this.monitor = monitor;
            this.monitorInfo = monitorInfo;

            this.ScreenWidth = monitorInfo.rcMonitor.right - monitorInfo.rcMonitor.left;
            this.ScreenHeight = monitorInfo.rcMonitor.bottom - monitorInfo.rcMonitor.top;
            this.WorkArea = new Rect(
                monitorInfo.rcWork.left,
                monitorInfo.rcWork.top,
                monitorInfo.rcWork.right - monitorInfo.rcWork.left,
                monitorInfo.rcWork.bottom - monitorInfo.rcWork.top);
        }

        public int ScreenHeight { get; private set; }

        public int ScreenWidth { get; private set; }

        public int ScreenEffectiveHeight
        {
            get
            {
                _ = PInvoke.GetDpiForMonitor(
                    monitor,
                    MONITOR_DPI_TYPE.MDT_EFFECTIVE_DPI,
                    out uint widthDPI,
                    out _);
                float scalingFactor = (float)widthDPI / 96;
                return (int)(ScreenHeight / scalingFactor);
            }
        }

        public int ScreenEffectiveWidth
        {
            get
            {
                _ = PInvoke.GetDpiForMonitor(
                    monitor,
                    MONITOR_DPI_TYPE.MDT_EFFECTIVE_DPI,
                    out uint _,
                    out uint heightDPI);
                float scalingFactor = (float)heightDPI / 96;
                return (int)(ScreenWidth / scalingFactor);
            }
        }

        public Rect WorkArea { get; private set; }
    }
}
