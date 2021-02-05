# escape=`

 ARG BASE_IMAGE
 ARG BUILD_IMAGE

 FROM ${BUILD_IMAGE} AS prep

# # Gather only artifacts necessary for NuGet restore, retaining directory structure
 COPY *.sln nuget.config \nuget\
 COPY src\ \temp\
 RUN Invoke-Expression 'robocopy C:\temp C:\nuget\src /s /ndl /njh /njs *.csproj'

FROM ${BUILD_IMAGE} AS builder

 ARG BUILD_CONFIGURATION

 SHELL ["powershell", "-Command", "$ErrorActionPreference = 'Stop'; $ProgressPreference = 'SilentlyContinue';"]

 RUN Invoke-WebRequest -OutFile nodejs.zip -UseBasicParsing "https://nodejs.org/dist/v15.4.0/node-v15.4.0-win-x64.zip"; `
     Expand-Archive nodejs.zip -DestinationPath C:\; `
     Rename-Item "C:\\node-v15.4.0-win-x64" c:\nodejs

 RUN SETX PATH C:\nodejs
 RUN npm config set registry https://registry.npmjs.org/



# # Create an empty working directory
 WORKDIR C:\build

# # Copy prepped NuGet artifacts, and restore as distinct layer to take better advantage of caching
 COPY --from=prep .\nuget .\
 RUN nuget restore

# # Copy remaining source code
 COPY src\ .\src\

# # Copy transforms, retaining directory structure
 RUN Invoke-Expression 'robocopy C:\build\src C:\out\transforms /s /ndl /njh /njs *.xdt'

# # Build website with file publish

RUN msbuild .\src\Healthcheck.Service\Healthcheck.Service.csproj  /t:Restore,Build /p:PublishProfile=DockerBuild /p:DeployOnBuild=True

# Create build directory for speak
WORKDIR C:\build_speak
COPY src\Healthcheck.Client\ .\
RUN npm install
RUN npm run build
RUN npm run toaspx
 FROM ${BASE_IMAGE}

 
 WORKDIR C:\artifacts

 # Copy final build artifacts
 COPY --from=builder C:\out\website .\website\
 COPY --from=builder C:\out\transforms .\transforms\

 COPY --from=builder C:\build_speak\build .\speakapp\sitecore\shell\client\applications\healthcheck\




