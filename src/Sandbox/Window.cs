using System.Runtime.InteropServices;
using Titan.Core.Logging;
using Titan.Windows;
using Titan.Windows.Win32;
using static Titan.Windows.Win32.User32;
using static Titan.Windows.Win32.WindowsMessage;
namespace Sandbox;
internal class Window
{
    public HWND Handle { get; }
    public unsafe Window(string title, int width, int height)
    {
        var lpszClassName = "dx12window";
        var wndClassExA = new WNDCLASSEXA
        {
            CbClsExtra = 0,
            CbSize = (uint)Marshal.SizeOf<WNDCLASSEXA>(),
            HCursor = default,
            HIcon = 0,
            LpFnWndProc = &WndProc,
            CbWndExtra = 0,
            HIconSm = 0,
            HInstance = Marshal.GetHINSTANCE(typeof(Window).Module),
            HbrBackground = 0,
            LpszClassName = lpszClassName,
            Style = 0
        };

        

        if (RegisterClassExA(wndClassExA) == 0)
        {
            Logger.Error($"RegisterClassExA failed with Win32Error {Marshal.GetLastWin32Error()}", typeof(Window));
            throw new Exception("Failed to register class");
        }

        // Adjust the window size to take into account for the menu etc
        const WindowStyles wsStyle = WindowStyles.OverlappedWindow | WindowStyles.Visible;
        const int windowOffset = 100;
        var windowRect = new RECT
        {
            Left = windowOffset,
            Top = windowOffset,
            Right = (int)(width + windowOffset),
            Bottom = (int)(height + windowOffset)
        };
        AdjustWindowRect(ref windowRect, wsStyle, false);

        Logger.Trace($"Create window with size Width: {width} Height: {height}", typeof(Window));

        // Create the Window
        Handle = CreateWindowExA(
            0,
            lpszClassName,
            title,
            wsStyle,
            -1,
            -1,
            windowRect.Right - windowRect.Left,
            windowRect.Bottom - windowRect.Top,
            0,
            0,
            wndClassExA.HInstance,
            null
        );

        if (!Handle.IsValid)
        {
            Logger.Error($"CreateWindowExA failed with Win32Error {Marshal.GetLastWin32Error()}");
            throw new Exception("Failed to create window");
        }

        ShowWindow(Handle, ShowWindowCommand.Show);
    }

    public bool Update()
    {
        while (PeekMessageA(out var msg, 0, 0, 0, 1)) // pass IntPtr.Zero as HWND to detect mouse movement outside of the window
        {
            if (msg.Message == WM_QUIT)
            {
                SetWindowLongPtrA(Handle, GWLP_USERDATA, 0);
                return false;
            }
            TranslateMessage(msg);
            DispatchMessage(msg);
        }
        return true;
    }

    [UnmanagedCallersOnly]
    private static nint WndProc(HWND hWnd, WindowsMessage message, nuint wParam, nuint lParam)
    {
        switch (message)
        {
            case WM_KILLFOCUS:
                break;
            case WM_SETFOCUS:
                break;
            case WM_KEYDOWN:
            case WM_SYSKEYDOWN:
                break;
            case WM_KEYUP:
            case WM_SYSKEYUP:
                break;
            case WM_CHAR:
                break;
            case WM_SIZE:
                {
                    var width = (uint)(lParam & 0xffff);
                    var height = (uint)((lParam >> 16) & 0xffff);

                    //Height = height;
                    //Width = width;
                    //_center = new POINT((int)(Width / 2), (int)(Height / 2));
                }
                break;
            case WM_EXITSIZEMOVE:
                break;
            case WM_LBUTTONDOWN:
                break;
            case WM_LBUTTONUP:
                break;
            case WM_RBUTTONDOWN:
                break;
            case WM_RBUTTONUP:
                break;
            case WM_MOUSELEAVE:
                break;
            case WM_MOUSEWHEEL:
                break;
            case WM_CREATE:
                break;
            case WM_CLOSE:
                PostQuitMessage(0);
                return 0;
        }

        return DefWindowProcA(hWnd, message, wParam, lParam);
    }
}
