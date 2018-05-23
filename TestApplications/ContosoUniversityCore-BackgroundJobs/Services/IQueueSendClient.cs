using System;
using System.Linq;
using System.Threading.Tasks;

namespace ContosoUniversityCore.Services
{
    public interface IQueueSendClient<in T>
    {
        /// <summary>
        /// Enqueues the message.
        /// </summary>
        /// <param name="message">The message.</param>
        Task EnqueueMessageAsync(T message);
    }
}