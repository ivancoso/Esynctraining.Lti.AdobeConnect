namespace Esynctraining.Core.Business.Models
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Web;

    using Castle.Core.Logging;

    using Esynctraining.Core.Domain.Entities;
    using Esynctraining.Core.Extensions;
    using Esynctraining.Core.FullText;
    using Esynctraining.Core.Providers;
    using Esynctraining.Core.Utils;


    using Lucene.Net.Analysis.Standard;
    using Lucene.Net.Documents;
    using Lucene.Net.Index;
    using Lucene.Net.QueryParsers;
    using Lucene.Net.Search;
    using Lucene.Net.Store;

    using NHibernate.Util;

    using Version = Lucene.Net.Util.Version;

    /// <summary>
    /// The full text model.
    /// </summary>
    public class FullTextModel
    {
        #region Static Fields

        /// <summary>
        /// The _ full search directory.
        /// </summary>
        private static FSDirectory fullSearchDirectory;

        /// <summary>
        /// The index path.
        /// </summary>
        private static string indexPath;

        #endregion

        #region Fields

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        /// The locker.
        /// </summary>
        private readonly static object locker = new object();

        /// <summary>
        /// The settings.
        /// </summary>
        private readonly dynamic settings;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FullTextModel"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="settings">
        /// The settings.
        /// </param>
        public FullTextModel(ILogger logger, ApplicationSettingsProvider settings)
        {
            this.logger = logger;
            this.settings = settings;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the full search directory.
        /// </summary>
        public FSDirectory FullSearchDirectory
        {
            get
            {
                if (fullSearchDirectory == null)
                {
                    lock (locker)
                    {
                        if (fullSearchDirectory == null)
                        {
                            // this is collected in my configuration settings.
                            var indexLocation = (string)this.settings.FullText_IndexLocation;
                            if (HttpContext.Current != null && indexLocation.StartsWith("~"))
                            {
                                indexLocation = HttpContext.Current.Server.MapPath(indexLocation);
                            }

                            indexPath = indexLocation;

                            fullSearchDirectory = FSDirectory.Open(new DirectoryInfo(indexPath));

                            if (IndexWriter.IsLocked(fullSearchDirectory))
                            {
                                IndexWriter.Unlock(fullSearchDirectory);
                            }

                            string lockFilePath = Path.Combine(indexPath, "write.lock");
                            if (File.Exists(lockFilePath))
                            {
                                File.Delete(lockFilePath);
                            }
                        }
                    }
                }

                return fullSearchDirectory;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Determines if the type has Full-Text index properties.
        /// </summary>
        /// <param name="type">
        /// Type of an object
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool IsIndexable(Type type)
        {
            return FullTextEnabled(type) && type.GetProperties().SelectMany(x => x.GetCustomAttributes(typeof(FullTextIndexedAttribute), true)).Any();
        }

        /// <summary>
        /// Determines if the object has Full-Text index properties.
        /// </summary>
        /// <param name="obj">
        /// Object instance
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool IsIndexable(object obj)
        {
            return IsIndexable(obj.GetType());
        }

        /// <summary>
        ///     Removes all indices from the Full Text index; used for regular maintenance.
        /// </summary>
        public void ClearIndices()
        {
            using (var analyzer = new StandardAnalyzer(Version.LUCENE_30))
            using (var writer = new IndexWriter(this.FullSearchDirectory, analyzer, true, IndexWriter.MaxFieldLength.UNLIMITED))
            {
                writer.DeleteAll();
            }
        }

        /// <summary>
        /// Removes the index of the supplied entity.
        /// </summary>
        /// <param name="entity">
        /// Entity object
        /// </param>
        public void DeleteIndexForEntity(object entity)
        {
            if (!FullTextEnabled(entity))
            {
                return;
            }

            List<PropertyInfo> indexables = this.GetIndexableProperties(entity);
            if (indexables.Count == 0)
            {
                return;
            }

            Type entityType = entity.GetType();
            string entityName = entityType.Name;
            string entityIdName = string.Format("{0}Id", entityName);
            string entityIdValue = entityType.GetProperty(Lambda.Property<Entity>(x => x.Id)).GetValue(entity, null).ToString();

//            this.logger.DebugFormat("Deleting FT Index for {0} {1}...", entityIdName, entityIdValue);
            var searchQuery = new TermQuery(new Term(entityIdName, entityIdValue));
            using (var analyzer = new StandardAnalyzer(Version.LUCENE_30))
            using (
                var writer = new IndexWriter(this.FullSearchDirectory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED))
            {
                writer.DeleteDocuments(searchQuery);
//                this.logger.DebugFormat("Deleted the FT index for {0} {1}", entityIdName, entityIdValue);
            }
        }

        /// <summary>
        /// Collects the list of PropertyInfo objects decorated with the
        ///     FullTextIndexedAttribute class for the supplied object.
        /// </summary>
        /// <param name="obj">
        /// Object instance
        /// </param>
        /// <returns>
        /// The <see cref="List{PropertyInfo}"/>.
        /// </returns>
        public List<PropertyInfo> GetIndexableProperties(object obj)
        {
            return this.GetIndexableProperties(obj.GetType()).ToList();
        }

        /// <summary>
        /// The full text enabled.
        /// </summary>
        /// <param name="obj">
        /// The type.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool FullTextEnabled(object obj)
        {
            return FullTextEnabled(obj.GetType());
        }

        /// <summary>
        /// The full text enabled.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool FullTextEnabled(Type type)
        {
            return (typeof(Entity).IsAssignableFrom(type) || typeof(EntityGuid).IsAssignableFrom(type)) && type.GetCustomAttributes(typeof(FullTextEnabledAttribute), true).Any();
        }

        /// <summary>
        /// Collects the list of PropertyInfo objects decorated with the
        ///     FullTextIndexedAttribute class for the supplied type.
        /// </summary>
        /// <param name="type">
        /// Type to check
        /// </param>
        /// <returns>
        /// The <see cref="List{PropertyInfo}"/>.
        /// </returns>
        public List<PropertyInfo> GetIndexableProperties(Type type)
        {
            IEnumerable<PropertyInfo> props =
                type.GetProperties()
                    .Where(x => x.GetCustomAttributes(true).OfType<FullTextIndexedAttribute>().Any())
                    .Select(
                        x =>
                        new
                            {
                                PropInfo = x,
                                Priority = x.GetCustomAttributes(true).OfType<FullTextIndexedAttribute>().First().IndexPriority
                            })
                    .OrderBy(x => x.Priority)
                    .Select(x => x.PropInfo);

            return props.ToList();
        }

        /// <summary>
        /// Creates a new index for the supplied entity.
        /// </summary>
        /// <param name="entity">
        /// Entity object
        /// </param>
        public void InsertIndexOnEntity(object entity)
        {
            if (!FullTextEnabled(entity))
            {
                return;
            }

            List<PropertyInfo> indexables = this.GetIndexableProperties(entity);
            if (indexables.Count == 0)
            {
                return;
            }

            // the just-persisted entity has full-text indexable fields, so create an indexed document
            Type entityType = entity.GetType();
            string entityName = entityType.Name;
            string entityIdName = string.Format("{0}Id", entityName);
            string entityIdValue = entityType.GetProperty(Lambda.Property<Entity>(x => x.Id)).GetValue(entity, null).ToString();

//            this.logger.DebugFormat("Inserting FT Index for {0} {1}...", entityIdName, entityIdValue);

            using (var analyzer = new StandardAnalyzer(Version.LUCENE_30))
            using (
                var writer = new IndexWriter(this.FullSearchDirectory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED))
            {
                Document doc = this.CreateDocument(entity, entityIdName, entityIdValue, indexables);
                writer.AddDocument(doc);
//                this.logger.DebugFormat("Inserted the FT index for {0} {1}", entityIdName, entityIdValue);
            }
        }

        /// <summary>
        /// Creates new indices for the collection of index objects supplied
        /// </summary>
        /// <typeparam name="T">
        /// Type of entities
        /// </typeparam>
        /// <param name="entities">
        /// The list of entities
        /// </param>
        public void PopulateIndex<T>(IEnumerable<T> entities) where T : class
        {
            Type entityType = typeof(T);
            if (!IsIndexable(entityType))
            {
                return;
            }

            string entityName = entityType.Name;
            string entityIdName = string.Format("{0}Id", entityName);

            List<PropertyInfo> indexables = this.GetIndexableProperties(entityType);

//            this.logger.DebugFormat("Populating the Full-text index with values from the {0} entity...", entityName);
            using (var analyzer = new StandardAnalyzer(Version.LUCENE_30))
            using (
                var writer = new IndexWriter(this.FullSearchDirectory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED))
            {
                foreach (T entity in entities)
                {
                    string entityIdValue = entityType.GetProperty(Lambda.Property<Entity>(x => x.Id)).GetValue(entity, null).ToString();
                    Document doc = this.CreateDocument(entity, entityIdName, entityIdValue, indexables);
                    writer.AddDocument(doc);
                }
            }

//            this.logger.DebugFormat("Index population of {0} is complete.", entityName);
        }

        /// <summary>
        /// Creates new indices for the collection of index objects supplied
        /// </summary>
        /// <param name="entities">
        /// The list of entities
        /// </param>
        public void PopulateIndex(IEnumerable entities) 
        {
            object first = null;
            if (!entities.Any() || !IsIndexable(entities.First()))
            {
                return;
            }

            Type entityType = first.GetType();
            string entityName = entityType.Name;
            string entityIdName = string.Format("{0}Id", entityName);

            List<PropertyInfo> indexables = this.GetIndexableProperties(entityType);

//            this.logger.DebugFormat("Populating the Full-text index with values from the {0} entity...", entityName);
            using (var analyzer = new StandardAnalyzer(Version.LUCENE_30))
            using (var writer = new IndexWriter(this.FullSearchDirectory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED))
            {
                foreach (var entity in entities)
                {
                    string entityIdValue = entityType.GetProperty(Lambda.Property<Entity>(x => x.Id)).GetValue(entity, null).ToString();
                    Document doc = this.CreateDocument(entity, entityIdName, entityIdValue, indexables);
                    writer.AddDocument(doc);
                }
            }

//            this.logger.DebugFormat("Index population of {0} is complete.", entityName);
        }

        /// <summary>
        /// The search guids.
        /// </summary>
        /// <param name="searchText">
        /// The search text.
        /// </param>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <param name="maxRows">
        /// The max rows.
        /// </param>
        /// <returns>
        /// The <see cref="Guid[]"/>.
        /// </returns>
        public Guid[] SearchGuids(string searchText, Type type, int maxRows)
        {
            Func<object, Guid> converter = x =>
            {
                Guid trial;
                return x == null || !Guid.TryParse(x.ToString(),  out trial)
                           ? Guid.Empty
                           : trial;
            };
            return this.Search(searchText, type, maxRows, converter);
        }

        /// <summary>
        /// The search.
        /// </summary>
        /// <param name="searchText">
        /// The search text.
        /// </param>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <param name="maxRows">
        /// The max rows.
        /// </param>
        /// <returns>
        /// The <see cref="int[]"/>.
        /// </returns>
        public int[] Search(string searchText, Type type, int maxRows)
        {
            Func<object, int> converter = x =>
                {
                    int trial;
                    return x == null
                           || !int.TryParse(x.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out trial)
                               ? 0
                               : trial;
                };
            return this.Search(searchText, type, maxRows, converter);
        }

        /// <summary>
        /// Returns a list of object IDs that match the search criteria.
        /// </summary>
        /// <typeparam name="TId">
        /// </typeparam>
        /// <param name="searchText">
        /// </param>
        /// <param name="type">
        /// </param>
        /// <param name="maxRows">
        /// </param>
        /// <param name="idParser">
        /// The id Parser.
        /// </param>
        /// <returns>
        /// The <see cref="TId[]"/>.
        /// </returns>
        public TId[] Search<TId>(string searchText, Type type, int maxRows, Func<object, TId> idParser)
        {
            if (string.IsNullOrWhiteSpace(searchText))
            {
                return new TId[] { };
            }

//            this.logger.DebugFormat(
//                "Searching Type {0} for max results of {2} with search string '{3}'...", 
//                type.Name, 
//                maxRows, 
//                searchText);
            using (var searcher = new IndexSearcher(this.FullSearchDirectory, true))
            using (var analyzer = new StandardAnalyzer(Version.LUCENE_30))
            {
                var parser = new MultiFieldQueryParser(
                    Version.LUCENE_30, 
                    this.GetIndexableProperties(type).Select(x => x.Name).ToArray(), 
                    analyzer);
                parser.AllowLeadingWildcard = true;
                Query query;
                try
                {
                    query = parser.Parse("*" + searchText.Trim() + "*");
                }
                catch (ParseException)
                {
                    // if a ParseException is thrown, it's likely due to extraneous odd symbols
                    // in the search text, so this will escape them.
                    query = parser.Parse("*" + QueryParser.Escape(searchText.Trim()) + "*");
                }

                ScoreDoc[] hits = searcher.Search(query, null, maxRows, Sort.RELEVANCE).ScoreDocs;
                TId[] results = hits.Select(x => idParser(searcher.Doc(x.Doc).Get(string.Format("{0}Id", type.Name)))).ToArray();

//                this.logger.DebugFormat("Found {0} hits for '{1}'.", results.Count(), searchText);
                return results;
            }
        }

        /// <summary>
        /// Refreshes the index of the supplied entity, assuming that the entity is indexable.
        /// </summary>
        /// <param name="entity">
        /// </param>
        public void UpdateIndexOnEntity(object entity)
        {
            if (!FullTextEnabled(entity))
            {
                return;
            }

            List<PropertyInfo> indexables = this.GetIndexableProperties(entity);
            if (indexables.Count == 0)
            {
                return;
            }

            Type entityType = entity.GetType();
            string entityName = entityType.Name;
            string entityIdName = string.Format("{0}Id", entityName);
            string entityIdValue = entityType.GetProperty(Lambda.Property<Entity>(x => x.Id)).GetValue(entity, null).ToString();

//            this.logger.DebugFormat("Updating FT Index for {0} {1}...", entityIdName, entityIdValue);

            using (var analyzer = new StandardAnalyzer(Version.LUCENE_30))
            using (var writer = new IndexWriter(this.FullSearchDirectory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED))
            {
                Document doc = this.CreateDocument(entity, entityIdName, entityIdValue, indexables);
                var val = doc.GetFieldable(entityIdName).StringValue;
                var term = new Term(entityIdName, val);
                writer.UpdateDocument(term, doc);
//                this.logger.DebugFormat("Updated the FT index for {0} {1}", entityIdName, entityIdValue);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The create document.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <param name="entityIdName">
        /// The entity id name.
        /// </param>
        /// <param name="entityIdValue">
        /// The entity id value.
        /// </param>
        /// <param name="indexables">
        /// The indexables.
        /// </param>
        /// <returns>
        /// The <see cref="Document"/>.
        /// </returns>
        private Document CreateDocument(
            object entity, 
            string entityIdName, 
            string entityIdValue, 
            IEnumerable<PropertyInfo> indexables)
        {
            var doc = new Document();
//            this.logger.DebugFormat("Adding {0} of {1}...", entityIdName, entityIdValue);
            doc.Add(new Field(entityIdName, entityIdValue, Field.Store.YES, Field.Index.NOT_ANALYZED));
            foreach (PropertyInfo property in indexables)
            {
                string propertyName = property.Name;
                string propertyValue = property.GetValue(entity, null).Return(x => x.ToString(), string.Empty);
//                this.logger.DebugFormat("Adding property '{0}'...", propertyName);
                doc.Add(new Field(propertyName, propertyValue, Field.Store.YES, Field.Index.ANALYZED));
            }

            return doc;
        }

        #endregion
    }
}