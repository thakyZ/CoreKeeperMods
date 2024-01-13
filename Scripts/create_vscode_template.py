#!/usr/bin/env python

"""
Creates a working VSCode workspace from VSCode workspace template file.
"""

# cSpell:ignoreRegexp (?<=\$\{)TMPLT(?=:.*\})
# cSpell:ignore testroots

import os

# Standardized Python Version
PYTHON_VERSION = "python3.11"

THIS_DIR = os.path.abspath(os.path.dirname(__file__))

REPOSITORY_FOLDER = os.path.abspath(os.path.join(THIS_DIR, ".."))
AUTOMATION_FOLDER = os.path.abspath(
    os.path.join(REPOSITORY_FOLDER, "automation"))

WORKSPACES_FOLDER = os.path.join(REPOSITORY_FOLDER, "workspaces")


def replace_macros(template_line: str):
    """
    Perform a simple replacement any macros found in the template line passed to us.

    Parameters:
        template_line (str): the line currently being read/written to in the template.

    Returns:
        str: A string containing the new line to write to file.
    """

    home = os.path.expanduser("~").replace("\\", "/")

    filled_line = template_line

    filled_line = filled_line.replace(
        r"${TMPLT:AUTOMATION_FOLDER}", AUTOMATION_FOLDER)
    filled_line = filled_line.replace(r"${TMPLT:HOME}", home)
    filled_line = filled_line.replace(
        r"${TMPLT:PYTHON_VERSION}", PYTHON_VERSION)
    filled_line = filled_line.replace(
        r"${TMPLT:REPOSITORY_FOLDER}", REPOSITORY_FOLDER)
    filled_line = filled_line.replace(
        r"${TMPLT:WORKSPACES_FOLDER}", WORKSPACES_FOLDER)

    return filled_line


def generate_vscode_workspace_files():
    """
    Go through all of the VSCODE workspace templates and generate the 'code-workspace' files homed
    to the location of this cloned repository
    """

    print("Scanning workspaces folder:")
    print(WORKSPACES_FOLDER)
    print("")

    workspace_template_files: list[str] = []

    for root, _, files in os.walk(WORKSPACES_FOLDER):
        for non_project in files:
            non_project_full = os.path.join(root, non_project)
            if os.path.isfile(non_project_full):
                _, non_project_ext = os.path.splitext(non_project_full)
                if non_project_ext == ".template":
                    workspace_template_files.append(non_project_full)

    for template_file in workspace_template_files:

        template_file_base, _ = os.path.splitext(
            os.path.basename(template_file))
        template_file_dir = os.path.dirname(template_file)
        workspace_file = os.path.join(template_file_dir, template_file_base)

        print(f"Processing template: {template_file}")

        with open(template_file, mode="r", encoding="utf-8") as read_file:
            template_lines = read_file.read().splitlines(True)

            with open(workspace_file, mode="w", encoding="utf-8") as write_file:
                print(f"Generating code-workspace: {workspace_file}")
                for template_line in template_lines:
                    file_line = replace_macros(template_line)
                    write_file.write(file_line)


def re_home_repository_main():
    """
    Main function to run all tasks.
    """
    generate_vscode_workspace_files()


if __name__ == "__main__":
    re_home_repository_main()
