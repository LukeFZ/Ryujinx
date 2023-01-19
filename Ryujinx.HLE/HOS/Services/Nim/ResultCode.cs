namespace Ryujinx.HLE.HOS.Services.Nim
{
    enum ResultCode
    {
        ModuleId       = 137,
        ErrorCodeShift = 9,

        Success = 0,

        AlreadyInitialized           = (10 << ErrorCodeShift)   | ModuleId,
        NotStarted                   = (60 << ErrorCodeShift)   | ModuleId,
        NullArgument                 = (90 << ErrorCodeShift)   | ModuleId,
        Cancelled                    = (1450 << ErrorCodeShift) | ModuleId
    }
}