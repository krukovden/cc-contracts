try {
    $ScriptDirectory = $( Split-Path $MyInvocation.MyCommand.Path )
    Write-Output "Generating JSON contracts..."
    & dotnet run -c Release --project "${ScriptDirectory}\..\ModelGenerator\ModelGenerator.csproj" -- --input "${ScriptDirectory}\..\..\json" --project "${ScriptDirectory}\..\Contracts\Contracts.csproj" --namespace "Abs.CommonCore.Contracts.Json"
    Write-Output "Done."
}
catch {
    exit 1
}

