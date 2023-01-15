using Ryujinx.Common;
using Ryujinx.Common.Logging;
using Ryujinx.HLE.HOS.Ipc;
using Ryujinx.HLE.HOS.Kernel.Threading;
using Ryujinx.HLE.HOS.Services.Nim.AsyncContext;
using Ryujinx.HLE.HOS.Services.Nim.ShopServiceAccessServerInterface.ShopServiceAccessServer.ShopServiceAccessor;
using Ryujinx.HLE.HOS.Services.Nim.Types;
using Ryujinx.Horizon.Common;
using System;

namespace Ryujinx.HLE.HOS.Services.Nim.ShopServiceAccessServerInterface.ShopServiceAccessServer
{
    class IShopServiceAccessor : IpcService
    {
        public IShopServiceAccessor() { }

        [CommandHipc(0)]
        // CreateAsyncInterface(nn::nim::ShopServiceAccessTypes::FixedParams) -> (handle<copy>, object<nn::ec::IShopServiceAsync>)
        public ResultCode CreateAsyncInterface(ServiceCtx context)
        {
            FixedParams    fixedParams       = context.RequestData.ReadStruct<FixedParams>();
            KEvent         asyncDoneEvent    = new KEvent(context.Device.System.KernelContext);
            AsyncExecution asyncExecution    = new AsyncExecution(asyncDoneEvent, fixedParams);

            if (context.Process.HandleTable.GenerateHandle(asyncDoneEvent.ReadableEvent, out int asyncDoneEventHandle) != Result.Success)
            {
                throw new InvalidOperationException("Out of handles!");
            }

            MakeObject(context, new IShopServiceAsync(asyncExecution));

            context.Response.HandleDesc = new IpcHandleDesc(new int[] { asyncDoneEventHandle }, context.Response.HandleDesc.ToMove);

            Logger.Stub?.PrintStub(LogClass.ServiceNim, new { fixedParams.UserId, asyncExecution.Timeout, fixedParams.Method});

            return ResultCode.Success;
        }
    }
}