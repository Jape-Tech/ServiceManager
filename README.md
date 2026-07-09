# JaPe.ServiceManager

Windows Service management made easy.

> **Status:** Early development (experimental)

## Overview

JaPe.ServiceManager is a command-line utility for creating, configuring and maintaining Windows services.

It was created to simplify deployment and automation scenarios where the built-in Windows tools (`sc.exe`, PowerShell cmdlets, or the Services MMC) become cumbersome or insufficient.

The project focuses on reliable, repeatable service configuration that can be fully automated and version controlled.

## Goals

- Create and update Windows services
- Configure service accounts
- Manage service permissions
- Configure per-service environment variables
- Synchronize service configuration from YAML or XML files
- Idempotent operation suitable for CI/CD pipelines
- Script-friendly command-line interface

## Why?

Managing Windows services programmatically often requires combining multiple tools:

- `sc.exe`
- PowerShell cmdlets
- Registry modifications
- Win32 API calls
- Service security descriptors
- Manual configuration steps

JaPe.ServiceManager aims to provide a single, consistent interface for all of these tasks.

## Roadmap

- ⏳ Create services
- ⏳ Update existing services 
- ⏳ Remove services
- ⏳ Start / stop / restart
- ⏳ Configure failure actions
- ⏳ Configure dependencies
- ⏳ Configure delayed auto start
- ⏳ Set service SID type
- ⏳ Configure environment variables
- ⏳ Export/import service configuration
- ⏳ Synchronize services from YAML
- ⏳ Dry-run mode

## Vision

The long-term goal is to make Windows service deployment as straightforward as container deployment.

A complete service definition should fit into a simple YAML file:

```yaml
service:
  name: PrintServer 
  executable: D:\App\ServerService\ServerService.exe
  arguments:
    - D:\App\SERVER\Server.exe
    - mode=Print

  account:
    type: gmsa
    name: DOMAIN\svc-account$

  environment:
    LOGDIR: D:\Logs\Server
    TMPDIR: D:\Temp

  startup: automatic
```

and be applied with a single command:

```bash
japesm apply services.yaml
```

