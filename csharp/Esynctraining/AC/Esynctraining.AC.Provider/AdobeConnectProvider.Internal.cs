namespace Esynctraining.AC.Provider
{
    public partial class AdobeConnectProvider
    {
        private static string UrlEncode(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return value;
            return HttpUtilsInternal.UrlEncode(value);
        }

    }

}
