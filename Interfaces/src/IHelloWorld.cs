using Orleans;
using System.Threading.Tasks;

namespace Interfaces
{
    public interface IHelloWorld : IGrainWithStringKey
    {
        Task<string> SayHello(string name, GrainCancellationToken grainCancellationToken);
    }
}
