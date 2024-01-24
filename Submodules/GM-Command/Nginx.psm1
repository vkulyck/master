Function New-MainNginxConfig {
	param(
		[String[]]$Projects
	)
	Remove-Item "$global:NginxConfigRoot\*.nginx","$global:NginxConfigRoot\main.conf" -ErrorAction Ignore
	$includes = "";
	foreach($project in $Projects) {
		$projectSitePath = "$global:NginxConfigRoot\$project.nginx"
		$includes += "`n`t include '$projectSitePath';".Replace('\', '/')
	}
	$mainConfig = "
		worker_processes 1;
		events {
			worker_connections  1024;
		}
		http {
			$includes
		}".Replace("`n`t`t", "`n")
	;
	$mainConfig | Set-Content "$global:NginxConfigRoot\main.conf"
}

function Write-ProjectNginxConfig {
	param(
		[String]$ProjectName,
		[String]$ProjectCertPath,
		[String]$ProjectDomain,
		[int[]]$ProjectPorts,
		[String]$CrtCertPath,
		[String]$RsaCertPath
	)
	$ProjectSubdomain = $($ProjectDomain -replace '\..*$').ToLower()

	$accessLog = "$global:NginxLogRoot/$ProjectSubdomain-access.log" -replace '\\','/'
	$errorLog = "$global:NginxLogRoot/$ProjectSubdomain-error.log" -replace '\\','/'

	$serverConfig = "
		server {
			server_name $ProjectDomain;
			listen 443 ssl;
			location / {		
				access_log '$accessLog' combined;
				error_log '$errorLog' warn;
				proxy_set_header X-Real-IP `$remote_addr;
				proxy_set_header X-Forwarded-For `$proxy_add_x_forwarded_for;
				proxy_set_header X-Forwarded-Proto `$scheme;
				proxy_set_header Host `$http_host;
				proxy_set_header X-NginX-Proxy true;
				proxy_set_header User-Agent `$http_user_agent;
				proxy_set_header Accept `$http_accept;
				proxy_connect_timeout       600;
				proxy_send_timeout          600;
				proxy_read_timeout          600;
				send_timeout                600;
				proxy_pass http`://$ProjectDomain`:$($ProjectPorts[0])/;
				proxy_redirect off;
			}
			ssl_certificate '$CrtCertPath';
			ssl_certificate_key '$RsaCertPath';
		}

		server {
			server_name $ProjectDomain;
			listen 80;
			if (`$host = $ProjectDomain) {
				return 307 https://`$host`$request_uri;
			}
			return 404;
		}".Replace("`n`t`t", "`n").Replace("'", '"')
	$serverConfig | Set-Content "$global:NginxConfigRoot\$ProjectName.nginx"
}