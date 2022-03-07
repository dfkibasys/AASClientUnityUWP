using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System;
using System.Collections.Concurrent;

public class AASClientBehaviour : MonoBehaviour
{

    public string AASServerIP = "http://localhost:4000";

    private ConcurrentDictionary<int, Action<IEnumerable<BaSyx.Models.Connectivity.Descriptors.ISubmodelDescriptor>>> _callbacks_submodels = new ConcurrentDictionary<int, Action<IEnumerable<BaSyx.Models.Connectivity.Descriptors.ISubmodelDescriptor>>>();
    private ConcurrentDictionary<int, IEnumerable<BaSyx.Models.Connectivity.Descriptors.ISubmodelDescriptor>> _results_submodels = new ConcurrentDictionary<int, IEnumerable<BaSyx.Models.Connectivity.Descriptors.ISubmodelDescriptor>>();

    private ConcurrentDictionary<int, Action<BaSyx.Models.Core.AssetAdministrationShell.Generics.ISubmodel>> _callbacks_submodel = new ConcurrentDictionary<int, Action<BaSyx.Models.Core.AssetAdministrationShell.Generics.ISubmodel>>();
    private ConcurrentDictionary<int, BaSyx.Models.Core.AssetAdministrationShell.Generics.ISubmodel> _results_submodel = new ConcurrentDictionary<int, BaSyx.Models.Core.AssetAdministrationShell.Generics.ISubmodel>();

    private ConcurrentDictionary<int, Action<BaSyx.Models.Core.Common.IValue>> _callbacks_submodelValues = new ConcurrentDictionary<int, Action<BaSyx.Models.Core.Common.IValue>>();
    private ConcurrentDictionary<int, BaSyx.Models.Core.Common.IValue> _results_submodelValues = new ConcurrentDictionary<int, BaSyx.Models.Core.Common.IValue>();

    private int _idCount = 0;


    //////////////////// Get Registered Submodels

    // Called from the main thread
    public void GetRegisteredSubmodels(string assetIdShort, Action<IEnumerable<BaSyx.Models.Connectivity.Descriptors.ISubmodelDescriptor>> callback)
    {
        var ret = new List<string>();

        // TODO: use static id counter generate random id
        var index = _idCount++;

        _callbacks_submodels.TryAdd(index, callback);

        var thread = new Thread(() => TGetRegisteresSubmodels(index, assetIdShort));

        thread.Start();


    }

    // called internaly by this class
    private void TGetRegisteresSubmodels(int id, string assetIdShort)
    {
        AASClient.AASClient aasClient = new AASClient.AASClient(assetIdShort, AASServerIP);

        var ret = aasClient.GetRegisteredSubmodels();

        _results_submodels.TryAdd(id, ret);

    }


    ////////////////////////////////////////////////////////////////////////////


    //////////////////// Get Registered Submodels

    // Called from the main thread
    public void GetSubmodelByIdShort(string assetIdShort, string submodelIdShort, Action<BaSyx.Models.Core.AssetAdministrationShell.Generics.ISubmodel> callback)
    {
        var ret = new List<string>();

        // TODO: generate random id
        var index = _idCount++;

        _callbacks_submodel.TryAdd(index, callback);

        var thread = new Thread(() => TGetSubmodelByIdShort(index, assetIdShort, submodelIdShort));

        thread.Start();


    }

    // called internaly by this class
    private void TGetSubmodelByIdShort(int id, string assetIdShort, string subModelIdShort)
    {
        AASClient.AASClient aasClient = new AASClient.AASClient(assetIdShort, AASServerIP);

        var ret = aasClient.GetSubmodelByIdShort(subModelIdShort);

        _results_submodel.TryAdd(id, ret);

    }


    ////////////////////////////////////////////////////////////////////////////


    //////////////////// Get Submodel value GetSubmodelElementValueBySubmodelId

    // Called from the main thread
    public void GetSubmodelElementValueBySubmodelIdShort(string assetIdShort, string submodelIdShort, string submodelElementIdShort, Action<BaSyx.Models.Core.Common.IValue> callback)
    {
        var ret = new List<string>();

        // TODO: generate random id
        var index = _idCount++;

        _callbacks_submodelValues.TryAdd(index, callback);

        var thread = new Thread(() => TGetSubmodelElementValueBySubmodelIdShort(index, assetIdShort, submodelIdShort, submodelElementIdShort));

        thread.Start();


    }

    // called internaly by this class
    private void TGetSubmodelElementValueBySubmodelIdShort(int id, string assetIdShort, string subModelIdShort, string submodelElementIdShort)
    {
        AASClient.AASClient aasClient = new AASClient.AASClient(assetIdShort, AASServerIP);

        var ret = aasClient.GetSubmodelElementValueBySubmodelIdShort(subModelIdShort, submodelElementIdShort);

        _results_submodelValues.TryAdd(id, ret);

    }

    // Update is called once per frame
    void Update()
    {


        foreach (var item in _callbacks_submodels)
        {

            IEnumerable<BaSyx.Models.Connectivity.Descriptors.ISubmodelDescriptor> output;

            if (_results_submodels.TryGetValue(item.Key, out output))
            {

                // Call callback
                item.Value(output);

                // remove callback and result obkects from dicrionaries
                Action<IEnumerable<BaSyx.Models.Connectivity.Descriptors.ISubmodelDescriptor>> callbackToDel;
                _callbacks_submodels.TryRemove(item.Key, out callbackToDel);
                IEnumerable<BaSyx.Models.Connectivity.Descriptors.ISubmodelDescriptor> resultObjtoDel;
                _results_submodels.TryRemove(item.Key, out resultObjtoDel);

            }

        }

        foreach (var item in _callbacks_submodel)
        {

            BaSyx.Models.Core.AssetAdministrationShell.Generics.ISubmodel output;

            if (_results_submodel.TryGetValue(item.Key, out output))
            {

                // Call callback
                item.Value(output);

                // remove callback and result obkects from dicrionaries
                Action<BaSyx.Models.Core.AssetAdministrationShell.Generics.ISubmodel> callbackToDel;
                _callbacks_submodel.TryRemove(item.Key, out callbackToDel);
                BaSyx.Models.Core.AssetAdministrationShell.Generics.ISubmodel resultObjtoDel;
                _results_submodel.TryRemove(item.Key, out resultObjtoDel);

            }

        }

        foreach (var item in _callbacks_submodelValues)
        {

            BaSyx.Models.Core.Common.IValue output;

            if (_results_submodelValues.TryGetValue(item.Key, out output))
            {

                // Call callback
                item.Value(output);

                // remove callback and result obkects from dicrionaries
                Action<BaSyx.Models.Core.Common.IValue> callbackToDel;
                _callbacks_submodelValues.TryRemove(item.Key, out callbackToDel);
                BaSyx.Models.Core.Common.IValue resultObjtoDel;
                _results_submodelValues.TryRemove(item.Key, out resultObjtoDel);

            }

        }
    }
}
