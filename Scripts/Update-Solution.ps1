Param()

$CWD = (Get-Item -LiteralPath $PSScriptRoot);
$CSharp = (Get-Item -LiteralPath (Join-Path -Path $CWD -ChildPath "UpdateSolution.cs"));
$SolutionFile = (Join-Path -Path $CWD -ChildPath ".." -AdditionalChildPath @("SDK Mods", "SDK Mods.sln"));

$CSharpScript = (Get-Content -LiteralPath $CSharp -Raw);

$Item = (Add-Type -Language CSharp @"
$($CSharpScript)
"@);

$Output = ([NekoBoiNick.CoreKeeperMods.UpdateSolution]::TryUpdateSolution($SolutionFile));

If ($Output.Error -ne $True) {
  $Output.Data | Out-File -FilePath $SolutionFile;
}