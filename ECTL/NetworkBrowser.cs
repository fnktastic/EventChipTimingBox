namespace ECTL
{
    using System;
    using System.Collections;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Windows.Forms;

    public sealed class NetworkBrowser
    {
        public ArrayList getNetworkComputers()
        {
            ArrayList list = new ArrayList();
            int num = 1;
            int num2 = 2;
            IntPtr zero = IntPtr.Zero;
            IntPtr ptr = IntPtr.Zero;
            int dwEntriesRead = 0;
            int dwTotalEntries = 0;
            int dwResumeHandle = 0;
            int num6 = Marshal.SizeOf(typeof(_SERVER_INFO_100));
            try
            {
                if (NetServerEnum(null, 100, ref zero, -1, out dwEntriesRead, out dwTotalEntries, num | num2, null, out dwResumeHandle) == 0)
                {
                    for (int i = 0; i < dwTotalEntries; i++)
                    {
                        ptr = new IntPtr(((int) zero) + (i * num6));
                        _SERVER_INFO_100 _server_info_ = (_SERVER_INFO_100) Marshal.PtrToStructure(ptr, typeof(_SERVER_INFO_100));
                        list.Add(_server_info_.sv100_name);
                    }
                }
                return list;
            }
            catch (Exception exception)
            {
                MessageBox.Show("Problem with acessing network computers in NetworkBrowser \r\n\r\n\r\n" + exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return null;
            }
            finally
            {
                NetApiBufferFree(zero);
            }
            return list;
        }

        [SuppressUnmanagedCodeSecurity, DllImport("Netapi32", SetLastError=true)]
        public static extern int NetApiBufferFree(IntPtr pBuf);
        [SuppressUnmanagedCodeSecurity, DllImport("Netapi32", CharSet=CharSet.Auto, SetLastError=true)]
        public static extern int NetServerEnum(string ServerNane, int dwLevel, ref IntPtr pBuf, int dwPrefMaxLen, out int dwEntriesRead, out int dwTotalEntries, int dwServerType, string domain, out int dwResumeHandle);

        [StructLayout(LayoutKind.Sequential)]
        public struct _SERVER_INFO_100
        {
            internal int sv100_platform_id;
            [MarshalAs(UnmanagedType.LPWStr)]
            internal string sv100_name;
        }
    }
}

