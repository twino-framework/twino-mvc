using System.Threading.Tasks;
using Horse.Core;
using Horse.Core.Protocols;

namespace Horse.Protocols.Http
{
    /// <summary>
    /// Http Request handler
    /// </summary>
    public delegate Task HttpRequestHandler(HttpRequest request, HttpResponse response);

    /// <summary>
    /// Http Protocol handler for accepting HTTP Requests via HttpRequestHandler action  
    /// </summary>
    internal class HttpMethodHandler : IProtocolConnectionHandler<SocketBase, HttpMessage>
    {
        /// <summary>
        /// User defined action
        /// </summary>
        private readonly HttpRequestHandler _action;

        public HttpMethodHandler(HttpRequestHandler action)
        {
            _action = action;
        }

        /// <summary>
        /// Unused method, HTTP connections are not piped and this method isn't called
        /// </summary>
        public async Task<SocketBase> Connected(IHorseServer server, IConnectionInfo connection, ConnectionData data)
        {
            return await Task.FromResult((SocketBase)null);
        }

        /// <summary>
        /// Triggered when handshake is completed and the connection is ready to communicate 
        /// </summary>
        public async Task Ready(IHorseServer server, SocketBase client)
        {
            await Task.CompletedTask;
        }

        /// <summary>
        /// Triggered when a client sends a message to the server 
        /// </summary>
        public async Task Received(IHorseServer server, IConnectionInfo info, SocketBase client, HttpMessage message)
        {
            await _action(message.Request, message.Response);
        }

        /// <summary>
        /// Unused method, HTTP connections are not piped and this method isn't called
        /// </summary>
        public async Task Disconnected(IHorseServer server, SocketBase client)
        {
            await Task.CompletedTask;
        }
    }
}