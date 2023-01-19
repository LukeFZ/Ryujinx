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
        public ResultCode Status        { get; private set; }
        public byte[]     OutputBuffer  { get; private set; }

        private readonly CancellationTokenSource _tokenSource;
        private readonly CancellationToken       _token;

        private readonly KEvent                  _doneEvent;

        private readonly UserId                  _userId;
        private readonly int                     _timeout;
        private readonly byte                    _method;

        private string                           _path;
        private string                           _postData;

        public AsyncExecution(KEvent doneEvent, FixedParams fixedParams)
        {
            Status = ResultCode.NotStarted;

            _tokenSource = new CancellationTokenSource();
            _token = _tokenSource.Token;

            _doneEvent = doneEvent;

            _userId = fixedParams.UserId;
            _timeout = (int)fixedParams.Timeout.ToSeconds() * 1000;
            _method = fixedParams.Method;
        }

        public void Request()
        {
            Task.Run(() =>
            {
                _tokenSource.CancelAfter(_timeout);

                // NOTE: This is stubbed here since we cannot implement store network calls.
                // Instead, we just immediately signal completion with an empty output buffer.

                Logger.Stub?.PrintStub(LogClass.ServiceNim, new {Path = _path, PostData = _postData, Method = _method, UserId = _userId});

                Status = ResultCode.Success;

                OutputBuffer = Array.Empty<byte>();

                _doneEvent.ReadableEvent.Signal();
            }, _token);
        }

        public void Prepare(string path, string postData)
        {
            _path = path;
            _postData = postData;
        }

        public void Cancel()
        {
            Status = ResultCode.Cancelled;

            _tokenSource.Cancel();

            _doneEvent.ReadableEvent.Signal();
        }
    }
}