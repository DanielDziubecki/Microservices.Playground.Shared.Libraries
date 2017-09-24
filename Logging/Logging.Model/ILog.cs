using System;

namespace Logging.Model
{
    public interface ILog
    {
        Guid OperationId { get; set; }
        string Protocol { get; set; }

    }
}