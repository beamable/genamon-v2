using System;
using Beamable.Common;

namespace Web3FederationCommon
{
    ///<Summary>
    /// Web3SolanaIdentity implementation
    ///</Summary>
    [FederationId(Web3FederationSettings.SolanaIdentityName)]
    public class Web3SolanaIdentity : IThirdPartyCloudIdentity
    {
        ///<Summary>
        /// Identity component name
        ///</Summary>
        public string UniqueName => Web3FederationSettings.SolanaIdentityName;
    }
    
    ///<Summary>
    /// Common Web3FederationSettings
    ///</Summary>
    public static class Web3FederationSettings
    {
        ///<Summary>
        /// Web3Federation microservice name
        ///</Summary>
        public const string MicroserviceName = "Web3Federation";
        ///<Summary>
        /// Web3Identity name
        ///</Summary>
        public const string IdentityName = "Web3Identity";
        ///<Summary>
        /// Web3Federation microservice name
        ///</Summary>
        public const string SolanaMicroserviceName = "Web3SolanaFederation";
        ///<Summary>
        /// Web3Identity name
        ///</Summary>
        public const string SolanaIdentityName = "Web3SolanaIdentity";
    }
    
    /// <summary>
    /// Empty type used for Web3FederationCommon assembly load in the Federation service
    /// </summary>
    public class Web3FederationCommonAssemblyIdentifier
    {

    }
}
