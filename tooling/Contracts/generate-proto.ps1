try {
    $ScriptDirectory = $( Split-Path $MyInvocation.MyCommand.Path )
    Write-Output "Adding Protobuf contracts to Contracts.csproj..."
    & dotnet run -c Release --project "${ScriptDirectory}\..\ModelGenerator\ModelGenerator.csproj" -- proto --input "${ScriptDirectory}\..\..\proto" --project "${ScriptDirectory}\..\Contracts\Contracts.csproj"
    Write-Output "Done."
}
catch {
    exit 1
}

