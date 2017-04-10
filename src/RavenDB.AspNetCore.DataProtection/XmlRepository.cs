using Microsoft.AspNetCore.DataProtection.Repositories;
using Raven.Client.Documents;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace RavenDB.AspNetCore.DataProtection
{
    public class XmlRepository
        : IXmlRepository
    {
        private readonly IDocumentStore _store;
        private readonly string _key;

        /// <summary>
        /// Creates a <see cref="XmlRepository"/> with keys stored at the given directory.
        /// </summary>
        /// <param name="database">Database to store</param>
        public XmlRepository(IDocumentStore store, string key)
        {
            _store = store;
            _key = key;
        }

        /// <inheritdoc />
        public IReadOnlyCollection<XElement> GetAllElements()
        {
            using (var session = _store.OpenSession())
            {
                var dataProtection = session.Load<DataProtection>(_key) 
                    ?? new DataProtection();

                return GetAllElementsCore(dataProtection).ToList().AsReadOnly();
            }
        }

        private IEnumerable<XElement> GetAllElementsCore(DataProtection entity)
        {
            // Note: Inability to read any value is considered a fatal error (since the file may contain
            // revocation information), and we'll fail the entire operation rather than return a partial
            // set of elements. If a value contains well-formed XML but its contents are meaningless, we
            // won't fail that operation here. The caller is responsible for failing as appropriate given
            // that scenario.
            foreach (var value in entity.Elements)
            {
                yield return XElement.Parse(value);
            }
        }

        /// <inheritdoc />
        public void StoreElement(XElement element, string friendlyName)
        {
            using (var session = _store.OpenSession())
            {
                session.Advanced.UseOptimisticConcurrency = true;

                var dataProtection = session.Load<DataProtection>(_key);
                if(dataProtection == null)
                {
                    dataProtection = new DataProtection() {
                        Id = _key
                    };

                    session.Store(dataProtection);
                }

                dataProtection.Elements.Add(element.ToString(SaveOptions.DisableFormatting));
                session.SaveChanges();
            }
        }
    }
}