# Sitecore SPEAK3 starter template

This repository contains Speak 3 starter template.

This project was generated with [Angular CLI](https://github.com/angular/angular-cli) version 1.2.7.

## Development server

Run `npm run start` for a disconnected mode. Navigate to `http://localhost:4123/`. The app will automatically reload if you change any of the source files.

## Code scaffolding

Run `ng generate component component-name` to generate a new component. You can also use `ng generate directive|pipe|service|class|guard|interface|enum|module`.

## Setup

1. Set the variables in `init.ps1` (see below). 
3. Run `init.ps1` script
4. Run `npm install` command
5. Run application in disconnected mode, run `npm run start` command
6. Run `npm run deploy` command 

`npm run deploy` command will build the application, copy files into your Sitecore instance and open the application in sitecore
- Files will be copied to {$instanceWebroot}\sitecore\shell\client\applications\{$applicationName} folder
- Browser will be opened on {$instanceHost}\sitecore\shell\client\{$application} url
- You can add the following shortcut to your launchpad : {$instanceHost}\sitecore\shell\client\{$application} url

## Variables
|Variable|Purpose|
|----|----|
|$applicationName| This will be your application name|
|$applicationTitle| Application name which appears in the browser's title|
|$instanceWebRoot | Webroot of your Sitecore instance|
|$instanceHost| Url of your Sitecore instance|

