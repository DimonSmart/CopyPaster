using System.Runtime.InteropServices;
using System.Text;

namespace KeyStrokeSender
{
    class ClipboardHelper
    {
        [DllImport("user32.dll")]
        private static extern bool OpenClipboard(IntPtr hWndNewOwner);

        [DllImport("user32.dll")]
        private static extern bool CloseClipboard();

        [DllImport("user32.dll")]
        private static extern IntPtr GetClipboardData(uint uFormat);

        [DllImport("user32.dll")]
        private static extern bool IsClipboardFormatAvailable(uint format);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GlobalLock(IntPtr hMem);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool GlobalUnlock(IntPtr hMem);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GlobalSize(IntPtr hMem);

        private const uint CF_UNICODETEXT = 13;

        public static string GetClipboardText()
        {
            if (!IsClipboardFormatAvailable(CF_UNICODETEXT))
                return null;

            if (!OpenClipboard(IntPtr.Zero))
                return null;

            string clipboardData = null;
            IntPtr hGlobal = GetClipboardData(CF_UNICODETEXT);
            if (hGlobal != IntPtr.Zero)
            {
                IntPtr lpwcstr = GlobalLock(hGlobal);
                if (lpwcstr != IntPtr.Zero)
                {
                    IntPtr size = GlobalSize(hGlobal);
                    if (size != IntPtr.Zero)
                    {
                        var bytes = new byte[(int)size];
                        Marshal.Copy(lpwcstr, bytes, 0, (int)size);
                        clipboardData = Encoding.Unicode.GetString(bytes).TrimEnd('\0');
                    }
                    GlobalUnlock(lpwcstr);
                }
            }
            CloseClipboard();
            return clipboardData;
        }
    }
}
