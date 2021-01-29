﻿namespace Esynctraining.AC.Provider.DataObjects
{
    public class ProxyCredentials : UserCredentials
    {
        public string Url { get; set; }

        public string Domain { get; set; }


        public ProxyCredentials(string login, string password) : base(login, password) { }

    }

}