using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces
{
    public interface ICreatureGrain : IGrainWithGuidKey
    {
        Task<string> GetCreatureAbilities(string message, GrainCancellationToken grainCancellationToken);
    }
}
