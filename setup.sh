#!/usr/bin/env bash
# Run once after installing the .NET 8 SDK.
set -e
dotnet new sln -n BlogSphere
dotnet sln add BlogSphere.Models BlogSphere.DAL BlogSphere.BLL BlogSphere.API
dotnet restore
dotnet build
