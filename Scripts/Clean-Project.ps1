Param()

$SDK_Mods_Path = (Get-Item -LiteralPath (Join-Path -Path $PSScriptRoot -ChildPath ".." -AdditionalChildPath @("SDK Mods")));
$Other_Path = (Get-Item -LiteralPath (Join-Path -Path $PSScriptRoot -ChildPath ".." -AdditionalChildPath @("Other")));
$Scripts_Path = (Get-Item -LiteralPath $PSScriptRoot);

[System.IO.FileSystemInfo[]] $script:ItemsToRemove = @()

Function Test-ForDirectories {
  Param(
    # Specifies a diectory to check.
    [Parameter(Mandatory = $True,
               Position = 0,
               HelpMessage = "A diectory to check.")]
    [ValidateNotNull()]
    [System.Io.FileSystemInfo]
    $Item
  )

  If (Test-Path -LiteralPath $Item -PathType Container) {
    If ($Item.Name -eq ".vs") {
      $script:ItemsToRemove += @($Item);
    } ElseIf ($Item.Name -eq "Logs") {
      $script:ItemsToRemove += @($Item);
    } ElseIf ($Item.Name -eq "Library") {
      ForEach ($SubItem in @(Get-ChildItem -LiteralPath $Item | Where-Object { $_.Name -ne ".editorconfig"})) {
        $script:ItemsToRemove += @($SubItem);
      }
    } ElseIf ($Item.Name -eq "bin") {
      $script:ItemsToRemove += @($Item);
    } ElseIf ($Item.Name -eq "obj") {
      $script:ItemsToRemove += @($Item);
    }
  } ElseIf (Test-Path -LiteralPath $Item -PathType Leaf) {
    If ($Item.Extension -eq ".csproj" -and $Item.BaseName -ne "scripts") {
      $script:ItemsToRemove += @($Item);
    } ElseIf ($Item.Extension -eq ".sln") {
      $script:ItemsToRemove += @($Item);
    } ElseIf ($Item.Name -eq "Library") {
      $script:ItemsToRemove += @($Item);
    } ElseIf ($Item.Name -eq "bin") {
      $script:ItemsToRemove += @($Item);
    } ElseIf ($Item.Name -eq "obj") {
      $script:ItemsToRemove += @($Item);
    }
  } Else {
    Write-Warning -Message "Could not determine type of item at path $($Item.FullName)";
  }
}

Get-ChildItem -LiteralPath $SDK_Mods_Path | ForEach-Object {
  $Item = $_;
  Test-ForDirectories -Item $Item;
};
Get-ChildItem -LiteralPath $SDK_Mods_Path -Hidden | ForEach-Object {
  $Item = $_;
  Test-ForDirectories -Item $Item;
};
Get-ChildItem -LiteralPath $Other_Path | ForEach-Object {
  $Item = $_;
  Test-ForDirectories -Item $Item;
};
Get-ChildItem -LiteralPath $Other_Path -Hidden | ForEach-Object {
  $Item = $_;
  Test-ForDirectories -Item $Item;
};
Get-ChildItem -LiteralPath $Scripts_Path | ForEach-Object {
  $Item = $_;
  Test-ForDirectories -Item $Item;
};
Get-ChildItem -LiteralPath $Scripts_Path -Hidden | ForEach-Object {
  $Item = $_;
  Test-ForDirectories -Item $Item;
};

ForEach ($Item in $script:ItemsToRemove) {
  Write-Host -Object "Removing $($Item.FullName)";
  Try {
    Remove-Item -LiteralPath $Item -Recurse -ErrorAction Stop;
  } Catch {
    If ($_.Exception.Message -match "^[Yy]ou do not have sufficient access rights to perform this operation or the item is hidden, system, or read only\.$") {
      Try {
        Remove-Item -LiteralPath $Item -Recurse -Force -ErrorAction Stop;
      } Catch {
        Write-Error -Exception $_.Exception -Message $_.Exception.Message;
      }
    } Else {
      Write-Error -Exception $_.Exception -Message $_.Exception.Message;
    }
  }
}
