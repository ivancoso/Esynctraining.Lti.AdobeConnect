namespace EdugameCloud.Core.Domain.DTO
{
    using System;
    using System.Collections.Specialized;
    using System.Runtime.Serialization;

    using Esynctraining.Core.Utils;

    /// <summary>
    ///     The geo DTO.
    /// </summary>
    [DataContract]
    public class GeoDTO : IEquatable<GeoDTO>
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the city
        /// </summary>
        [DataMember]
        public string city { get; set; }

        /// <summary>
        ///     Gets or sets the   Country
        /// </summary>
        [DataMember]
        public string country { get; set; }

        /// <summary>
        ///     Gets or sets the   Postal code
        /// </summary>
        [DataMember]
        public string postalcode { get; set; }

        /// <summary>
        ///     Gets or sets the query string to search for
        /// </summary>
        [DataMember]
        public string query { get; set; }

        /// <summary>
        ///     Gets or sets the state
        /// </summary>
        [DataMember]
        public string state { get; set; }

        /// <summary>
        ///     Gets or sets the street
        /// </summary>
        [DataMember]
        public string street { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The equals.
        /// </summary>
        /// <param name="other">
        /// The other.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool Equals(GeoDTO other)
        {
            if (other != null)
            {
                return this.GetAll().Equals(other.GetAll(), StringComparison.InvariantCultureIgnoreCase);
            }

            return false;
        }

        /// <summary>
        ///     The has query.
        /// </summary>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public NameValueCollection GetNameValueCollection()
        {
            if (this.HasPostalCode())
            {
                return new NameValueCollection { { Lambda.Property<GeoDTO>(x => x.postalcode), this.postalcode } };
            }

            if (this.HasQuery())
            {
                return new NameValueCollection { { Lambda.Property<GeoDTO>(x => x.query), this.query } };
            }

            if (!this.IsEmpty())
            {
                return new NameValueCollection
                           {
                               { Lambda.Property<GeoDTO>(x => x.state), this.state }, 
                               { Lambda.Property<GeoDTO>(x => x.city), this.city }, 
                               { Lambda.Property<GeoDTO>(x => x.country), this.country }, 
                               { Lambda.Property<GeoDTO>(x => x.street), this.street }, 
                           };
            }

            return null;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     The get all.
        /// </summary>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        private string GetAll()
        {
            return this.street + this.city + this.country + this.state + this.postalcode + this.query;
        }

        /// <summary>
        ///     The has postal code.
        /// </summary>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        private bool HasPostalCode()
        {
            return !string.IsNullOrWhiteSpace(this.postalcode);
        }

        /// <summary>
        ///     The has query.
        /// </summary>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        private bool HasQuery()
        {
            return !string.IsNullOrWhiteSpace(this.query);
        }

        /// <summary>
        ///     The is empty.
        /// </summary>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        private bool IsEmpty()
        {
            return string.IsNullOrWhiteSpace(this.GetAll());
        }

        #endregion
    }
}