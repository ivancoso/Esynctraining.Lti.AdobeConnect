namespace PDFAnnotation.Core.Domain.DTO
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    
    using PDFAnnotation.Core.Utils;

    /// <summary>
    ///     Represents parsed page symbols
    /// </summary>
    [DataContract]
    public class ATPageSymbolsDTO
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ATPageSymbolsDTO"/> class.
        /// </summary>
        public ATPageSymbolsDTO()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ATPageSymbolsDTO"/> class.
        /// </summary>
        /// <param name="pageIndex">
        /// The page index.
        /// </param>
        /// <param name="symbols">
        /// The symbols.
        /// </param>
        public ATPageSymbolsDTO(int pageIndex, IEnumerable<CharRenderInfo> symbols)
        {
            this.pageIndex = pageIndex;
            this.symbols = symbols.ToArray();

            //var jsonSerializer = new JavaScriptSerializer();
            //jsonSerializer.MaxJsonLength = int.MaxValue;
            //this.symbolsJSON = jsonSerializer.Serialize(this.symbols);

            this.symbolsJSON = Newtonsoft.Json.JsonConvert.SerializeObject(this.symbols);
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Page index
        /// </summary>
        [DataMember]
        public int pageIndex { get; private set; }

        /// <summary>
        ///     Page symbol positions and values
        /// </summary>
        public CharRenderInfo[] symbols { get; private set; }

        /// <summary>
        ///     JSON formatted <see cref="symbols" />
        /// </summary>
        [DataMember]
        public string symbolsJSON { get; private set; }

        #endregion
    }
}