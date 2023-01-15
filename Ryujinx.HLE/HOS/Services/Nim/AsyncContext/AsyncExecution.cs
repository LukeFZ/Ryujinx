using Ryujinx.Common.Logging;
using Ryujinx.HLE.HOS.Kernel.Threading;
using Ryujinx.HLE.HOS.Services.Account.Acc;
using Ryujinx.HLE.HOS.Services.Nim.Types;
using Ryujinx.Horizon.Common;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Ryujinx.HLE.HOS.Services.Nim.AsyncContext
{
    class AsyncExecution
    {
        private readonly CancellationTokenSource _tokenSource;
        private readonly CancellationToken       _token;
    
        public KEvent DoneEvent   { get; }
        public bool   IsInitialized { get; private set; }
        public bool   IsRunning     { get; private set; }
        public Result ErrorCode     { get; private set; }
        public byte[] OutputBuffer  { get; private set; }

        public UserId UserId        { get; }
        public int    Timeout       { get; }
        public byte   Method        { get; }
        public string Path          { get; private set; }
        public string PostData      { get; private set; }

        public AsyncExecution(KEvent doneEvent, FixedParams fixedParams)
        {
            DoneEvent = doneEvent;

            _tokenSource = new CancellationTokenSource();
            _token = _tokenSource.Token;

            UserId = fixedParams.UserId;
            Timeout = (int)fixedParams.Timeout.ToSeconds() * 1000;
            Method = fixedParams.Method;
        }

        public void Request()
        {
            Task.Run(() =>
            {
                IsRunning = true;

                _tokenSource.CancelAfter(Timeout);

                // NOTE: This is stubbed here since we cannot implement store network calls.
                // Instead, we just immediately signal completion with an empty output buffer.

                Logger.Stub?.PrintStub(LogClass.ServiceNim);

                OutputBuffer = Array.Empty<byte>();

                DoneEvent.ReadableEvent.Signal();

                IsRunning = false;
            }, _token);
        }

        public void Prepare(string path, string postData)
        {
            Path = path;
            PostData = postData;

            IsInitialized = true;
        }

        public void Cancel()
        {
            _tokenSource.Cancel();
        }
    }
}