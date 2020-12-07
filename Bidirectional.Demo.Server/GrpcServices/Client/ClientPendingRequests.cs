using System;
using System.Collections.Concurrent;

namespace Bidirectional.Demo.Server.GrpcServices.Client
{
    public interface IClientPendingRequests
    {
        void Write(PendingClientRequest pendingClientRequest);
        bool TryRead(string requestId, out PendingClientRequest? pendingClientRequest);
    }
    
    public class ClientPendingRequests : IClientPendingRequests
    {
        private readonly ConcurrentDictionary<string, PendingClientRequest> _pendingRequests;

        public ClientPendingRequests()
        {
            _pendingRequests = new ConcurrentDictionary<string, PendingClientRequest>();
        }

        public void Write(PendingClientRequest pendingClientRequest)
        {
            if (pendingClientRequest == null) throw new ArgumentNullException(nameof(pendingClientRequest));

            if (!_pendingRequests.TryAdd(pendingClientRequest.Request.MetaData.RequestId, pendingClientRequest))
                throw new InvalidOperationException("Another request with the same request ID is already pending: " + pendingClientRequest.Request.MetaData.RequestId);
        }

        public bool TryRead(string requestId, out PendingClientRequest? pendingClientRequest)
        {
            return _pendingRequests.TryGetValue(requestId, out pendingClientRequest);
        }
    }
}