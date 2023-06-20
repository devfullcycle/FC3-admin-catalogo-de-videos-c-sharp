using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FC.Codeflix.Catalog.Infra.Messaging.Configuration;
public class ChannelManager
{
    private readonly IConnection _connection;
    private readonly object _lock = new();
    private IModel? _channel = null;
    public ChannelManager(IConnection connection)
    {
        _connection = connection;
    }

    public IModel GetChannel()
    {
        lock (_lock)
        {
            if (_channel == null || _channel.IsClosed)
            {
                _channel = _connection.CreateModel();
                _channel.ConfirmSelect();
            }
            return _channel;
        }
    }
}
