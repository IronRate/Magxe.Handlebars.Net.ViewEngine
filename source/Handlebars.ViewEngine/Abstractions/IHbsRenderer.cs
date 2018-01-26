using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace HandlebarsDotNet.ViewEngine.Abstractions
{
    public interface IHbsRenderer
    {
        Task<string> Render(string filePath, object viewData);
    }
}