using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.Shared.PubSub
{
    public interface IProducerClient<T>
    {
        bool Produce(string topic, T message);
    }
}
