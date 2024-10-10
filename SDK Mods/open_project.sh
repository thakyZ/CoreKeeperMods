#!/usr/bin/bash
SCRIPT_DIR=$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")" &> /dev/null && pwd)

echo "Script Directory = \"${SCRIPT_DIR}\"";

PWSH=$(which "pwsh")

exitcode=1

if [ -z "${PWSH}" ]; then
  powershell -noprofile -c "& \"${SCRIPT_DIR}\Open-Project.ps1\" \"$*\""
  exitcode=$?
else
  "${PWSH}" -noprofile -c "& \"${SCRIPT_DIR}\Open-Project.ps1\" \"$*\""
  exitcode=$?
fi

exit $exitcode
