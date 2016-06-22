using System;
using System.Collections.Generic;

namespace Esynctraining.AdobeConnect.Tests
{
    public class RecIdentity //: IEqualityComparer<RecIdentity>
//IEquatable<RecIdentity> 
    {
        public RecIdentity(string sco, string folderId)
        {
            Sco = sco;
            FolderId = folderId;
        }

        //public RecIdentity()
        //{
            
        //}
        public string Sco { get; set; }
        public string FolderId { get; set; }
        public bool Equals(RecIdentity other)
        {
            return Sco.Equals(other.Sco) && FolderId.Equals(other.FolderId);
        }

        public override bool Equals(object obj)
        {
            return Equals((RecIdentity)obj);
        }

        public override int GetHashCode()
        {
            return Sco.GetHashCode() ^ FolderId.GetHashCode();
        }

        //public bool Equals(RecIdentity x, RecIdentity y)
        //{
        //    return x.Equals(y);
        //}

        //public int GetHashCode(RecIdentity obj)
        //{
        //    return 0;
        //}
    }
}