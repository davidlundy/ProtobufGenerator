#!/usr/bin/env bash

if [ ! -x "$(command -v mono)" ]; then
  echo >&2 "Could not find 'mono' on the path."
  exit 1
fi

if [ ! -x "$(command -v curl)" ]; then
  echo >&2 "Could not find 'curl' on the path."
  exit 1
fi

if [ -z $XDG_DATA_HOME ]; then
    cachedir=$HOME/.local/share
else
    cachedir=$XDG_DATA_HOME;
fi

mkdir -p $cachedir
nugetVersion=latest
cachePath=$cachedir/nuget.$nugetVersion.exe

url=https://dist.nuget.org/win-x86-commandline/$nugetVersion/nuget.exe

if [ ! -f $cachePath ]; then
    wget -O $cachePath $url 2>/dev/null || curl -o $cachePath --location $url /dev/null
fi

if [ ! -e .nuget ]; then
    mkdir .nuget
    cp $cachePath .nuget/nuget.exe
fi

if [ ! -d packages/Sake ]; then
    mono .nuget/nuget.exe install Sake -Version 0.2.2 -ExcludeVersion -Out packages -Source https://api.nuget.org/v3/index.json
fi
if [ ! -d packages/dnx-mono ]; then
    mono .nuget/nuget.exe install dnx-mono -Version 1.0.0-beta8 -ExcludeVersion -Out packages -Source https://www.myget.org/F/aspnetmaster/api/v3/index.json
    chmod a+x packages/dnx-mono/bin/dnu
    chmod a+x packages/dnx-mono/bin/dnx
fi
if [ ! -d packages/dnx-coreclr-linux-x64 ]; then
    mono .nuget/nuget.exe install dnx-coreclr-linux-x64 -Version 1.0.0-beta8 -ExcludeVersion -Out packages -Source https://www.myget.org/F/aspnetmaster/api/v3/index.json
    chmod a+x packages/dnx-coreclr-linux-x64/bin/dnu
    chmod a+x packages/dnx-coreclr-linux-x64/bin/dnx
fi

mono packages/Sake/tools/Sake.exe -f makefile.shade "$@"