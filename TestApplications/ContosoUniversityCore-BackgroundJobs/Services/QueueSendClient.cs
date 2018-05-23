using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

using Microsoft.Extensions.Configuration;

namespace ContosoUniversityCore.Services
{
    public class QueueSendClient<T> : IDisposable, IQueueSendClient<T>
    {
        private readonly IConfiguration config;

        /// <summary>
        /// The request queue
        /// </summary>
        private CloudQueue requestQueue;

        /// <summary>
        /// The connection name
        /// </summary>
        private const string ConnectionName = "AzureStorage";

        /// <summary>
        /// The queue name
        /// </summary>
        private readonly string queueName;

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="QueueSendClient{T}" /> class.
        /// </summary>
        /// <param name="config">The configuration.</param>
        public QueueSendClient(IConfiguration config)
        {
            this.config = config;
            this.queueName = "queueappb";
            this.InitializeQueue();
        }

        #endregion

        #region Initialize

        /// <summary>
        /// Initializes the queue.
        /// </summary>
        private async void InitializeQueue()
        {
            // Open storage account using credentials from .cscfg file.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(config.GetConnectionString(ConnectionName));

            // Get context object for working with queues, and 
            // set a default retry policy appropriate for a web user interface.
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            //queueClient.DefaultRequestOptions.RetryPolicy = new LinearRetry(TimeSpan.FromSeconds(3), 3);

            // Get a reference to the queue.
            this.requestQueue = queueClient.GetQueueReference(this.queueName);

            //create message queue if not exists
            await this.requestQueue.CreateIfNotExistsAsync();
        }

        #endregion

        /// <summary>
        /// Enqueues the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void EnqueueMessage(T message)
        {
            var queueMessage = new CloudQueueMessage(JsonConvert.SerializeObject(message));
            this.requestQueue.AddMessageAsync(queueMessage);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            requestQueue = null;
        }
    }
}