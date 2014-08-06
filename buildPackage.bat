@echo off
set OUTDIR=NuGet\bin
mkdir %OUTDIR%
nuget pack NuGet\OAuthUI.nuspec -OutputDirectory %OUTDIR%

