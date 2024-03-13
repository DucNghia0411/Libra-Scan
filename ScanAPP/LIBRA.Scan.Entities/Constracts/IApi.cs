
namespace LIBRA.Scan.Entities.Constracts
{
    public interface IApi<T> where T : class
    {
        void Init(String _baseUrl, Dictionary<String, String> Headers);
        Task<T> Get(String UrlParam);
        Task<T> Post(String UrlParam, String Content);
    }
}
