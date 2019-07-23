Run Montage M101 Use Case on Local OpenWhisk
===

This document shows the overhead incurred in accessing the external storage to store the state of a task in the serverless environment described in the poster "Implementing Efficient State Maintenance in Serverless-based Workflow Using Host Storage" submitted to SC'19.

Table of Contents
===
- [Run Montage M101 Use Case on Local OpenWhisk](#run-montage-m101-use-case-on-local-openwhisk)
- [Table of Contents](#table-of-contents)
- [Prerequisites](#prerequisites)
- [Getting Started](#getting-started)
  - [Install OpenWhisk on Docker Compose](#install-openwhisk-on-docker-compose)
  - [Build Activities](#build-activities)
  - [Compose Workflow](#compose-workflow)
  - [Open Storage Server](#open-storage-server)
  - [Run Workflow](#run-workflow)
  - [Get Result](#get-result)
- [Result](#result)

Prerequisites
===
>This demonstration is conducted on a local machine for experimental purposes.
>We have not confirmed execution in a commercial cloud environment.

>We conduct demonstration on Ubuntu 19.04 LTS x64


- [Node.js](https://nodejs.org/ko/) 10+
  - To run storage server.
- [OpenWhisk Devtools](https://github.com/apache/incubator-openwhisk-devtools)
  - [Docker](https://github.com/apache/incubator-openwhisk-devtools/blob/master/docker-compose/README.md) 1.13+
  - [Docker Compose](https://github.com/apache/incubator-openwhisk-devtools/blob/master/docker-compose/README.md) 1.6+
- [OpenWhisk Composer](https://github.com/apache/incubator-openwhisk-composer)
- [.Net Core](https://dotnet.microsoft.com/download/linux-package-manager/ubuntu16-04/sdk-current) 2.2+
  - To compile activities.
- [lesomnus/wsk-dotnet-montage on Docker Hub](https://hub.docker.com/r/lesomnus/wsk-dotnet-montage)
  - Runtime for activities.

Getting Started
===

> The line begins with `$` indicates the command you enter.

> The line begins with `>` indicates the output of the command.

## Install OpenWhisk on Docker Compose

Ensure that you installed the `Docker` and `Docker Compose`.
```sh
$ docker --version
> Docker version 18.09.6, build 481bc77
$ docker-compose --version
> docker-compose version 1.24.0, build 0aa59064
```

If you clone the repository [OpenWhisk Devtools](https://github.com/apache/incubator-openwhisk-devtools), you can see the folder `docker-compose` in git repository root directory. In the `docker-compose`, execute `make quick-start`. See details on [How to setup OpenWhisk with Docker Compose](https://github.com/apache/incubator-openwhisk-devtools/blob/master/docker-compose/README.md)

```sh
$ cd docker-compose
$ make quick-start
> ...
> ok: updated action hello
> invoking http://YOUR_MACHIN_IP/api/.../hello/world
>   "payload": "Hello, World!"
> ok: APIs
> ...
> To use the wsk CLI: export WSK_CONFIG_FILE=...
                      or copy the file to /home/USERNAME/.wskprops
$ export WSK_CONFIG_FILE=...
```
If installation was successful, you can see the `"paload": "Hello, World!"`. To For convenience, copy the `export WSK_CONFIG_FILE` command from the end of the output and enter it.

## Build Activities

If you installed .Net core and configured `WSK_CONFIG_FILE` properly, you can use `dotnet` and `wsk` commands.

``` sh
$ dotnet --version
> 2.2.300
$ wsk --help
> [OpenWhisk logo]
> Usage:
>   wsk [command]
> ...
```

If you cloned this repository, you can see the files `Step*.cs`. Theses files are activities we demonstrate. You need to build it and register to OpenWhisk but we have already created a script `build.sh`. Before the build, ensure that you pulled the [lesomnus/wsk-dotnet-montage](https://hub.docker.com/r/lesomnus/wsk-dotnet-montage).

```sh
$ docker pull lesomnus/wsk-dotnet-montage # Enter if needed
$ cd ~ # Enter if needed
$ git clone https://github.com/KU-Cloud/wskMontageM101.git
$ cd wskMontageM101
./build.sh
> Microsoft (R) Build Engine version ... for .Net Core
> Copy Copyright (C) Microsoft Corporation. All rights reserved.
> Restore completed in ...ms for ~/wskMontageM101.csproj.
> ...
> ok: updated action Step1
> ok: updated action Step2
> ...
> ok: updated action Step11
```

## Compose Workflow

If you installed [OpenWhisk Composer](https://github.com/apache/incubator-openwhisk-composer), you can use `compose` and `deploy` commands.
```sh
$ compose --version
> 0.11.0
$ deploy --version
> 0.11.0
```

The workflow specification `workflow.js` is in the root directory of this repository.
```sh
$ compose workflow.js > workflow.json
$ deploy -i --kind nodejs:6 m101 workflow.json -w
```
## Open Storage Server

If you installed [Node.js](https://nodejs.org/ko/), you can use `node` and `npm` commands.
```sh
$ node --vresion
> v10.15.3
$ npm --vresion
> 6.4.1
```

The server resources are in the `server` directory in the root directory of this repositry.

> We recommended to run storage server on another machine to reproduce serverless environment. Remind that the storage server is outside the worker node.

You should install the its dependencies before run it.
``` sh
$ npm install
> ...
$ node index.js
> listening...
```
The default port of the server is `5122`. If you want to open another port, modify the value of `PORT` in file `index.js` first line.

## Run Workflow
Before run the workflow, you need to specify the address of storage server. Open file `param.json` in the root directory of this repository and modify the value of `serverURL` field to the address of your storage server. Also, since composer requires `redis`, you need to specify `redis` address. The `redis` server lives on the local with `OpenWhisk`, it is fine to enter the IP of your machine.

The command below will start M101.
``` sh
$ wsk -i action invoke m101 -P param.json
> ok: invoked /_/m101 with id INVOKE_ID
```

You should remember the `INVOKE_ID` to get result

## Get Result
You can get the result using `wsk` command.
```sh
$ wsk -i activation result INVOKE_ID > logs.json
```
The `INVOKE_ID` indicates the id that was output when invoking m101 with the `wsk -i action ...` command. You can see the sample logs.json in [result](./result/logs.json).

Result
===
If you run the storage server on the another machine, the data transmission speed will limited to the machine network equipment. We conducted this demonstration on 100Mbps LAN.

The table below shows the time spent by each activity.

| Activity   | Download<br>required state | Process | Upload<br>updated state | Total execution time |
| ---------- | -------------------------- | ------- | ----------------------- | -------------------- |
| STEP 1     | 1884                       | 62      | 56                      | 2002                 |
| STEP 2     | 92                         | 6       | 0                       | 98                   |
| STEP 3 #1  | 266                        | 910     | 759                     | 1935                 |
| STEP 3 #2  | 182                        | 893     | 1363                    | 2438                 |
| STEP 3 #3  | 184                        | 892     | 1447                    | 2523                 |
| STEP 3 #4  | 254                        | 934     | 768                     | 1956                 |
| STEP 3 #5  | 188                        | 894     | 748                     | 1830                 |
| STEP 3 #6  | 184                        | 890     | 771                     | 1845                 |
| STEP 3 #7  | 330                        | 894     | 1324                    | 2548                 |
| STEP 3 #8  | 211                        | 894     | 1531                    | 2637                 |
| STEP 3 #9  | 240                        | 893     | 1002                    | 2135                 |
| STEP 3 #10 | 274                        | 891     | 952                     | 2117                 |
| STEP 4     | 3654                       | 65      | 39                      | 3759                 |
| STEP 5     | 106                        | 380     | 35                      | 521                  |
| STEP 6     | 90                         | 4       | 0                       | 94                   |
| STEP 7 #1  | 4140                       | 52      | 97                      | 4289                 |
| STEP 7 #2  | 4122                       | 53      | 99                      | 4274                 |
| STEP 7 #3  | 3401                       | 45      | 47                      | 3493                 |
| STEP 7 #4  | 4174                       | 48      | 50                      | 4272                 |
| STEP 7 #5  | 3698                       | 51      | 94                      | 3843                 |
| STEP 7 #6  | 5181                       | 47      | 57                      | 5285                 |
| STEP 7 #7  | 1898                       | 54      | 125                     | 2077                 |
| STEP 7 #8  | 4209                       | 52      | 125                     | 4386                 |
| STEP 7 #9  | 4118                       | 45      | 43                      | 4206                 |
| STEP 7 #10 | 2033                       | 58      | 97                      | 2189                 |
| STEP 7 #11 | 3696                       | 52      | 104                     | 3852                 |
| STEP 7 #12 | 4899                       | 47      | 48                      | 4994                 |
| STEP 7 #13 | 4071                       | 48      | 50                      | 4169                 |
| STEP 7 #14 | 3422                       | 48      | 67                      | 3537                 |
| STEP 7 #15 | 4212                       | 56      | 120                     | 4388                 |
| STEP 7 #16 | 4199                       | 45      | 63                      | 4307                 |
| STEP 7 #17 | 3421                       | 47      | 36                      | 3504                 |
| STEP 8     | 581                        | 884     | 31                      | 1496                 |
| STEP 9     | 74                         | 833     | 34                      | 942                  |
| STEP 10    | 7287                       | 1829    | 7244                    | 16361                |
| STEP 11    | 7244                       | 1042    | 3416                    | 11703                |

The total execution time of each activities is `126005ms`

The total upload/download time during the activity execution is `111061ms`

The overhead caused by accessing external storage is `126005/11061` = `88.1402%`

If Applied our Proposed System
---
The total execution time of each activities will be `20238ms`

The total upload/download time during the activity execution is `5300ms`

The overhead caused by accessing external storage is `20238/5300` = `26.1884%`
