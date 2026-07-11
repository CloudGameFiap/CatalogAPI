# CatalogAPI

1. Build da Imagem da Api

Acessar a pasta raiz e executar o seguinte comando
```Bash
docker build -f .\src\CloudGameCatalog.API\Dockerfile -t gpaschoal/cloudgamefiapcatalogapi:1.0 .
```

Para subir, acessar a pasta src e executar o seguinte comando
```Bash
docker push gpaschoal/cloudgamefiapcatalogapi:1.0
```

2. Build da Imagem do Consumer
```Bash
docker build -f .\src\CloudGameCatalog.Consumer\Dockerfile -t gpaschoal/cloudgamefiapcatalogconsumer:1.0 .
```

Para subir, acessar a pasta src e executar o seguinte comando
```Bash
docker push gpaschoal/cloudgamefiapcatalogconsumer:1.0
```