$ErrorActionPreference = 'Stop'
$compiler = Join-Path $env:WINDIR 'Microsoft.NET\Framework64\v4.0.30319\csc.exe'
if (-not (Test-Path -LiteralPath $compiler)) {
    $compiler = Join-Path $env:WINDIR 'Microsoft.NET\Framework\v4.0.30319\csc.exe'
}
if (-not (Test-Path -LiteralPath $compiler)) { throw 'The .NET Framework 4.x compiler was not found.' }
$framework = Split-Path -Parent $compiler
$arguments = @('/nologo', '/target:winexe', ('/out:' + (Join-Path $PSScriptRoot 'SandroneCat.exe')))
foreach ($assembly in @('PresentationCore.dll', 'PresentationFramework.dll', 'WindowsBase.dll')) {
    $arguments += '/reference:' + (Join-Path $framework ('WPF\' + $assembly))
}
$arguments += '/reference:' + (Join-Path $framework 'System.Xaml.dll')
foreach ($source in @('Cat.cs', 'Behavior.cs', 'WalkCycle.cs')) { $arguments += Join-Path $PSScriptRoot $source }
& $compiler @arguments
if ($LASTEXITCODE -ne 0) { throw 'Compilation failed. Exit the running app and try again.' }
Write-Output 'Built SandroneCat.exe successfully.'
