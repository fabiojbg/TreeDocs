#!/bin/sh

# Replace placeholders in /usr/share/nginx/html/defaults.js
echo "Replacing defaults vars in defaults.js..."
sed -i "s|REPLACE_TREE_NOTES_SERVICE_URL|${TREE_NOTES_SERVICE_URL:-http://localhost:5100}|g" /usr/share/nginx/html/js/defaults.js

# Start nginx
nginx -g 'daemon off;'