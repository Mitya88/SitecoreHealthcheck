# Advanced Sitecore Healthcheck
> The repository contains source code and documentation for the Advanced Sitecore Healthcheck Application.
[Detailed Documentation](https://docs.advancedschealthcheck.com/)

![Logo](documentation/logo.png)

# Table of Contents
* [Introduction](#Introduction)
    * [Purpose](#Purpose)
    * [Supported Sitecore Versions](#Supported-Sitecore-Versions) 
    * [Releases](#Releases)
    * [Installation](#Installation)
        * [ConfigurationFiles](#Configuration-files)
* [Container Support](#container-support)
* [Configure the development environment](#configure-the-developer-environment)
# Introduction

* [BlogPost](https://tinyurl.com/ybq26ay8)
* [Demo Video](https://www.youtube.com/watch?v=J_qk7jT_Y-U)

## Purpose
The purpose of this module to check up Sitecore components if they are working correctly and notify the devs/devops engineers in time about a possible failure or just send a reminder about some upcoming expiration (e.g licence or certificate)


## Supported Sitecore Versions

- Sitecore 9.0
- Sitecore 9.0 Update-1
- Sitecore 9.0 Update-2
- Sitecore 9.1 
- Sitecore 9.1 Update-1
- Sitecore 9.2
- Sitecore 9.3 
- Sitecore 10.0

## Releases
- 1.0  - [package](sc.package/Advanced.Sitecore.Healthcheck-1.0.zip)
  - Initial Release
  - Hotfix for initial release [package](sc.package/Advanced.Sitecore.Healthcheck.Hotfix.20200521-1.0.zip)
  - Download and install the hotfix package. (It only contains file changes, it will not update your configured items)
    - **Fixes**
    - 'ReRun' option returned wrong orders of error entries
    - Added ?sc_site=shell querystring to each API call, it makes sure to using proper site 
    - Added language switcher, the healtcheck may not work properly if your Default Content Language is not 'en'
- 1.0.1 - [package](sc.package/Advanced.Sitecore.Healthcheck-1.0.1.zip)
  - Contains hotfix #1
  - Fixing performance issue in Search Component check
  - Fixing an issue in Search Component Check - exception has been throwed when 'Run' button was clicked in the application
- 1.1.0 - [package](sc.package/Advanced.Sitecore.Healthcheck-1.1.0.zip)
  - Moving models into customization project
  - Adding Readonly repository to customization project
  - publishing customization project on [Nuget.org](https://www.nuget.org/packages/AdvancedSitecoreHealthCheckExtensions/1.1.0)
- 1.2.0 - [Full Installer](sc.package/Advanced.Sitecore.Healthcheck-1.2.0.zip)
  - [Upgrade Package](sc.package/Advanced.Sitecore.Healthcheck-upgrade-1.2.0.zip)
    - You can upgrade any of previous healthcheck module version with the upgrade package. (Existing component configurations wont be overwritten)
  - [SPE Extenions](sc.package/Advanced.Sitecore.Healthcheck.SPE.Extensions-1.2.0.zip)
    - Gives you ability to write custom checks in Sitecore PowerShell Extenions
    - It has a powershell health check report application
  - Contains:
    - Local Disk Space Check
    - SPE Support
    - Ability to remove error entries for **Administrator** users
    - Application contains a link for the official documentation
- 1.3.0 - [Full Installer](sc.package/Advanced.Sitecore.Healthcheck-1.3.0.zip)
  - [Upgrade Package](sc.package/Advanced.Sitecore.Healthcheck-upgrade-1.3.0.zip)
    - You can upgrade the v1.2 healthcheck module version with the upgrade package. (Existing component configurations wont be overwritten)  
  - Contains:
    - [Memory and CPU Usage](https://docs.advancedschealthcheck.com/memory-and-cpu-usage)
    - LogFile check bugfixes
    - Updated UI
      - Redesigned header 
      - new grid styled view

## Installation

Provide detailed instructions on how to install the module, and include screenshots where necessary.

1. Use the Sitecore Installation wizard to install the [package](sc.package/Advanced.Sitecore.Healthcheck-1.3.0.zip)
2. Make sure if your search indexes are working correctly
3. Go the LaunchPad and open the Healthcheck

### Configuration files
The package contains a configuration patch, which sets the "Sitecore.Services.SecurityPolicy" to "ServicesOnPolicy" - it is required for the Speak application.

**Settings in the Sitecore.Healthcheck.config**


| Key        | Value           |
| ------------- |:-------------:| 
| Healthcheck.MaxNumberOfThreads      | Sets the maximum number of threads which will be used during running healthcheck | 

#
![Startpage](documentation/startpage.png)


![UI](documentation/ui.png)


# Container Support

An assets image is available on Docker for [Sitecore containered development](https://containers.doc.sitecore.com/docs/intro). 
https://hub.docker.com/repository/docker/mitya88/sitecore-advanced-healthcheck-assets

You can reference the image with mitya88/sitecore-advanced-healthcheck-assets name

Read more [here](https://medium.com/@mitya_1988/docker-asset-image-for-the-advanced-sitecore-healthcheck-module-is-available-62ca61995f43)

# Configure the developer environment

If you want to enhance or contribute into the module, you should perform the following steps to setup the codebase locally.

## How to setup the API
* It should work with Sitecore 9.0, 9.1, 9.2, 9.3
* Build the Healthcheck.Service.sln Visual Studio Solution. 
* Copy the **Healthcheck.Service.dll** and **Healthcheck.Service.Customization.dll** and pdb files into your Sitecore's bin folder. 
* Copy the Configuration files from the Healthcheck.Service\App_Config\Include\Healtcheck folder into your Sitecore instance
* Sync the items with Unicorn

## How to setup the client Application
- go to the \src\Healthcheck.Client\ folder
- make sure you are using node version 8.x
- run "npm install" in the folder
- run "npm run build" command
- Copy the DIST folder content to \sitecore\shell\client\Applications\healthcheck\ (Create the healthcheck folder)
- Open in http://sc.local/sitecore/shell/client/Applications/healthcheck/ url

