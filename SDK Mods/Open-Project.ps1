Param()

$EditorVersion = "2022.3.10f1";

$UnityModuleFetch = (Get-Module -ListAvailable -Name UnitySetup);

# Check if the UnitySetup module is installed
If (-not $UnityModuleFetch -or $Null -eq $UnityModuleFetch) {
  # Prompt to install the UnitySetup module
  Write-Host -Object "Installing UnitySetup module...";
  Install-Module -Name "UnitySetup" -Scope CurrentUser -Force -AllowClobber;
  Write-Host -Object "Done";
}

# Import the UnitySetup module
Import-Module -Name "UnitySetup";

# Check if the desired Unity version is installed
$UnityEditor = (Get-UnitySetupInstance | Select-UnitySetupInstance -Version "$($EditorVersion)");
If ($Null -eq $UnityEditor -or -not $UnityEditor) {
  # Install Unity version
  Write-Host -Object "Installing Unity $($EditorVersion)...";

  # Note: You may need to adjust this command based on the components you need
  Find-UnitySetupInstaller -Version "$($EditorVersion)" -Components Windows | Install-UnitySetupInstance;

  $UnityEditor = (Get-UnitySetupInstance | Select-UnitySetupInstance -Version "$($EditorVersion)");

  If ($Null -eq $UnityEditor -or -not $UnityEditor) {
    Write-Error -Message "Installation failed or Unity path not found.";
    Read-Host -Prompt "Press Enter to exit";
    Exit 1;
  }

  Write-Host -Object "Done";
}

$UnityEditorExecutable = (Get-UnityEditor -Instance $UnityEditor);

$ProjectDirectory = (Get-UnityProjectInstance $PSScriptRoot);

If ($Null -eq $UnityEditorExecutable -or -not $UnityEditorExecutable -or -not (Test-Path -Path $UnityEditorExecutable -PathType Leaf)) {
  Write-Error -Message "Unity editor exectuable not found.";
}

If ($Null -eq $ProjectDirectory -or -not $ProjectDirectory -or -not (Test-Path -Path $ProjectDirectory.Path -PathType Container)) {
  Write-Error -Message "Unity project not found.";
}

& "$($UnityEditorExecutable)" -openfile "$($ProjectDirectory.Path)";
$ExitCode = $LASTEXITCODE;

If ($ExitCode -ne 0) {
  Read-Host -Prompt "Press Enter to exit";
}

Exit $ExitCode;
