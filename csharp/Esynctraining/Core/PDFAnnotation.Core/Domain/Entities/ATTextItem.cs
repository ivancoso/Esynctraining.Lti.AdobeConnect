namespace PDFAnnotation.Core.Domain.Entities
{
    using System;
    using Esynctraining.Core.Domain.Entities;

    [Serializable]
    public partial class ATTextItem : Entity 
	{
		#region Private fields


		#endregion

		#region Public Properties
    
		/// <summary>
		/// Gets or sets TextId
		/// </summary>
		public virtual int TextId { get; set; }
    
		/// <summary>
		/// Gets or sets Text
		/// </summary>
		public virtual string Text { get; set; }
    
		/// <summary>
		/// Gets or sets FontName
		/// </summary>
		public virtual string FontName { get; set; }
    
		/// <summary>
		/// Gets or sets FontFamily
		/// </summary>
		public virtual string FontFamily { get; set; }
    
		/// <summary>
		/// Gets or sets FontSize
		/// </summary>
		public virtual int FontSize { get; set; }
    
		/// <summary>
		/// Gets or sets PositionX
		/// </summary>
		public virtual double PositionX { get; set; }
    
		/// <summary>
		/// Gets or sets PositionY
		/// </summary>
		public virtual double PositionY { get; set; }
    
		/// <summary>
		/// Gets or sets Color
		/// </summary>
		public virtual string Color { get; set; }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        public virtual double Width { get; set; }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        public virtual double Height { get; set; }

        /// <summary>
        /// Gets or sets text alignment
        /// </summary>
        public virtual string Alignment { get; set; }

        /// <summary>
        /// Gets or sets the mark.
        /// </summary>
        public virtual ATMark Mark { get; set; }
        
		#endregion

		#region Methods
		#endregion
	}
}

