using System;

namespace Esynctraining.Lti.Zoom.OAuth
{
    internal sealed class NonceData
    {
        public string Nonce { get; }

        public DateTime Timestamp { get; }


        public NonceData(string nonce, DateTime timestamp)
        {
            if (string.IsNullOrWhiteSpace(nonce))
                throw new ArgumentException("nonce can't be empty", nameof(nonce));

            Nonce = nonce;
            Timestamp = timestamp;
        }


        public override bool Equals(object obj)
        {
            var otherNonce = obj as NonceData;
            if (otherNonce != null)
            {
                return string.Equals(otherNonce.Nonce, Nonce, StringComparison.OrdinalIgnoreCase);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return Nonce.GetHashCode();
        }

    }

}
