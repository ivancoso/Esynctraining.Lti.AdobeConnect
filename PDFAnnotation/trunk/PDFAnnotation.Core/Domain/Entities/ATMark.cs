using System;
using System.Collections.Generic;
using Esynctraining.Core.Domain.Entities;
using PDFAnnotation.Core.Business.Models;

namespace PDFAnnotation.Core.Domain.Entities
{
    /// <summary>
    /// The mark entity.
    /// </summary>
    [Serializable]
    public class ATMark : IEquatable<ATMark>, IEntity<Guid>
    {
        #region Fields

        /// <summary>
        /// The requested hash code.
        /// </summary>
        private int? requestedHashCode;

        /// <summary>
        /// The notes.
        /// </summary>
        private ISet<ATShape> shapes = new HashSet<ATShape>();

        /// <summary>
        /// The rotations.
        /// </summary>
        private ISet<ATRotation> rotations = new HashSet<ATRotation>();

        /// <summary>
        /// The drawings.
        /// </summary>
        private ISet<ATDrawing> drawings = new HashSet<ATDrawing>();

        /// <summary>
        /// The highlight strike outs.
        /// </summary>
        private ISet<ATHighlightStrikeOut> highlightStrikeOuts = new HashSet<ATHighlightStrikeOut>();

        /// <summary>
        /// The text items.
        /// </summary>
        private ISet<ATTextItem> textItems = new HashSet<ATTextItem>();


        /// <summary>
        /// The picture items.
        /// </summary>
        private ISet<ATPicture> pictures = new HashSet<ATPicture>();

        /// <summary>
        /// The formula items.
        /// </summary>
        private ISet<ATFormula> formulas = new HashSet<ATFormula>();


        /// <summary>
        /// The annotation items.
        /// </summary>
        private ISet<ATAnnotation> annotations = new HashSet<ATAnnotation>();

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the notes.
        /// </summary>
        public virtual ISet<ATRotation> Rotations
        {
            get
            {
                return this.rotations;
            }

            set
            {
                this.rotations = value;
            }
        }

        /// <summary>
        /// Gets or sets the notes.
        /// </summary>
        public virtual ISet<ATShape> Shapes
        {
            get
            {
                return this.shapes;
            }

            set
            {
                this.shapes = value;
            }
        }

        /// <summary>
        /// Gets or sets the drawings.
        /// </summary>
        public virtual ISet<ATDrawing> Drawings
        {
            get
            {
                return this.drawings;
            }

            set
            {
                this.drawings = value;
            }
        }

        /// <summary>
        /// Gets or sets the highlight strike outs.
        /// </summary>
        public virtual ISet<ATHighlightStrikeOut> HighlightStrikeOuts
        {
            get
            {
                return this.highlightStrikeOuts;
            }

            set
            {
                this.highlightStrikeOuts = value;
            }
        }

        /// <summary>
        /// Gets or sets the text items.
        /// </summary>
        public virtual ISet<ATTextItem> TextItems
        {
            get
            {
                return this.textItems;
            }

            set
            {
                this.textItems = value;
            }
        }


        /// <summary>
        /// Gets or sets the text items.
        /// </summary>
        public virtual ISet<ATPicture> Pictures
        {
            get
            {
                return this.pictures;
            }

            set
            {
                this.pictures = value;
            }
        }


        /// <summary>
        /// Gets or sets the formulas.
        /// </summary>
        public virtual ISet<ATFormula> Formulas
        {
            get
            {
                return this.formulas;
            }

            set
            {
                this.formulas = value;
            }
        }



        /// <summary>
        /// Gets or sets the formulas.
        /// </summary>
        public virtual ISet<ATAnnotation> Annotations
        {
            get
            {
                return this.annotations;
            }

            set
            {
                this.annotations = value;
            }
        }

        /// <summary>
        /// Gets or sets the date changed.
        /// </summary>
        public virtual DateTime DateChanged { get; set; }

        /// <summary>
        /// Gets or sets the date created.
        /// </summary>
        public virtual DateTime DateCreated { get; set; }

        /// <summary>
        /// Gets or sets the file.
        /// </summary>
        public virtual File File { get; set; }

        /// <summary>
        /// Gets or sets the display format.
        /// </summary>
        public virtual string DisplayFormat { get; set; }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public virtual Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the page index.
        /// </summary>
        public virtual int PageIndex { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether readonly.
        /// </summary>
        public virtual bool IsReadonly { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        public virtual string Type { get; set; }

        /// <summary>
        /// Gets or sets the object rotation angle.
        /// </summary>
        public virtual float? Rotation { get; set; }


        /// <summary>
        /// Gets or sets the created by.
        /// </summary>
        public virtual string CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets the updated by.
        /// </summary>
        public virtual string UpdatedBy { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Compare equality trough Id
        /// </summary>
        /// <param name="other">
        /// Entity to compare.
        /// </param>
        /// <returns>
        /// true is are equals
        /// </returns>
        /// <remarks>
        /// Two entities are equals if they are of the same hierarchy tree/sub-tree
        /// and has same id.
        /// </remarks>
        public virtual bool Equals(ATMark other)
        {
            if (null == other || !this.GetType().IsInstanceOfType(other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            bool otherIsTransient = other.Id == default(Guid);
            bool thisIsTransient = this.IsTransient();
            if (otherIsTransient && thisIsTransient)
            {
                return ReferenceEquals(other, this);
            }

            return other.Id.Equals(this.Id);
        }

        /// <summary>
        /// The equals.
        /// </summary>
        /// <param name="obj">
        /// The object.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public override bool Equals(object obj)
        {
            var that = obj as ATMark;
            return this.Equals(that);
        }

        /// <summary>
        /// The get hash code.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        // ReSharper disable BaseObjectGetHashCodeCallInGetHashCode
        // ReSharper disable NonReadonlyFieldInGetHashCode
        public override int GetHashCode()
        {
            if (!this.requestedHashCode.HasValue)
            {
                this.requestedHashCode = this.IsTransient() ? base.GetHashCode() : this.Id.GetHashCode();
            }

            return this.requestedHashCode.Value;
        }
        // ReSharper restore BaseObjectGetHashCodeCallInGetHashCode
        // ReSharper restore NonReadonlyFieldInGetHashCode

        /// <summary>
        /// The is transient.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public virtual bool IsTransient()
        {
            return this.Id == default(Guid);
        }

        #endregion
    }
}