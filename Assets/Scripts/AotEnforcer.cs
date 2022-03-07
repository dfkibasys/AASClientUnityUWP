using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Utilities;
using BaSyx.Models.Connectivity.Descriptors;

public class AotTypeEnforcer :MonoBehaviour {
    public void Awake() {
        AotHelper.EnsureList<IAssetAdministrationShellDescriptor>();
        AotHelper.Ensure(() => {
            _ = new List<IAssetAdministrationShellDescriptor>(); 
        });
    }
}