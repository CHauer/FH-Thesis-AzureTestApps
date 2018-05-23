using ContosoUniversityFull.DAL;
using System;
using System.Linq;

using Unity;
using Unity.Lifetime;

namespace GenerateImageWebJobFull.Infrastructure
{

    /// <summary>
    /// Specifies the Unity configuration for the main container.
    /// </summary>
    // ReSharper disable once ClassNeverInstantiated.Global
    public class UnityConfig
    {
        #region Unity Container

        private static Lazy<IUnityContainer> container = new Lazy<IUnityContainer>(() =>
        {
            var container = new UnityContainer();
            RegisterTypes(container);
            return container;
        });

        /// <summary>
        /// Gets the configured Unity container.
        /// </summary>
        public static IUnityContainer GetConfiguredContainer()
        {
            return container.Value;
        }
        #endregion

        /// <summary>
        /// Registers the type mappings with the Unity container.
        /// </summary>
        /// <param name="containerConfig">The container configuration.</param>
        /// <remarks>
        /// There is no need to register concrete types such as controllers or API controllers (unless you want to
        /// change the defaults), as Unity allows resolving a concrete type even if it was not previously registered.
        /// </remarks>
        // ReSharper disable once MemberCanBePrivate.Global
        public static void RegisterTypes(IUnityContainer containerConfig)
        {
            // NOTE: To load from web.config uncomment the line below. Make sure to add a Microsoft.Practices.Unity.Configuration to the using statements.
            // container.LoadConfiguration();

            // Register project specific types here
            containerConfig.RegisterType<SchoolContext>(new PerResolveLifetimeManager());
        }
    }
}
