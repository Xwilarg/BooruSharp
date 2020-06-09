#!/bin/sh
set -e
cd BooruSharp.UnitTests/TestResults
cd $(ls)
cp coverage.cobertura.xml  ../../../coverage.xml
cd ../../..