using System.IO.Abstractions;
using System.Threading.Tasks;

namespace Gsl 
{
    // 1. Json.Net can already solve problems with circular references
    // https://www.newtonsoft.com/json/help/html/PreserveObjectReferences.htm
    // 
    // 2. The longer plan is to extract out the components so that this can
    //    be assembled together as a powershell cmdlet or just a library, therefore, making
    //    customisations of Gsl inviting.
    public interface IDataProvider
    {
        Task<object> FetchAsync();
    }

    public interface IEngine 
    {
        Task<IFileInfo[]> ExecuteAsync(IFileInfo templateFile, IDataProvider dataSource);
    }

    public class JsonFileDataProvider : IDataProvider
    {
        public Task<object> FetchAsync()
        {
            throw new System.NotImplementedException();
        }
    }

}