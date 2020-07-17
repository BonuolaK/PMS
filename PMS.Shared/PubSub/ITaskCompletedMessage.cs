using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.Shared.PubSub
{
    public interface ITaskCompletedMessage
    {
        Guid MessageId { get; set; }
        int ProjectId { get; set; }
    }

    public class TaskCompletedMessage : ITaskCompletedMessage
    {
        public Guid MessageId { get; set; }
        public int ProjectId { get; set; }
    }
}
