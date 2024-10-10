#!/usr/bin/env python

"""
Creates a working VSCode workspace from VSCode workspace template file.
"""

# cSpell:ignoreRegexp (?<=\\?\$\\?\{)TMPLT(?=:.*\\?\})
# cSpell:ignore testroots

import os
import re
from pathlib import Path
from re import Match, Pattern
from typing import Iterator

def create_template_vars() -> dict[str, str]:
    output: dict[str, str] = {}
	# Standardized Python Version
    output["PYTHON_VERSION"] = "python3.11"
    output["THIS_DIR"] = str(Path(__file__).parent.absolute())
    output["REPOSITORY_FOLDER"] = str(Path(output["THIS_DIR"]).parent.absolute())
    output["AUTOMATION_FOLDER"] = str(Path(output["REPOSITORY_FOLDER"], "automation").absolute())
    output["WORKSPACES_FOLDER"] = str(Path(output["REPOSITORY_FOLDER"], "workspaces").absolute())
    output["HOME"] = os.path.expanduser("~").replace("\\", "/")
    return output

TEMPLATE_VARS: dict[str, str] = create_template_vars()

def replace_macros(template_line: str) -> str:
    """
    Perform a simple replacement any macros found in the template line passed to us.

    Parameters:
        template_line (str): the line currently being read/written to in the template.

    Returns:
        str: A string containing the new line to write to file.
    """

    filled_line = template_line

    REGEX: Pattern[str] = re.compile(r"\$\{TMPLT:(.*?)\}")

    matches: Iterator[Match[str]] = REGEX.finditer(template_line)

    for match in matches:
        if len(match.groups()) != 1 or match.groups()[0] == "":
            raise ArithmeticError("Failed to find match groups for template_line or the group was empty.")
        if match.groups()[0] not in TEMPLATE_VARS.keys():
            continue
        filled_line = filled_line.replace(match.string, TEMPLATE_VARS[match.groups()[0]])

    return filled_line


def generate_vscode_workspace_files():
    """
    Go through all of the VSCODE workspace templates and generate the 'code-workspace' files homed
    to the location of this cloned repository
    """

    print("Scanning workspaces folder:")
    print(TEMPLATE_VARS["WORKSPACES_FOLDER"])
    print("")

    workspace_template_files: list[str] = []

    for root, _, files in os.walk(TEMPLATE_VARS["WORKSPACES_FOLDER"]):
        for non_project in files:
            non_project_full = os.path.join(root, non_project)
            if os.path.isfile(non_project_full):
                _, non_project_ext = os.path.splitext(non_project_full)
                if non_project_ext == ".template":
                    workspace_template_files.append(non_project_full)

    for _template_file in workspace_template_files:
        template_file: Path = Path(_template_file)
        template_file_base: str = os.path.splitext(Path(template_file).name)[0]
        template_file_dir: str = str(template_file.parent)
        workspace_file: Path = Path(template_file_dir, template_file_base)

        print(f"Processing template: {template_file}")

        with template_file.open(mode="r", encoding="utf-8") as read_file:
            template_lines: list[str] = read_file.readlines()

            print(f"Replacing macros...")
            replaced_lines: list[str] = []
            for template_line in template_lines:
                replaced_lines.append(replace_macros(template_line))

            with open(workspace_file, mode="w", encoding="utf-8") as write_file:
                print(f"Generating code-workspace: {workspace_file}")
                write_file.writelines(replaced_lines)


def re_home_repository_main():
    """
    Main function to run all tasks.
    """
    generate_vscode_workspace_files()


if __name__ == "__main__":
    re_home_repository_main()
