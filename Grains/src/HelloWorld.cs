using Interfaces;
using Orleans;
using System.Threading.Tasks;
using Microsoft.FeatureManagement;
using System;

namespace Grains
{
    public class HelloWorld : Grain, IHelloWorld
    {
        private readonly IFeatureManagerSnapshot _featureManagerSnapshot;
        public HelloWorld(IFeatureManagerSnapshot featureManagerSnapshot)
        {
            _featureManagerSnapshot = featureManagerSnapshot;
        }
        public async Task<string> SayHello(string name, GrainCancellationToken grainCancellationToken)
        {
            string result = null;

            //if (!grainCancellationToken.CancellationToken.IsCancellationRequested &&
            //    await _featureManagerSnapshot.IsEnabledAsync("FeatureA"))
            if (!grainCancellationToken.CancellationToken.IsCancellationRequested)
            {
                result = $" {name} Grain reference - {IdentityString}";
            }
            Console.WriteLine(result);
            return result;
        }
    }
}