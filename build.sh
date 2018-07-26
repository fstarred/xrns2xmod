export output_path=releasePackage
export output_path_test=releasePackage/test

xbuild Xrns2XModCmd/Xrns2XModCmd.csproj /t:rebuild /p:Configuration=Release /p:DebugSymbols=false /p:Platform="x64" /p:OutputPath="$output_path"

xbuild Xrns2XModUnitTest/Xrns2XModUnitTest.csproj /t:rebuild /p:Configuration=Release /p:DebugSymbols=false /p:Platform="x64" /p:OutputPath="$output_path_test"


