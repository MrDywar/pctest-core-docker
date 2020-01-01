# .Net core + docker test app.

1. Create a console application that must perform the following actions:
- Add an entry to the clickhouse
- Get the same entry from the clickhouse
- Put this entry in aerospike
- Read the record of their aerospike
- Put this entry in the redis
- Convert recording to proto-model
- Send protomodel via nats to the second application

2. Create a second console application that should:
- Accept models through nats from the first application
- Get the model from redis and compare it with the data coming from nats

As a model, we use "User":
- Id long
- Name string
- Age int

Used:
- .net core 3.1
- docker (clickhouse, aerospike, redis, nats)

How to run:
- set docker ip address in "appsettings.json". For docker desktop - localhost, for docker toolbox - see you configuration (default is 192.168.99.101).
- open command promt on "PCTest\src\PCTest.ClientA\Docker" and execute "docker-compose up". Ctrl+C to stop.
- run ClientB console.
- run ClientA console.