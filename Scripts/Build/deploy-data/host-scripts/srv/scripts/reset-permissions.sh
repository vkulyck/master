#!/usr/bin/env bash

sudo chown -R root:webdev /srv/
find /srv -type f -print0 | sudo xargs -0 chmod 0664
find /srv -type d -print0 | sudo xargs -0 chmod 0775
find /srv/scripts -type f \( -name '*.ps1' -o -name '*.sh' -o -name 'gm*' \) -print0 | sudo xargs -0 chmod 0775