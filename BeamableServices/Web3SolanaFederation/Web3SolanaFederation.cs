using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Beamable.Common;
using Beamable.Common.Api.Inventory;
using Beamable.Server;
using Beamable.Web3SolanaFederation.Endpoints;
using Beamable.Web3SolanaFederation.Extensions;
using Web3FederationCommon;

namespace Beamable.Web3SolanaFederation
{
	[Microservice(Web3FederationSettings.SolanaMicroserviceName)]
	public partial class Web3SolanaFederation : Microservice, IFederatedInventory<Web3SolanaIdentity>
	{
		[ConfigureServices]
		public static void Configure(IServiceBuilder serviceBuilder)
		{
			var dependencyBuilder = serviceBuilder.Builder;

			dependencyBuilder.AddFeatures();
			dependencyBuilder.AddEndpoints();
		}

		[InitializeServices]
		public static async Task Initialize(IServiceInitializer initializer)
		{
			try
			{
				initializer.SetupExtensions();

				// Validate configuration
				if (string.IsNullOrWhiteSpace(await initializer.Provider.GetService<Configuration>().RPCEndpoint))
				{
					throw new ConfigurationException("RPCEndpoint is not defined in realm config. Please apply the configuration and restart the service to make it operational.");
				}
			}
			catch (Exception ex)
			{
				BeamableLogger.LogException(ex);
				BeamableLogger.LogError("Service initialization failed. Fix the issues before using the service.");
			}
		}

		public async Promise<FederatedAuthenticationResponse> Authenticate(string token, string challenge, string solution)
		{
			return await Provider.GetService<AuthenticateEndpoint>()
				.Authenticate(token, challenge, solution);
		}

		public async Promise<FederatedInventoryProxyState> GetInventoryState(string id)
		{
			return await Provider.GetService<GetInventoryStateEndpoint>()
				.GetInventoryState(id);
		}

		public async Promise<FederatedInventoryProxyState> StartInventoryTransaction(string id, string transaction, Dictionary<string, long> currencies, List<FederatedItemCreateRequest> newItems, List<FederatedItemDeleteRequest> deleteItems,
			List<FederatedItemUpdateRequest> updateItems)
		{
			return await Provider.GetService<GetInventoryStateEndpoint>()
				.GetInventoryState(id);
		}
	}
}