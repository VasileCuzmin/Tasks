using System;

namespace Tasks.Api.Models
{
    public class AsyncCommandResult
    {
        public Guid CommandId { get; }
        public Guid? CorrelationId { get; }

        public AsyncCommandResult(Guid? correlationId)
        {
            CommandId = Guid.NewGuid();
            CorrelationId = correlationId;
        }
    }
}
