# Integration of Asset Administration Shells (AAS) in Unity
This project integrates a slightly modified version of the [BaSyx SDK for .NET](https://git.eclipse.org/r/plugins/gitiles/basyx/basyx/+/refs/heads/master/sdks/dotnet/) into the [Unity](https://www.unity.com/) game engine while exposing functionality of internal clients for communicating with an AAS server and the registry within an easy-to-use wrapper client and an associated Unity behaviour. Scripts are also compatible with the IL2CPP scripting backend of the Universal Windows Platform (UWP) which enables using AAS concepts on Microsoft HoloLens 2, for example.

## Dependencies
- Unity >= 2020.3.9.f1, API compatibility level .NET 4.x
- AAS backend consisting of server and registry (included in project)


## Changes in BaSyx SDK
In order to achieve a compatibility with UWPs IL2CPP scripting backend, the following changes to the sources of the BaSyx SDK for .NET and its dependencies have been made:
- in `Assets/Third Party/BaSyx SDK/BaSyx.Registry.Client.Http/RegistryHttpClient.cs`:
    - line 24: changed `using System.Web;` to `using System.Net;`
    - line 70: changed `string encodedPathElement = HttpUtility.UrlEncode(pathElement);` to `string encodedPathElement = WebUtility.UrlEncode(pathElement);` 
- replaced `Assets/Plugins/Newtonsoft.Json.dll` with [Newtonsoft.Json for Unity](https://github.com/jilleJr/Newtonsoft.Json-for-Unity) 
- replaced all other DLLs in `Assets/Plugins` with the latest version (netstandard2.0) from Nuget.
- added `Assets/Scripts/AotEnforcerer.cs` (because of https://github.com/jilleJr/Newtonsoft.Json-for-Unity/issues/79)
