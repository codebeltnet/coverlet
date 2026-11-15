$ErrorActionPreference = 'Stop'

Push-Location $PSScriptRoot
try {
    $version = minver -i -t v -v w
    if ($LASTEXITCODE -ne 0) { throw 'MinVer failed to resolve the documentation image version.' }

    docker tag coverlet-docfx:$version jcr.codebelt.net/geekle/coverlet-docfx:$version
    if ($LASTEXITCODE -ne 0) { throw 'Tagging the documentation image failed.' }

    docker push jcr.codebelt.net/geekle/coverlet-docfx:$version
    if ($LASTEXITCODE -ne 0) { throw 'Publishing the documentation image failed.' }
}
finally {
    Pop-Location
}
