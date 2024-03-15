#!/bin/bash

# Build for Windows
dotnet publish -c Release -r win-x64

# Build for Linux
dotnet publish -c Release -r linux-x64

# Build for macOS
dotnet publish -c Release -r osx-x64
