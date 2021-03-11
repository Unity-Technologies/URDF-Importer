# UnityMeshImporter

Runtime mesh importer for Unity using AssimpNet. This mesh importer creates UnityEngine.GameObject from mesh files.

**Update**: Tested on Linux, macOS and Windows.

## What is Assimp? 

> Open Asset Import Library (Assimp) is a cross-platform 3D model import library which aims to provide a common application programming interface (API) for different 3D asset file formats. 
> Written in C++, it offers interfaces for both C and C++. 
> Bindings to other languages (e.g., BlitzMax, C#, Python) are developed as part of the project or are available elsewhere.
> 
> By Wikipedia
  
This project uses C# .NET wrapper for the Assimp, [AssimpNet](https://bitbucket.org/Starnick/assimpnet/src/master/)

Supported file formates are listed [here](http://assimp.sourceforge.net/main_features_formats.html). 

## Quickstart

Before you start, you may need to install minizip package by 

```sh
$ sudo apt install minizip
```

1. Install "com.donghok.meshimporter" package as follows:
    In the Packages directory of your Unity project, 
    ```sh
    $ git clone https://github.com/eastskykang/UnityMeshImporter.git com.donghok.meshimporter
    ```
    
    or
    
    Open ```Packages/manifest.json``` and add ```"com.donghok.meshimporter":"https://github.com/eastskykang/UnityMeshImporter.git"``` to the "dependencies" list.

2. As the package is imported, you can use UnityMeshImporter as follows:

    ```cs
    using UnityMeshImporter;
    
    string meshFile = <YOUR-MESH-FILE-PATH>;
    var ob = MeshImporter.Load(meshFile);
    ```

3. The mesh importer uses Unity "Standard" shader. Please add Standard shader to ```Project Settings > Graphics > Built-in Shader Settings > Always Included Shaders```. 

## Unity Example

See the following examples:

- [UnityMeshImportExample](https://github.com/eastskykang/UnityMeshImportExample)
- [RaiSimUnity](https://github.com/leggedrobotics/RaiSimUnity) 
