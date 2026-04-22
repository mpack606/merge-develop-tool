# MergeDevelopTool (mdev)

## Overview
`MergeDevelopTool` (invoked as `mdev`) is a lightweight Git workflow automation CLI tool written in C#. It aims to simplify the frequent process of updating a feature branch with the latest changes from a target branch (by default, `develop`).

## Problem Statement
When working on a feature branch, developers often need to incorporate the latest changes from the main development branch. The manual Git process involves multiple steps:
1. `git checkout develop`
2. `git pull`
3. `git checkout <feature-branch>`
4. `git merge develop`

`MergeDevelopTool` automates this entire sequence into a single command, reducing boilerplate and saving time.

## Features & Workflows

### 1. Default Merge Workflow
By running the tool without arguments (or providing a specific target branch), it securely updates your feature branch:
- **Validation**: Ensures you are not already on the target branch.
- **Update Target**: Checks out the target branch and pulls its latest remote changes (`git pull`).
- **Merge**: Switches back to your original feature branch and merges the updated target branch into it.

### 2. Quick Refresh Workflow
The tool provides a `--refresh` (or `-r`) flag to safely update the current branch while preserving uncommitted work:
- **Stash**: Saves existing changes, including untracked files (`git stash --include-untracked`).
- **Update**: Pulls the latest changes for the current branch (`git pull`).
- **Restore**: Pops the stashed changes back into the working directory (`git stash pop`).

## Usage Interface

```bash
mdev [target-branch] [options]
```

**Arguments:**
- `target-branch`: The branch to merge into current branch (default: `develop`)

**Options:**
- `--help`, `-h`: Show the help message
- `--refresh`, `-r`: Stash changes, pull latest and pop stash

## Architecture Context
The application uses `System.Diagnostics.Process` to wrap the local `git` executable. It handles execution of Git commands seamlessly by intercepting `StandardOutput` and `StandardError`, allowing the tool to parse results, check for exceptions, and report cleanly to the console.
