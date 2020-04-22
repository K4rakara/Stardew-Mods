const fs = require('fs');

const content = JSON.parse(fs.readFileSync('./src/Unnamed/manifest.json', 'utf8'));

console.log(content.Version);
