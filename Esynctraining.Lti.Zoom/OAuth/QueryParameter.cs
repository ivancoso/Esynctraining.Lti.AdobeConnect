namespace Esynctraining.Lti.Zoom.OAuth
{
    internal sealed class QueryParameter
    {
        public QueryParameter(string name, string value)
        {
            Name = name.OAuthUrlEncode();
            Value = value.OAuthUrlEncode();
        }

        public string Name { get; }

        public string Value { get; }

    }

}
