#!/bin/bash
# Publish for Windows x64
dotnet publish -c Release -r win-x64 --self-contained

# Publish for Linux x64
dotnet publish -c Release -r linux-x64 --self-contained

# Publish for macOS x64
dotnet publish -c Release -r osx-x64 --self-contained

# Publish for Linux ARM64
dotnet publish -c Release -r linux-arm64 --self-contained

# Publish for Windows ARM64
dotnet publish -c Release -r win-arm64 --self-contained

# Publish for macOS ARM64
dotnet publish -c Release -r osx-arm64 --self-contained