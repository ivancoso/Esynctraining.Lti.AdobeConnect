namespace Esynctraining.Core.Domain.Entities
{
    using System;
    using System.Data.SqlTypes;
    using System.Runtime.Serialization;

    /// <summary>
    /// The entity.
    /// </summary>
    [Serializable]
    [DataContract]
    public class Entity : IEquatable<Entity>, IEntity<int>
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
        public virtual int Id { get; set; }

        #endregion

        #region Public Methods and Operators

        protected DateTime AdaptToSql(DateTime dt)
        {
            if (SqlDateTime.MinValue.Value > dt)
            {
                return SqlDateTime.MinValue.Value;
            }

            if (SqlDateTime.MaxValue.Value < dt)
            {
                return SqlDateTime.MaxValue.Value;
            }

            return dt;
        }

        /// <summary>
        /// The fix date time.
        /// </summary>
        /// <param name="dt">
        /// The dt.
        /// </param>
        /// <returns>
        /// The <see cref="Nullable<DateTime>"/>.
        /// </returns>
        protected DateTime? AdaptToSql(DateTime? dt)
        {
            if (dt.HasValue)
            {
                if (SqlDateTime.MinValue.Value > dt.Value)
                {
                    return SqlDateTime.MinValue.Value;
                }

                if (SqlDateTime.MaxValue.Value < dt.Value)
                {
                    return SqlDateTime.MaxValue.Value;
                }
            }

            return dt;
        }

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
        public virtual bool Equals(Entity other)
        {
            if (null == other || !this.GetType().IsInstanceOfType(other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            bool otherIsTransient = other.Id == default(int);
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
            var that = obj as Entity;
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
            return this.Id == default(int);
        }

        #endregion
    }
}