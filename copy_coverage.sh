#!/bin/sh
set -e
cd BooruSharp.UnitTests/TestResults
ls | cd
cp coverage.cobertura.xml  ../../../coverage.xml
cd ../../..