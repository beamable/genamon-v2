using System;
using System.Reflection;
using Beamable.Server;
using System.Threading.Tasks;
using Web3FederationCommon;

namespace Beamable.Web3SolanaFederation
{
	public class Program
	{
		/// <summary>
		/// The entry point for the <see cref="Web3SolanaFederation"/> service.
		/// </summary>
		public static async Task Main()
		{
			//Preload content types from Web3FederationCommon project
			AppDomain.CurrentDomain.Load(Assembly.GetAssembly(typeof(Web3FederationCommonAssemblyIdentifier))!.GetName());
			
			// inject data from the CLI.
			await MicroserviceBootstrapper.Prepare<Web3SolanaFederation>();
			
			// run the Microservice code
			await MicroserviceBootstrapper.Start<Web3SolanaFederation>();
		}
	}
}