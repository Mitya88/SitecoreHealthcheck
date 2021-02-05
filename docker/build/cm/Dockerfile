# escape=`

ARG BASE_IMAGE
ARG SPE_IMAGE
ARG TOOLING_IMAGE
ARG SOLUTION_IMAGE
ARG MANAGEMENT_SERVICES_IMAGE

FROM ${MANAGEMENT_SERVICES_IMAGE} AS management_services
FROM ${SOLUTION_IMAGE} as solution
FROM ${TOOLING_IMAGE} as tooling
FROM ${SPE_IMAGE} as spe
FROM ${BASE_IMAGE}
SHELL ["powershell", "-Command", "$ErrorActionPreference = 'Stop'; $ProgressPreference = 'SilentlyContinue';"]

# Copy development tools and entrypoint
COPY --from=tooling \tools\ \tools\

WORKDIR C:\inetpub\wwwroot

# Add SPE module
COPY --from=spe \module\cm\content .\

# Copy solution website files
COPY --from=solution \artifacts\website\ .\
COPY --from=solution \artifacts\speakapp\ .\

# Copy the Sitecore Management Services Module
COPY --from=management_services C:\module\cm\content C:\inetpub\wwwroot

# Copy role transforms
#COPY .\transforms\ \transforms\role\

# Perform solution transforms
#RUN C:\tools\scripts\Invoke-XdtTransform.ps1 -Path .\ -XdtPath C:\transforms\project\helixbase\website

# Perform role transforms
#RUN C:\tools\scripts\Invoke-XdtTransform.ps1 -Path .\ -XdtPath C:\transforms\role