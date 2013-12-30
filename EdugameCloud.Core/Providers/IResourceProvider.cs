
namespace EdugameCloud.Core.Providers
{
	public interface IResourceProvider
	{
		string GetResourceString(string key, string resourceName);
		void ClearCache();
	}
}
