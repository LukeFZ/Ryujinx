using Ryujinx.HLE.HOS.Services.Account.Acc;
using Ryujinx.HLE.HOS.Services.Time.Clock;
using System.Runtime.InteropServices;

namespace Ryujinx.HLE.HOS.Services.Nim.Types
{
    [StructLayout(LayoutKind.Sequential, Pack = 8, Size = 32)]
    struct FixedParams
    {
        public UserId UserId;
        public TimeSpanType Timeout;
        public byte Method;
    }
}