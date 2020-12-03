using System;
using Bidirectional.Demo.Common.Contracts.Client;

namespace Bidirectional.Demo.Client.GrpcClient
{
    public interface IClientResponseMetaDataFactory
    {
        ClientResponseMetaData CreateNew(ClientRequestMetaData clientRequestMetaData);
    }
    
    public class ClientResponseMetaDataFactory : IClientResponseMetaDataFactory
    {
        public ClientResponseMetaData CreateNew(ClientRequestMetaData clientRequestMetaData)
        {
            var (requestId, requestTimeStamp) = clientRequestMetaData;
            var responseTimeStamp = DateTime.UtcNow;
            return new ClientResponseMetaData(requestId,  requestTimeStamp, responseTimeStamp);
        }
    }
}