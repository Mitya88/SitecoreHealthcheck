


((Get-Content -path .\build\index.html -Raw) -replace '<!DOCTYPE html>','<%@ Page AutoEventWireup="true" Inherits="Sitecore.Speak.Integration.Spa.SecureSpaPage" Language="C#" %><!DOCTYPE html>') | Set-Content -Path .\build\index.aspx
del .\build\index.html