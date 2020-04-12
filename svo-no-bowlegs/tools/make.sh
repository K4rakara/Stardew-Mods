echo "Compiling tools..."
tsc
echo "Compiling JSON..."
node ./tools/make-json.js ./src/content.jsonc ./src/manifest.json
