#!/bin/sh

# Replace placeholders in /usr/share/nginx/html/env.js
echo "Replacing env vars in env.js..."
sed -i "s|REPLACE_TREE_NOTES_SERVICE_URL|${TREE_NOTES_SERVICE_URL:-http://localhost:3000}|g" /usr/share/nginx/html/env.js

# Start nginx
nginx -g 'daemon off;'