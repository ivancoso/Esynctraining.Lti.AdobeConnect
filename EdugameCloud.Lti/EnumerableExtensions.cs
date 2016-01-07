using System.Collections.Generic;
using System.Linq;

namespace EdugameCloud.Lti
{
    public static class EnumerableExtensions
    {
        /// <summary>
        /// The chunk.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="chunksize">
        /// The chunksize.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        public static IEnumerable<IEnumerable<T>> Chunk<T>(this IEnumerable<T> source, int chunksize)
        {
            while (source.Any())
            {
                yield return source.Take(chunksize);
                source = source.Skip(chunksize);
            }
        }

    }

}
