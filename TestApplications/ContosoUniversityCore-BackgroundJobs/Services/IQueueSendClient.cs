using System;
using System.Linq;

namespace ContosoUniversityCore.Services
{
    public interface IQueueSendClient<in T>
    {
        /// <summary>
        /// Enqueues the message.
        /// </summary>
        /// <param name="message">The message.</param>
        void EnqueueMessage(T message);
    }
}