cd MultiCube
dotnet build -c Release
dotnet publish -c Release -r win-x86 -o bin/publish/win-x86
dotnet publish -c Release -r win-x64 -o bin/publish/win-x64
dotnet publish -c Release -r osx-x64 -o bin/publish/osx-x64
dotnet publish -c Release -r linux-x64 -o bin/publish/linux-x64

