using System;
using System.Runtime.InteropServices;
using System.Threading;
using WindowsInput;

namespace KeyStrokeSender
{
    class Program
    {
        // Constants for hotkey modifiers
        private const int MOD_ALT = 0x1;
        private const int MOD_CONTROL = 0x2;
        private const int MOD_SHIFT = 0x4;
        private const int MOD_WIN = 0x8;

        // Constant for the WM_HOTKEY message
        private const int WM_HOTKEY = 0x0312;

        // DllImport for user32.dll functions
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

      

        [DllImport("user32.dll")]
        private static extern IntPtr GetMessage(out MSG lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax);

        [DllImport("user32.dll")]
        private static extern bool TranslateMessage([In] ref MSG lpMsg);

        [DllImport("user32.dll")]
        private static extern IntPtr DispatchMessage([In] ref MSG lpMsg);

        [StructLayout(LayoutKind.Sequential)]
        private struct MSG
        {
            public IntPtr hwnd;
            public uint message;
            public IntPtr wParam;
            public IntPtr lParam;
            public uint time;
            public POINT pt;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct INPUT
        {
            public uint type;
            public InputUnion u;
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct InputUnion
        {
            [FieldOffset(0)]
            public KEYBDINPUT ki;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct KEYBDINPUT
        {
            public ushort wVk;
            public ushort wScan;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        const uint INPUT_KEYBOARD = 1;
        const uint KEYEVENTF_KEYUP = 0x0002;

        static void Main(string[] args)
        {
            // Register the hotkey (Ctrl+Shift+Insert)
            if (!RegisterHotKey(IntPtr.Zero, 1, MOD_CONTROL | MOD_SHIFT, (int)ConsoleKey.Insert))
            {
                Console.WriteLine("Failed to register hotkey.");
                return;
            }

            Console.WriteLine("Hotkey registered. Press Ctrl+Shift+Insert to paste text from clipboard as keystrokes.");

            // Message loop to capture the hotkey
            MSG msg;
            while (GetMessage(out msg, IntPtr.Zero, 0, 0) != IntPtr.Zero)
            {
                if (msg.message == WM_HOTKEY)
                {
                    Console.WriteLine("Hotkey pressed.");

                    OnHotKeyPressed();
                }
                TranslateMessage(ref msg);
                DispatchMessage(ref msg);
            }

            // Unregister the hotkey when the application exits
            UnregisterHotKey(IntPtr.Zero, 1);
        }

        private static void OnHotKeyPressed()
        {
            // Get the text from the clipboard using the ClipboardHelper
            string clipboardText = ClipboardHelper.GetClipboardText();

            if (!string.IsNullOrEmpty(clipboardText))
            {
                var simulator = new InputSimulator();

                // Send the text as keystrokes
                foreach (char c in clipboardText)
                {
                    // Simulate the key press
                    simulator.Keyboard.TextEntry(c);
                }
            }
        }


      

    }
}
