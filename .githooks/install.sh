#!/bin/bash
# Run once after cloning to install git hooks.
# Usage: bash .githooks/install.sh

REPO_ROOT=$(git rev-parse --show-toplevel)
HOOKS_SRC="$REPO_ROOT/.githooks"
HOOKS_DST="$REPO_ROOT/.git/hooks"

cp "$HOOKS_SRC/post-checkout" "$HOOKS_DST/post-checkout"
cp "$HOOKS_SRC/pre-commit" "$HOOKS_DST/pre-commit"
chmod +x "$HOOKS_DST/post-checkout" "$HOOKS_DST/pre-commit"

echo "Git hooks installed."
