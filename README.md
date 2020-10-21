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
[Release notes](https://docs.advancedschealthcheck.com/releases)

## Installation

[Installation Guide](https://docs.advancedschealthcheck.com/installation)

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

