﻿using Ryujinx.Common.Logging;
using Ryujinx.HLE.HOS.Services.Nim.AsyncContext;
using Ryujinx.Horizon.Common;
using System.Text;

namespace Ryujinx.HLE.HOS.Services.Nim.ShopServiceAccessServerInterface.ShopServiceAccessServer.ShopServiceAccessor
{
    class IShopServiceAsync : IpcService
    {
        private readonly AsyncExecution _asyncExecution;

        public IShopServiceAsync(AsyncExecution asyncExecution)
        {
           _asyncExecution = asyncExecution;
        }

        [CommandHipc(0)]
        // Cancel()
        public ResultCode Cancel(ServiceCtx context)
        {
            if (_asyncExecution.Status == ResultCode.NotStarted && _asyncExecution.OutputBuffer.Length == 0)
            {
                _asyncExecution.Cancel();
            }

            return ResultCode.Success;
        }


        [CommandHipc(1)]
        // GetSize() -> u64
        public ResultCode GetSize(ServiceCtx context)
        {
            if (_asyncExecution.Status != ResultCode.Success)
            {
                return _asyncExecution.Status;
            }

            ulong size = (ulong)_asyncExecution.OutputBuffer.Length;
            context.ResponseData.Write(size);

            return ResultCode.Success;
        }


        [CommandHipc(2)]
        // Read() -> (u64, buffer<bytes, 6>)
        public ResultCode Read(ServiceCtx context)
        {
            if (_asyncExecution.Status != ResultCode.Success)
            {
                return _asyncExecution.Status;
            }

            ulong bufferPosition = context.Request.ReceiveBuff[0].Position;
            ulong bufferSize = context.Request.ReceiveBuff[0].Size;

            ulong  size  = (ulong)_asyncExecution.OutputBuffer.Length;
            byte[] data = _asyncExecution.OutputBuffer;

            context.Memory.Write(bufferPosition, data);
            context.ResponseData.Write(size);

            return ResultCode.Success;
        }


        [CommandHipc(3)]
        // GetErrorCode() -> ErrorCode
        public ResultCode GetErrorCode(ServiceCtx context)
        {
            if (_asyncExecution.Status != ResultCode.Success)
            {
                return _asyncExecution.Status;
            }

            context.ResponseData.Write(0);

            return ResultCode.Success;
        }


        [CommandHipc(4)]
        // Request()
        public ResultCode Request(ServiceCtx context)
        {
            _asyncExecution.Request();

            return ResultCode.Success;
        }

        [CommandHipc(5)]
        // Prepare(buffer<string, 5>, buffer<string, 5>)
        public ResultCode Prepare(ServiceCtx context)
        {
            ulong pathPosition     = context.Request.SendBuff[0].Position;
            ulong pathSize         = context.Request.SendBuff[0].Size;
            ulong postDataPosition = context.Request.SendBuff[1].Position;
            ulong postDataSize     = context.Request.SendBuff[1].Size;

            string path     = "";
            string postData = "";

            byte[] pathBuffer = new byte[pathSize];
            context.Memory.Read(pathPosition, pathBuffer);
            path = Encoding.ASCII.GetString(pathBuffer);

            if (postDataSize != 0)
            {
                byte[] postDataBuffer = new byte[postDataSize];
                context.Memory.Read(postDataPosition, postDataBuffer);
                postData = Encoding.ASCII.GetString(postDataBuffer);
            }

            _asyncExecution.Prepare(path, postData);

            return ResultCode.Success;
        }
    }
}