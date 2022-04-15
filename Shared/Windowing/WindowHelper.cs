// Copyright (c) Microsoft Corporation. All rights reserved.

using System;
using System.Collections.Generic;
using Microsoft.UI.Xaml;

namespace Shared
{
    /// <summary>
    /// Class to be used to find the window associated with an element as long as
    /// WindowHelper.CreateWindow was used to create the window.
    /// </summary>
    public static class WindowHelper
    {
        private static List<Window> activeWindows = new List<Window>();
        private static List<Window> windows = new List<Window>();

        public static event EventHandler<EventArgs> ActiveWindowsChanged;

        public static event EventHandler<EventArgs> WindowsChanged;

        public static IReadOnlyList<Window> ActiveWindows
        {
            get { return activeWindows; }
        }

        public static IReadOnlyList<Window> Windows
        {
            get { return windows; }
        }

        public static Window CreateWindow()
        {
            Window newWindow = new Window();
            TrackWindow(newWindow);
            return newWindow;
        }

        public static void TrackWindow(Window window)
        {
            window.Activated += WindowActivated;
            window.Closed += WindowClosed;

            windows.Add(window);
            WindowsChanged?.Invoke(null, EventArgs.Empty);

            activeWindows.Add(window);
            ActiveWindowsChanged?.Invoke(null, EventArgs.Empty);
        }

        private static void WindowActivated(object sender, WindowActivatedEventArgs args)
        {
            if (sender is Window window)
            {
                if (args.WindowActivationState == WindowActivationState.Deactivated)
                {
                    activeWindows.Remove(window);
                }
                else
                {
                    if (!activeWindows.Contains(window))
                    {
                        activeWindows.Add(window);
                    }
                }

                ActiveWindowsChanged?.Invoke(null, EventArgs.Empty);
            }
        }

        private static void WindowClosed(object sender, WindowEventArgs args)
        {
            if (sender is Window window)
            {
                window.Activated -= WindowActivated;
                window.Closed -= WindowClosed;

                activeWindows.Remove(window);
                ActiveWindowsChanged?.Invoke(null, EventArgs.Empty);

                windows.Remove(window);
                WindowsChanged?.Invoke(null, EventArgs.Empty);
            }
        }

        public static Window GetWindowForElement(UIElement element)
        {
            if (element.XamlRoot != null)
            {
                foreach (Window window in windows)
                {
                    if (element.XamlRoot == window.Content?.XamlRoot)
                    {
                        return window;
                    }
                }
            }
            return null;
        }
    }
}
