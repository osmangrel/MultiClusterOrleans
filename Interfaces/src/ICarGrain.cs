using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces
{
    public interface ICarGrain : IGrainWithGuidKey
    {
        Task<string> GetCarAbilities(string message, GrainCancellationToken grainCancellationToken);
    }
}
