using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace ContosoUniversityFull.Services
{
    public class QueueSendClient<T> : IDisposable, IQueueSendClient<T>
    {
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
        public QueueSendClient()
        {
            this.queueName = "queueappa";
            this.InitializeQueue();
        }

        #endregion

        #region Initialize

        /// <summary>
        /// Initializes the queue.
        /// </summary>
        private void InitializeQueue()
        {
            // Open storage account using credentials from .cscfg file.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings[ConnectionName].ToString());

            // Get context object for working with queues, and 
            // set a default retry policy appropriate for a web user interface.
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            //queueClient.DefaultRequestOptions.RetryPolicy = new LinearRetry(TimeSpan.FromSeconds(3), 3);

            // Get a reference to the queue.
            this.requestQueue = queueClient.GetQueueReference(this.queueName);

            //create message queue if not exists
            this.requestQueue.CreateIfNotExists();
        }

        #endregion

        /// <summary>
        /// Enqueues the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public async Task EnqueueMessageAsync(T message)
        {
            var queueMessage = new CloudQueueMessage(JsonConvert.SerializeObject(message));
            await this.requestQueue.AddMessageAsync(queueMessage);
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