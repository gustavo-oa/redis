# Redis

Simple project to connect and use all resources of Redis


## Estrutura

```
Redis                                   --> raiz projeto
├── Repository                          
│   └── RedisRepository                 --> connection with redis
│     
├── Interface                           --> interfaces
│   ├── IRedisModel                     --> Interface used on every model to set, delete, get data from redis 
│
├── Model                               --> Examples models
│
└── Redis.Test                          
    └── RedisRepositoryTest             --> fill test's using real data
```
