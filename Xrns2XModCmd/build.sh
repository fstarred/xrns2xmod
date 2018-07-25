export output_path=releasePackage

xbuild /t:rebuild /p:Configuration=Release /p:DebugSymbols=false /p:Platform="x64" /p:OutputPath="$output_path"

