using Interfaces;
using Orleans;
using System.Threading.Tasks;
using Microsoft.FeatureManagement;
using System;

namespace Grains.src
{
    public class CreatureGrain : Grain, ICreatureGrain
    {
        public Task<string> GetCreatureAbilities(string message, GrainCancellationToken grainCancellationToken)
        {
            string result = null;

            //if (!grainCancellationToken.CancellationToken.IsCancellationRequested &&
            //    await _featureManagerSnapshot.IsEnabledAsync("FeatureA"))
            if (!grainCancellationToken.CancellationToken.IsCancellationRequested)
            {
                result = $" {message} Grain reference - {IdentityString}";
            }
            Console.WriteLine(result);
            return Task.Delay(500).ContinueWith(t => result);
        }
    }
}
