# BlueStack Unity Demo

## Introduction

[![Build Status](<https://teamcity.azerdev.com/app/rest/builds/buildType:(id:BlueStack_Sdk_Build_SDK_Unity)/statusIcon>)](https://teamcity.azerdev.com/viewType.html?buildTypeId=BlueStack_Sdk_Build_SDK_Unity&guest=1)

**BlueStack Unity Demo** is a project to showcase all the functionalities that BlueStack Unity SDK provides for both iOS and Android platform.

## Prerequisites

-   Use Unity 2020 or higher
-   Android
    - Minimum Android API level of 21 or higher  
    - Target Android API level 31 or higher
-   iOS
    - Xcode 15.3 or higher  
    - iOS 12.2 or higher
    - [CocoaPods](https://guides.cocoapods.org/using/getting-started.html)

## Import the package

Follow the steps listed below to ensure your project includes the BlueStack SDK.

- Add scoped registries in `Edit -> Project Settings -> Package Manager -> Add Scoped Registry`

- Add npm registry for BlueStack

  ```showLineNumbers
  Name: Azerion
  URL: http://registry.npmjs.com
  Scope(s): com.azerion.bluestack
  ```

- BlueStack Unity package has a indirect dependency of **EDM4U** which need to be resolved using UPM.

  ```showLineNumbers
  Name: package.openupm.com
  URL: https://package.openupm.com
  Scope(s): com.google.external-dependency-manager
  ```

- Update `Packages/manifest.json`
  ```json showLineNumbers
  {
    "dependencies": {
      "com.azerion.bluestack": "3.0.0"
    }
  }
  ```

## Manage Dependencies

BlueStack Unity plugin maintains native BlueStack SDK version compatibility. To accomplish this BlueStack Unity plugin
is distributed with the [**EDM4U**](https://github.com/googlesamples/unity-jar-resolver). It provides Unity plugins
the ability to declare dependencies(Android specific libraries (e.g., AARs) or iOS CocoaPods), which are then
automatically resolved and copied into your Unity project.

Note: The BlueStack Unity plugin dependencies are listed in Packages/com.azerion.bluestack/Editor/BlueStackDependencies.xml file.

## Mediation Ad Network

BlueStack SDK mediation feature enable you to server ads from multiple sources including Madvertise and third-party ad networks.  

Note: To load ads from third party ad networks, you will have to first configure each ad network for your app on BlueStack console.

## Settings

BlueStack setings allows you to configure mediation networks. To open the setings, select `Azerion > BlueStack > Settings` from the menu.

#### Configure AdMob App ID

- You can insert AdMob App ID here for both iOS and Android platforms.

#### Select Mediation Networks

- You can choose your prefered mediation networks from the list. Currently this option is available for only iOS platform.

## Documentation

-   [SDK Integration](https://developers.bluestack.app/unity)
-   [Displaying Ads](https://developers.bluestack.app/unity/ad-formats/)
