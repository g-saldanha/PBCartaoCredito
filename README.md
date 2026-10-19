# 🏦 PBCartaoCredito - Microsserviço de Cartões

## 🛠 Tecnologias Utilizadas
- **.NET 10.0** (ASP.NET Core Web API)
- **Entity Framework Core** (In-Memory Database para ambiente local)
- **MassTransit + RabbitMQ** (Consumo de eventos)
- **xUnit + Moq** (Testes de Unidade)
- **Asp.Versioning.Http** (Versionamento de API)

## 🏗 Regras de Negócio Implementadas
A emissão dos cartões segue estritamente a seguinte matriz baseada no *Score* da proposta:
- **0 a 100 (Negado):** Nenhum cartão é emitido.
- **101 a 500 (Aprovado):** Emissão de 1 cartão com limite de **R$ 1.000,00**.
- **501 a 1000 (Aprovado):** Emissão de 2 cartões com limite de **R$ 5.000,00** cada (totalizando R$ 10.000,00 de crédito disponibilizado).

## 📡 Mensageria (Eventos)
- **Consome / Ouve:** `PropostaGerada` (Fila: `fila-proposta-gerada`)
    - *Payload:* `ClienteId`, `PropostaId`, `Score`, `Status`
    - *Namespace global:* `ParanaBanco.Messages`

## 🚀 Como Executar Localmente

### Pré-requisitos
- [.NET 10 SDK](https://dotnet.microsoft.com/)
- Ter o **RabbitMQ** rodando e os serviços anteriores (MS1 e MS2) operantes para que o fluxo completo aconteça.

### Passos para Execução

1. **Clone e inicie a Aplicação:**
   Navegue até a pasta raiz deste microsserviço e execute:
   ```bash
   dotnet run
   
### Fluxograma

![mermaid fluxograma.png](mermaid%20fluxograma.png)