Instructions

1. Build the project
2. Publish to folder (standalone)
3. Open terminal then run
    dotnet msbuild -t:BundleApp -p:RuntimeIdentifier=osx-x64 -property:Configuration=Release

Fix attributes
xattr -cr /path/to/BeeRock.app
