using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Esynctraining.Persistence.Mappings;
using FluentNHibernate.Mapping;
using NHibernate;
using PDFAnnotation.Core.Domain.Entities;


namespace PDFAnnotation.Persistence.Mappings
{
    /// <summary>
    /// The formula map.
    /// </summary>
    public class ATFormulaMap : BaseClassMap<ATFormula>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ATFormulaMap"/> class.
        /// </summary>
        public ATFormulaMap()
        {
            this.Map(x => x.PositionX).Not.Nullable();
            this.Map(x => x.PositionY).Not.Nullable();
            this.Map(x => x.Equation).Not.Nullable();
            this.Map(x => x.Height).Nullable();
            this.Map(x => x.Width).Nullable();
            this.References(x => x.Mark).Not.Nullable();
        }

        #endregion
    }
}
