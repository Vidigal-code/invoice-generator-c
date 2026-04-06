# Serviços de Infraestrutura

A camada de **Infraestrutura** no projeto Invoice Generator C# tem foco no isolamento de dependências externas em relação ao domínio central.

## Acesso a Dados
O Entity Framework Core é o ORM principal.
- **Repositórios**: Usamos o padrão de repositório genérico em combinação com interfaces específicas para encapsular o `DbContext`, evitando forte acoplamento nos controladores ou serviços.

## Cache
- **Cache Redis**: Uma camada de cache distribuída está integrada para garantir que as consultas de dados de alto volume e repetição escalem bem, protegendo a performance do banco de dados SQL primário.

## Integrações Externas
- **Serviços Rest**: Aproveitando padrões de `HttpClient` tipados, estabelecemos conexões com provedores fintech externos (como o SocRestService) de forma confiável, encapsulando chamadas com lógicas de retentativa/circuit-breaker via Polly sempre que necessário.
