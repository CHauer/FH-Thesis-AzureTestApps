using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Azure.WebJobs.Host;
using Unity;

namespace GenerateImageWebJobFull.Infrastructure
{
    public class UnityActivator : IJobActivator
    {
        /// <summary>
        /// The _container
        /// </summary>
        private readonly IUnityContainer _container;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnityActivator"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        public UnityActivator(IUnityContainer container)
        {
            _container = container;
        }

        /// <summary>
        /// Creates a new instance of a job type.
        /// </summary>
        /// <typeparam name="T">The job type.</typeparam>
        /// <returns>
        /// A new instance of the job type.
        /// </returns>
        public T CreateInstance<T>()
        {
            return _container.Resolve<T>();
        }
    }
}
