#!/usr/bin/env pwsh

<#
.SYNOPSIS

  Updates the Route53 mapping to the instance using the instance's hostname and (ephemeral) public IP address:
    {HOSTNAME}.{GM_SERVER_ENV}.goodmojo.io => {INSTANCE_PUBLIC_IP}

.EXAMPLE

  Suppose a server in the Team environment has been shutdown and then started in the EC2 web console. The server's
  new public IP address is 13.57.1.100 and a static internal IP address of 10.0.4.33 =~ 0x0A000421, and it has a
  tag with key='GM_SERVER_ENV' and value='team'. Based on the internal IP , the server's hostname is team0421. When
  this script is run on the server, it will upsert a DNS 'A' record with the following parameters:

    Hostname = api0421.team.goodmojo.io
    IP Address = 13.57.1.100

  This script is designed to be called without parameters at the end of the boot process via `/etc/rc.local`
#>

. "$PSScriptRoot/init.ps1"

Update-InstanceDns