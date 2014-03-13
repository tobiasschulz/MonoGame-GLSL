#!/bin/bash

SCRIPT_PATH="${BASH_SOURCE[0]}";
if ([ -h "${SCRIPT_PATH}" ]) then
  while([ -h "${SCRIPT_PATH}" ]) do SCRIPT_PATH=`readlink "${SCRIPT_PATH}"`; done
fi
pushd . > /dev/null
cd `dirname ${SCRIPT_PATH}` > /dev/null
SCRIPT_PATH=`pwd`;
popd  > /dev/null


$SCRIPT_PATH/internal/astyle-csharp.sh --suffix=none --lineend=linux $(find . -name "*.cs" -or -name "*.template" | egrep -v '(ThirdParty|MonoGame.Framework|2MGFX)')
$SCRIPT_PATH/internal/corrent-empty-lines
