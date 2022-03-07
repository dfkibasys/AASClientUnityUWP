# Integration of Asset Administration Shells (AAS) in Unity
This project integrates the [BaSyx SDK for .NET](https://git.eclipse.org/r/plugins/gitiles/basyx/basyx/+/refs/heads/master/sdks/dotnet/) into the [Unity](https://www.unity.com/) game engine while exposing functionality of the internal clients for communicating with an AAS server and the registry within an easy-to-use wrapper client and an associated Unity behaviour. Scripts are also compatible with the IL2CPP scripting backend of the Universal Windows Platform (UWP) which enables using AAS concepts on Microsoft HoloLens 2, for example.

## Dependencies
- Unity >= 2020.3.9.f1, API compatibility level .NET 4.x
- AAS backend consisting of server and registry (included in project)
