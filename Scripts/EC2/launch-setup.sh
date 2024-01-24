#!/usr/bin/env bash

SCRIPT_PATH=${BASH_SOURCE[0]}
while [ -h "$SCRIPT_PATH" ]; do # resolve $SCRIPT_PATH until the file is no longer a symlink
  SCRIPT_DIR=$( cd -P "$( dirname "$SCRIPT_PATH" )" >/dev/null 2>&1 && pwd )
  SCRIPT_PATH=$(readlink "$SCRIPT_PATH")
  [[ $SCRIPT_PATH != /* ]] && SCRIPT_PATH=$SCRIPT_DIR/$SCRIPT_PATH # if $SCRIPT_PATH was a relative symlink, we need to resolve it relative to the path where the symlink file was located
done
SCRIPT_DIR=$( cd -P "$( dirname "$SCRIPT_PATH" )" >/dev/null 2>&1 && pwd )

# exit when any command fails
set -e
# keep track of the last executed command
trap 'last_command=$current_command; current_command=$BASH_COMMAND' DEBUG
# echo an error message before exiting
trap 'echo "\"${last_command}\" command filed with exit code $?."' EXIT

sudo apt-get update
sudo apt-get install -y p7zip-full net-tools python3-pip nginx-core mlocate awscli certbot python3-certbot-nginx git-lfs unzip

echo Downloading configs and placing at ~/code/configs [WITH OVERWRITE]
rm -rf ~/code/configs
wget https://bread.mrskeltal.io/configz/configs.7z
7za x -p"gratify freedom blitz" ./configs.7z
mv ./configs ~/code/

echo Setting the default editor to vim.basic
sudo update-alternatives --set editor /usr/bin/vim.basic

echo Installing pip3 packages
sudo pip3 install xkcdpass pexpect

echo Enabling rc.local
RC_LOCAL_SVC="
[Unit]
 Description=/etc/rc.local Compatibility
 ConditionPathExists=/etc/rc.local

[Service]
 Type=forking
 ExecStart=/etc/rc.local start
 TimeoutSec=0
 StandardOutput=tty
 RemainAfterExit=yes
 SysVStartPriority=99

[Install]
 WantedBy=multi-user.target
"
echo $RC_LOCAL_SVC | sudo tee -a /etc/rc.local > /dev/null
sudo touch /etc/rc.local
sudo chmod +x /etc/rc.local

echo Creating links
cfg="code/configs"
rm -r ~/.addendum ~/.fonts ~/.gitconfig ~/.vim ~/.vimrc
ln -s $cfg/bash/bashrc_addendum ~/.addendum
ln -s $cfg/fonts ~/.fonts
ln -s $cfg/dot_gitconfig ~/.gitconfig
ln -s $cfg/vim/dot_vim ~/.vim
ln -s $cfg/vim/dot_vimrc ~/.vimrc
echo "import numpy as np" > ~/.pythonrc

echo Installing Microsoft Signing Key
wget https://packages.microsoft.com/config/ubuntu/20.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
rm packages-microsoft-prod.deb

echo Installing .NET SDK
sudo apt-get update; \
  sudo apt-get install -y apt-transport-https && \
  sudo apt-get update && \
  sudo apt-get install -y dotnet-sdk-6.0

echo Installing .NET Runtime
sudo apt-get update; \
  sudo apt-get install -y apt-transport-https && \
  sudo apt-get update && \
  sudo apt-get install -y aspnetcore-runtime-6.0

echo Installing PowerShell Dependencies
sudo apt-get update
sudo apt-get install -y wget apt-transport-https software-properties-common

echo Download/register the Microsoft repository GPG keys
wget -q https://packages.microsoft.com/config/ubuntu/20.04/packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb

echo Install PowerShell
sudo apt-get update
sudo apt-get install -y powershell

echo "Subsystem       powershell /bin/pwsh --sshs -NoLogo" | sudo tee -a /etc/ssh/sshd_config > /dev/null

sudo addgroup webdev
sudo adduser ubuntu webdev
sudo usermod -a -G webdev www-data
