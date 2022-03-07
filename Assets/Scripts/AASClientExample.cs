using System;
using System.Collections;
using System.Collections.Generic;
using BaSyx.Models.Connectivity.Descriptors;
using BaSyx.Models.Core.AssetAdministrationShell.Generics;
using BaSyx.Models.Core.Common;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class AASClientExample : MonoBehaviour
{

    public AASClientBehaviour AASClientBehaviour;
    public string AssetIDShort = "ur10_1";

    // Start is called before the first frame update
    void Start()
    {
        // Retrieve registered submodels for asset
        Debug.LogFormat("Submodels requested for asset with idShort {0} ...", AssetIDShort);
        AASClientBehaviour.GetRegisteredSubmodels(AssetIDShort, onSubmodelsReceived);

        // Retrieve Identification submodel
        Debug.LogFormat("Submodel requested for asset with idShort {0} ...", AssetIDShort);
        AASClientBehaviour.GetSubmodelByIdShort(AssetIDShort, "Identification", onSubmodelReceived);

        // Retrieve submodel element values of Identfication submodel
        Debug.LogFormat("Submodels Element Value requested for asset with idShort {0} ...", AssetIDShort);
        AASClientBehaviour.GetSubmodelElementValueBySubmodelIdShort(AssetIDShort, "Identification", "ManufacturerName", onSubmodelElementValueReceived);

        // Retrieve submodel element values of Signals submodel
        string signals = "Signals";
        string type = "MessageBrokerSet/MessageBroker01/Type";
        AASClientBehaviour.GetSubmodelElementValueBySubmodelIdShort(AssetIDShort, "Signals", type, ret =>
        {
            Debug.LogFormat("Received Submodel Element Value of submodel {0} for asset {1} -> {2}: {3}", signals, AssetIDShort, type, ret.Value);
        });

        string connection = "MessageBrokerSet/MessageBroker01/ConnectionString";
        AASClientBehaviour.GetSubmodelElementValueBySubmodelIdShort(AssetIDShort, signals, connection, ret =>
        {
            Debug.LogFormat("Received Submodel Element Value of submodel {0} for asset {1} -> {2}: {3}", signals, AssetIDShort, connection, ret.Value);
        });

        string signalSet = "SignalSet";
        string provisioningRate = "DataProvisionings/DataProvisioning01/ProvisioningRate";
        AASClientBehaviour.GetSubmodelElementValueBySubmodelIdShort(AssetIDShort, signals, signalSet, ret =>
        {
            var jsonSignalSet = JArray.Parse(ret.ToString());
            Debug.LogFormat("Received Submodel Element Value of submodel {0} for asset {1} -> {2}:", signals, AssetIDShort, signalSet);
            foreach (var signal in jsonSignalSet)
            {
                string signalIdShort = signal["idShort"].ToString();
                Debug.Log(signalIdShort);
                AASClientBehaviour.GetSubmodelElementValueBySubmodelIdShort(AssetIDShort, signals, signalSet + "/" + signalIdShort + "/" + provisioningRate, ret =>
                {
                    Debug.LogFormat("{0} -> {1}: {2} Hz", signalIdShort, provisioningRate, ret.Value);
                });
            }
        });
    }


    private void onSubmodelsReceived(IEnumerable<ISubmodelDescriptor> smds)
    {
        Debug.LogFormat("Submodels received for asset {0}: ", AssetIDShort);
        foreach (var sm in smds)
        {
            Debug.Log(sm.IdShort);
        }
    }

    private void onSubmodelReceived(ISubmodel sm)
    {
        string subModel = sm.Identification.Id;
        Debug.LogFormat("Received Submodel for asset {0}: {1} ", AssetIDShort, subModel);
    }

    private void onSubmodelElementValueReceived(IValue smev)
    {
        string value = smev.Value.ToString();
        Debug.LogFormat("Received Submodel Element Value for asset {0} -> ManufacturerName: {1}", AssetIDShort, value);
    }



}
