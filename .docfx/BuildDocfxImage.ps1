$ErrorActionPreference = 'Stop'

Push-Location $PSScriptRoot
try {
    $version = minver -i -t v -v w
    if ($LASTEXITCODE -ne 0) { throw 'MinVer failed to resolve the documentation image version.' }

    docfx metadata docfx.json
    if ($LASTEXITCODE -ne 0) { throw 'DocFX metadata generation failed.' }

    docker buildx build -t coverlet-docfx:$version --platform linux/arm64,linux/amd64 --load -f Dockerfile.docfx .
    if ($LASTEXITCODE -ne 0) { throw 'The documentation image build failed.' }

    # Metadata is generated directly in api. Preserve authored namespace/type pages.
    $metadataPath = Join-Path $PSScriptRoot 'api'
    Get-ChildItem -LiteralPath $metadataPath -File |
        Where-Object { $_.Extension -eq '.yml' -or $_.Name -eq '.manifest' -or $_.Extension -eq '.manifest' } |
        ForEach-Object { Remove-Item -LiteralPath $_.FullName }
}
finally {
    Pop-Location
}
