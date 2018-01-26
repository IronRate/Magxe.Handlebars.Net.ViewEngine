using System.Threading.Tasks;

namespace Magxe.Handlebars.ViewEngine.Abstractions
{
    public interface IHbsRenderer
    {
        Task<string> Render(string filePath, object viewData);
    }
}