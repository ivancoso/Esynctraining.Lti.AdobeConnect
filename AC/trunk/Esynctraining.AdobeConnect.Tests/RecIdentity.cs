using System;

namespace Esynctraining.AdobeConnect.Tests
{
    public class RecIdentity : IEquatable<RecIdentity>
    {
        public RecIdentity(string sco, string folderId)
        {
            Sco = sco;
            FolderId = folderId;
        }

        public RecIdentity()
        {
            
        }
        public string Sco { get; set; }
        public string FolderId { get; set; }
        public bool Equals(RecIdentity other)
        {
            return Sco.Equals(other.Sco) && FolderId.Equals(other.FolderId);
        }
    }
}