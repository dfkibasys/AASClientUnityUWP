using BaSyx.Models.Connectivity;
using BaSyx.Models.Connectivity.Descriptors;
using BaSyx.Models.Core.AssetAdministrationShell.Generics;
using BaSyx.Models.Core.Common;
using BaSyx.Registry.Client.Http;
using BaSyx.Submodel.Client.Http;
using BaSyx.Utils.Client.Http;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace AASClient
{
    /// <summary>
    /// Class for connecting to AAS Registry, retrieval of asset specific
    /// Asset Administration Shells and related Submodels based on
    /// https://git.eclipse.org/r/plugins/gitiles/basyx/basyx/+/refs/heads/master/sdks/dotnet/
    /// </summary>
    class AASClient
    {
        // Id of asset this client belongs to
        private string _assetId;

        // Client for communication with aas registry
        private RegistryHttpClient _aasRegistryClient;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="assetIdShort"></param>
        /// <param name="registryURL"></param>
        public AASClient(string assetIdShort, string registryURL)
        {
            // Create aas registry client
            RegistryClientSettings settings = new RegistryClientSettings
            {
                RegistryConfig = new RegistryClientSettings.RegistryConfiguration()
                {
                    RegistryUrl = registryURL,
                },

                ProxyConfig = new BaSyx.Utils.Settings.Sections.ProxyConfiguration()
                {
                    UseProxy = false
                }
            };

            SimpleHttpClientTimeoutHandler handler = new SimpleHttpClientTimeoutHandler()
            {
                InnerHandler = new HttpClientHandler()
                {
                    //MaxConnectionsPerServer = 100,
                    AllowAutoRedirect = true,
                    UseProxy = false,
                    //ServerCertificateCustomValidationCallback = Validate,
                },
                //DefaultTimeout = TimeSpan.FromSeconds(5)

            };


            _aasRegistryClient = new RegistryHttpClient(settings, handler);
            _aasRegistryClient.HttpClient.Timeout = TimeSpan.FromSeconds(3);

            // Set assetId
            _assetId = RetrieveAssetIdFromIdShort(assetIdShort);
        }

        public RegistryHttpClient GetAASRegistryClient()
        {
            return _aasRegistryClient;
        }

        /// <summary>
        /// Retrieve aas descriptor of asset
        /// </summary>
        /// <returns>descriptor of asset administration shell</returns>
        public IAssetAdministrationShellDescriptor GetAAS()
        {
            // Retrieve aas
            var aas = _aasRegistryClient.RetrieveAssetAdministrationShellRegistration(_assetId);
            if (!aas.Success || aas.Entity == null)
            {
                Console.Error.WriteLine("Error occured while retrieving aas registration for {0}: {1}", _assetId, aas.Messages.ToString());
                return null;
            }
            return aas.Entity;
        }

        public IEnumerable<ISubmodelDescriptor> GetRegisteredSubmodels()
        {
            var sms = _aasRegistryClient.RetrieveAllSubmodelRegistrations(_assetId);
            if (!sms.Success || sms.Entity == null)
            {
                Console.Error.WriteLine("Error occured while retrieving submodel registrations for {0}: {1}", _assetId, sms.Messages.ToString());
                return null;
            }
            return sms.Entity.Flatten();
        }

        /// <summary>
        /// Retrieve submodel for specified submodel id short
        /// </summary>
        /// <param name="submodelIdShort">id short of the submodel</param>
        /// <returns></returns>
        public ISubmodel GetSubmodelByIdShort(string submodelIdShort)
        {
            string submodelId = RetrieveSubmodelIdFromIdShort(submodelIdShort);
            return GetSubmodelById(submodelId);
        }

        /// <summary>
        /// Retrieve submodel for specified submodel semantic id
        /// </summary>
        /// <param name="submodelSemanticId">semantic id of the submodel</param>
        /// <returns></returns>
        public ISubmodel GetSubmodelBySemanticId(string submodelSemanticId)
        {
            string submodelId = RetrieveSubmodelIdFromSemanticId(submodelSemanticId);
            return GetSubmodelById(submodelId);
        }

        /// <summary>
        /// Retrieve submodel for specified submodel id
        /// </summary>
        /// <param name="submodelId">submodel id</param>
        /// <returns></returns>
        public ISubmodel GetSubmodelById(string submodelId)
        {
            var sm_client = GetSubmodelClient(submodelId);
            if (sm_client == null)
            {
                Console.Error.WriteLine("Client for submodel {0} is null!", submodelId);
                return null;
            }
            var result_sm = sm_client.RetrieveSubmodel();
            if (!result_sm.Success || result_sm.Entity == null)
            {
                Console.Error.WriteLine("Error occured while retrieving submodel {0}: {1}", submodelId, result_sm.Messages.ToString());
                return null;
            }
            return result_sm.Entity;
        }

        /// <summary>
        /// Retrieve value of submodel element for specified submodel element path from submodel with specified id
        /// </summary>
        /// <param name="submodelId"></param>
        /// <param name="submodelElementIdShortPath"></param>
        /// <returns></returns>
        public IValue GetSubmodelElementValueBySubmodelId(string submodelId, string submodelElementIdShortPath)
        {
            var sm_client = GetSubmodelClient(submodelId);
            if (sm_client == null)
            {
                Console.Error.WriteLine("Client for submodel {0} is null!", submodelId);
                return null;
            }
            var result_sme_val = sm_client.RetrieveSubmodelElementValue(submodelElementIdShortPath);
            if (!result_sme_val.Success || result_sme_val.Entity == null)
            {
                Console.Error.WriteLine("Error occured while retrieving value of submodel element {0}: {1}", submodelElementIdShortPath, result_sme_val.Messages.ToString());
                return null;
            }
            return result_sme_val.Entity;
        }

        /// <summary>
        /// Retrieve value of submodel element for specified path from submodel with specified id short
        /// </summary>
        /// <param name="submodelIdShort">id short of submodel</param>
        /// <param name="submodelElementIdShortPath">id short path of submodel elements</param>
        /// <returns></returns>
        public IValue GetSubmodelElementValueBySubmodelIdShort(string submodelIdShort, string submodelElementIdShortPath)
        {
            // Retrieve submodel id
            string submodelId = RetrieveSubmodelIdFromIdShort(submodelIdShort);
            return GetSubmodelElementValueBySubmodelId(submodelId, submodelElementIdShortPath);
        }

        /// <summary>
        /// Retrieve value of submodel element for specified path from submodel with specified semantic id
        /// </summary>
        /// <param name="submodelSemanticId"></param>
        /// <param name="submodelElementIdShortPath"></param>
        /// <returns></returns>
        public IValue GetSubmodelElementValueBySubmodelSemanticId(string submodelSemanticId, string submodelElementIdShortPath)
        {
            // Retrieve submodel id
            string submodelId = RetrieveSubmodelIdFromSemanticId(submodelSemanticId);
            return GetSubmodelElementValueBySubmodelId(submodelId, submodelElementIdShortPath);
        }

        /// <summary>
        /// Resolves SubmodelElements of type Reference
        /// </summary>
        /// <param name="referenceJSON">Json formatted reference value</param>
        /// <returns>tuple of submodel id and idSHortPath to submodel element</returns>
        public Tuple<string, string> ResolveReferenceSubmodelElement(string referenceJSON)
        {
            // Create JObject from json string

            JToken token = JToken.Parse(referenceJSON);
            var keys = token.SelectToken("keys");

            // Retrieve submodel id
            string submodelId = (string)keys[0].SelectToken("value");

            // Retrieve idShort path for submodel element
            int count = 0;
            string idShortPath = "";
            foreach (var item in keys)
            {
                if (count == 0)
                {
                    count++;
                    continue;
                }

                idShortPath += (string)item.SelectToken("value") + "/";

            }

            return new Tuple<string, string>(submodelId, idShortPath);
        }

        private SubmodelHttpClient GetSubmodelClient(string submodelId)
        {
            // Retrieve submodel registration
            var result_sm_reg = _aasRegistryClient.RetrieveSubmodelRegistration(_assetId, submodelId);
            if (!result_sm_reg.Success || result_sm_reg.Entity == null)
            {
                Console.Error.WriteLine("Error occured while retrieving registration for submodel {0}: {1}", submodelId, result_sm_reg.Messages.ToString());
                return null;
            }
            // Retrieve submodel endpoint
            string address;
            using (IEnumerator<IEndpoint> iter = result_sm_reg.Entity.Endpoints.GetEnumerator())
            {
                iter.MoveNext();
                address = iter.Current.Address;
            }


            SimpleHttpClientTimeoutHandler handler = new SimpleHttpClientTimeoutHandler()
            {
                InnerHandler = new HttpClientHandler()
                {
                    //MaxConnectionsPerServer = 100,
                    AllowAutoRedirect = true,
                    UseProxy = false,
                    //ServerCertificateCustomValidationCallback = Validate,
                },
                //DefaultTimeout = TimeSpan.FromSeconds(5)

            };

            // Create submodel client with endpoint for retrieving submodel
            return new SubmodelHttpClient(new Uri(address), handler);
        }

        private string RetrieveSubmodelIdFromIdShort(string submodelIdShort)
        {
            //var sms = _aasRegistryClient.RetrieveAllSubmodelRegistrations(_assetId, (sm) => sm.IdShort == submodelIdShort);
            var sms = _aasRegistryClient.RetrieveAllSubmodelRegistrations(_assetId, (sm) => sm.IdShort == submodelIdShort);

            string submodelId;
            using (var iter = sms.Entity.Children.GetEnumerator())
            {
                iter.MoveNext();
                submodelId = iter.Current.Value.Identification.Id;
            }
            return submodelId;
        }

        private string RetrieveSubmodelIdFromSemanticId(string submodelSemanticId)
        {
            var sms = _aasRegistryClient.RetrieveAllSubmodelRegistrations(_assetId, (sm) => sm.SemanticId.First.Value.ToString() == submodelSemanticId);
            string submodelId;
            using (var iter = sms.Entity.Children.GetEnumerator())
            {
                iter.MoveNext();
                submodelId = iter.Current.Value.Identification.Id;
            }
            return submodelId;
        }

        private string RetrieveAssetIdFromIdShort(string assetIdShort)
        {
            var aas = _aasRegistryClient.RetrieveAllAssetAdministrationShellRegistrations((aas) => aas.Asset.IdShort == assetIdShort);
            string assetId;
            using (var iter = aas.Entity.Children.GetEnumerator())
            {
                iter.MoveNext();
                assetId = iter.Current.Value.Asset.Identification.Id;
            }
            return assetId;
        }
    }

}
