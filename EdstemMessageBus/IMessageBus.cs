using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdstemMessageBus
{
    public interface IMessageBus
    {
        Task Publish(BaseMessage message, string topic);
    }
}
