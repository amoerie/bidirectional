using System;
using System.Threading;
using Bidirectional.Grpc.Common.Contracts.Client;

namespace Bidirectional.Grpc.Server.GrpcServices.Client
{
    public interface IClientRequestMetaDataFactory
    {
        ClientRequestMetaData CreateNew();
    }
    
    public class ClientRequestMetaDataFactory : IClientRequestMetaDataFactory
    {
        private int _requestId;

        public ClientRequestMetaDataFactory()
        {
            _requestId = 0;
        }
        
        public ClientRequestMetaData CreateNew()
        {
            var requestId = Interlocked.Increment(ref _requestId);
            var timestamp = DateTime.UtcNow;
            return new ClientRequestMetaData($"REQ-{requestId}", timestamp);
        }
    }
}