using MassTransit;
using Microsoft.EntityFrameworkCore;
using Moq;
using PBCartaoCredito.Consumers;
using PBCartaoCredito.Data;
using ParanaBanco.Messages;
using Xunit;

namespace PBCartaoCredito.Tests.Consumers;

public class PropostaGeradaConsumerTests
{
    private AppDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task Consume_NaoDeveEmitirCartao_QuandoStatusForNegado()
    {
        var dbContext = GetDbContext();
        var consumer = new PropostaGeradaConsumer(dbContext);
        var clienteId = Guid.NewGuid();
        
        var contextMock = new Mock<ConsumeContext<PropostaGerada>>();
        contextMock.Setup(x => x.Message).Returns(new PropostaGerada(clienteId, Guid.NewGuid(), 50, "Negado"));

        await consumer.Consume(contextMock.Object);

        var cartoes = await dbContext.Cartoes.Where(c => c.ClienteId == clienteId).ToListAsync();
        Assert.Empty(cartoes);
    }

    [Fact]
    public async Task Consume_DeveEmitirUmCartao_QuandoScoreForAte500()
    {
        var dbContext = GetDbContext();
        var consumer = new PropostaGeradaConsumer(dbContext);
        var clienteId = Guid.NewGuid();
        
        var contextMock = new Mock<ConsumeContext<PropostaGerada>>();
        contextMock.Setup(x => x.Message).Returns(new PropostaGerada(clienteId, Guid.NewGuid(), 300, "Aprovado"));

        await consumer.Consume(contextMock.Object);

        var cartoes = await dbContext.Cartoes.Where(c => c.ClienteId == clienteId).ToListAsync();
        Assert.Single(cartoes);
        Assert.Equal(500m, cartoes.First().Limite);
    }

    [Fact]
    public async Task Consume_DeveEmitirDoisCartoes_QuandoScoreForMaiorQue500()
    {
        var dbContext = GetDbContext();
        var consumer = new PropostaGeradaConsumer(dbContext);
        var clienteId = Guid.NewGuid();
        
        var contextMock = new Mock<ConsumeContext<PropostaGerada>>();
        contextMock.Setup(x => x.Message).Returns(new PropostaGerada(clienteId, Guid.NewGuid(), 800, "Aprovado"));

        await consumer.Consume(contextMock.Object);

        var cartoes = await dbContext.Cartoes.Where(c => c.ClienteId == clienteId).ToListAsync();
        Assert.Equal(2, cartoes.Count);
    }
}