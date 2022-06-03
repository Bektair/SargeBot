using SC2APIProtocol;
using ResponseType = SC2APIProtocol.Response.ResponseOneofCase;
using RequestType = SC2APIProtocol.Request.RequestOneofCase;

namespace SC2ClientApi;

/// <summary>
///     Handles responses from websockets sequentially by type
///     ResponseOneofCase and RequestOneofCase are equal
/// </summary>
public class ResponseHandler
{
    private readonly IDictionary<ResponseType, Action<Response>> _handlers;

    public ResponseHandler()
    {
        _handlers = new Dictionary<ResponseType, Action<Response>>();
    }

    public void RegisterHandler(RequestType key, Action<Response> handler) => RegisterHandler((ResponseType) key, handler);

    public void RegisterHandler(ResponseType key, Action<Response> handler)
    {
        _handlers[key] = handler;
    }

    public void DeregisterHandler(RequestType key) => DeregisterHandler((ResponseType) key);

    public void DeregisterHandler(ResponseType key)
    {
        _handlers.Remove(key);
    }

    public void Handle(RequestType key, Response response) => Handle((ResponseType) key, response);

    public void Handle(ResponseType key, Response response)
    {
        // if (!_handlers.ContainsKey(key)) return;

        _handlers[key](response);
    }
}