#!/bin/bash

# **** Get the DocFx tools ****
# Use package manager like apt-get, yum, or brew to install DocFx tools
dotnet tool install DocFxTocGenerator -g
dotnet tool install DocLanguageTranslator -g
dotnet tool install DocLinkChecker -g
dotnet tool install DocFxOpenApi -g

# **** Check for markdown errors
echo "Checking markdown syntax"
DocLinkChecker -d references/*.md
if [ $? -eq 1 ]; then
  echo "*** ERROR ***"
  echo "An error occurred in markdown syntax checking."
  exit 1
fi

# **** Check the docs folder. On errors, quit processing
echo "Checking references and attachments"
DocLinkChecker -d ./references -a
if [ $? -eq 1 ]; then
  echo "*** ERROR ***"
  echo "An error occurred in checking references and attachments."
  exit 1
fi

# **** Generate the table of contents
echo "Generating table of contents for General"
DocFxTocGenerator -d ./references -sri
if [ $? -eq 1 ]; then
  echo "*** ERROR ***"
  echo "An error occurred in generating table of contents for General."
  exit 1
fi

echo "Generating table of contents for Services"
DocFxTocGenerator -d ./services -sri
if [ $? -eq 1 ]; then
  echo "*** Warning ***"
  echo "An error occurred in generating table of contents for Services."
fi

# **** Clean up old generated files
echo "Clean up previous generated contents"
[ -d docs/_site ] && rm -rf docs/_site
[ -d docs/_pdf ] && rm -rf docs/_pdf

# **** Generate the website
echo "Generating website in _site"
docfx ./docs/docfx.json "$1"

exit 0
