namespace Esynctraining.Core.Domain.Entities
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// The entity.
    /// </summary>
    [Serializable]
    [DataContract]
    public class EntityGuid : IEquatable<EntityGuid>, IEntity<Guid>
    {
        #region Fields

        /// <summary>
        /// The requested hash code.
        /// </summary>
        private int? requestedHashCode;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public virtual Guid Id { get; set; }

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
        public virtual bool Equals(EntityGuid other)
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
        /// The obj.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public override bool Equals(object obj)
        {
            var that = obj as EntityGuid;
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