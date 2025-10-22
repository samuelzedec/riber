#!/bin/bash

set -e

if [[ ! -f "Directory.Build.props" ]]; then
  echo "Erro: Directory.Build.props não encontrado"
  exit 1
fi

VERSION=$(sed -n 's/.*<Version>\([^<]*\)<\/Version>.*/\1/p' Directory.Build.props)

if [[ -z "$VERSION" ]]; then
  echo "Erro: Versão não encontrada"
  exit 1
fi

TAG="v${VERSION}"

echo "Tag: $TAG"

git pull origin main

git tag -a "$TAG" -m "Release version $VERSION"

echo "Tag $TAG criada localmente"