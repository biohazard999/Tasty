﻿using System;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;

using Xenial.Delicious.Remote;

namespace Xenial.Delicious.Transports
{
    public static class InMemoryServerTranport
    {
        public static Task<TransportStreamFactory> CreateInMemoryTransportStream(Uri connectionString, CancellationToken token = default)
        {
            _ = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            var connectionId = connectionString.Segments[1];

            TransportStreamFactory functor = () => CreateStream(connectionId, token);
            return Task.FromResult(functor);
        }

        private static Task<Stream> CreateStream(string connectionId, CancellationToken token = default)
        {
            _ = connectionId ?? throw new ArgumentNullException(nameof(connectionId));

            token.ThrowIfCancellationRequested();

            var (_, serverStream) = InMemoryTransport.GetStream(connectionId);
            return Task.FromResult(serverStream);
        }
    }
}
