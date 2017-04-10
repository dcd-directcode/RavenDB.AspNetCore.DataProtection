using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.Repositories;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Raven.Client.Documents;
using System;

namespace RavenDB.AspNetCore.DataProtection
{
    /// <summary>
    /// Contains Redis-specific extension methods for modifying a <see cref="IDataProtectionBuilder"/>.
    /// </summary>
    public static class DataProtectionBuilderExtensions
    {
        private const string DataProtectionKeysName = "DataProtection-Keys";

        /// <summary>
        /// Configures the data protection system to persist keys to specified key in Redis database
        /// </summary>
        /// <param name="builder">The builder instance to modify.</param>
        /// <param name="databaseFactory">The delegate used to create <see cref="IDatabase"/> instances.</param>
        /// <param name="key">The <see cref="RedisKey"/> used to store key list.</param>
        /// <returns>A reference to the <see cref="IDataProtectionBuilder" /> after this operation has completed.</returns>
        public static IDataProtectionBuilder PersistKeysToRavenDB(this IDataProtectionBuilder builder, IDocumentStore store, string key)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }
            if (store == null)
            {
                throw new ArgumentNullException(nameof(store));
            }
            return PersistKeysToRavenDBInternal(builder, store, key);
        }

        private static IDataProtectionBuilder PersistKeysToRavenDBInternal(IDataProtectionBuilder config, IDocumentStore store, string key)
        {
            config.Services.TryAddSingleton<IXmlRepository>(services => new XmlRepository(store, key));
            return config;
        }
    }
}