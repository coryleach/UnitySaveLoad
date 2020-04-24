<h1 align="center">Welcome to com.gameframe.saveload 👋</h1>
<p>
  <img alt="Version" src="https://img.shields.io/badge/version-1.0.0-blue.svg?cacheSeconds=2592000" />
  <a href="https://twitter.com/coryleach">
    <img alt="Twitter: coryleach" src="https://img.shields.io/twitter/follow/coryleach.svg?style=social" target="_blank" />
  </a>
</p>

> This is a simple utility for quickly saving and loading objects to disk in unity.</br></br>
> Supports Binary, UnityJson, and JsonDotNet.</br>
> Optionally you can select an encrypted version of each of the above.</br>
> Additionally custom serialization methods are supported using the ISerializationMethod interface.</br>
> JsonDotNet support requires the Json.Net for Unity asset store package or Newtonsoft's Json.</br>
> For info on enabling JsonDotNet support see the <b>Enable Json.Net Support</b> section of this readme.</br>

## Quick Package Install

#### Using UnityPackageManager (for Unity 2019.1 or later)

Find the manifest.json file in the Packages folder of your project and edit it to look like this:
```js
{
  "dependencies": {
    "com.gameframe.saveload": "https://github.com/coryleach/UnitySaveLoad.git#1.0.0",
    ...
  },
}
```

## Usage

SaveLoadManager is not a singleton. Multiple instances may be used and created.<br />
In the project tab menu select Create->Gameframe->SaveLoad->SaveLoadManager<br />
This will create an instance of a SaveLoadManager asset.<br />
Select the created object and configure options via the inspector.<br />

```C#
//Use the Project tab's create menu GameFrame->SaveLoad->SaveLoadManager to create a manager
//You can then use public or serialized fields to reference your save system.
// OR
//Create a Manager at Runtime like this
manager = SaveLoadManager.Create("BaseDirectory","SaveDirectory",SerializationMethod.Default);

//Save object to disk in a file named "MySave.data"
manager.Save("MySave.data",objectToBeSaved);

//Load from disk
var loadedObject = manager.Load<SavedObjectType>("MySave.data");

//Delete saved file
manager.DeleteSave("MySave.data");

//Setup a Custom Save/Load Method by passing any object that implements ISerializationMethod
manager.SetCustomSerializationMethod(new MyCustomMethod());

//Save a ScriptableObject or any object derived from UnityEngine.Object directly to disk
var myScriptableObject = ScriptableObject.CreateInstance<MyScriptableObjectType>();
manager.SaveUnityObject(myScriptableObject,"MyUnityObjectData.dat");

//Loading a UnityEngine.Object type requires an existing object to overwrite
//The following method will overwrite all the serialized fields on myScriptableObject with values loaded from disk
manager.LoadUnityObjectOverwrite(myScriptableObject,"MyUnityObjectData.data");
```

## Enable Json.Net Support

Ensure the Json.Net for Unity package has been imported.</br>
In player settings add the string 'JSON_DOT_NET' to Scripting Define Symbols.

## Author

👤 **Cory Leach**

* Twitter: [@coryleach](https://twitter.com/coryleach)
* Github: [@coryleach](https://github.com/coryleach)

## Show your support

Give a ⭐️ if this project helped you!

***
_This README was generated with ❤️ by [readme-md-generator](https://github.com/kefranabg/readme-md-generator)_
