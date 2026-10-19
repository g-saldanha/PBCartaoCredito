using MassTransit;
using PBCartaoCredito.Data;
using ParanaBanco.Messages;

namespace PBCartaoCredito.Consumers;

public class PropostaGeradaConsumer : IConsumer<PropostaGerada>
{
    private readonly AppDbContext _db;

    public PropostaGeradaConsumer(AppDbContext db)
    {
        _db = db;
    }

    public async Task Consume(ConsumeContext<PropostaGerada> context)
    {
        var mensagem = context.Message;
        Console.WriteLine($"\n[MS3] ---> RECEBEU evento PropostaGerada! ClienteID: {mensagem.ClienteId} | Score: {mensagem.Score} | Status: {mensagem.Status}");

        if (mensagem.Status == "Negado") return;

        var cartoesGerados = new List<Cartao>();

        if (mensagem.Score > 100 && mensagem.Score <= 500)
        {
            cartoesGerados.Add(CriarCartao(mensagem.ClienteId, 500m));
        }
        else if (mensagem.Score > 500)
        {
            cartoesGerados.Add(CriarCartao(mensagem.ClienteId, 1000m));
            cartoesGerados.Add(CriarCartao(mensagem.ClienteId, 2000m));
        }

        if (cartoesGerados.Any())
        {
            _db.Cartoes.AddRange(cartoesGerados);
            await _db.SaveChangesAsync();
            Console.WriteLine($"[MS3] Processamento concluído! {cartoesGerados.Count} cartão(ões) emitido(s) e salvo(s) na base.\n");
        }
    }

    private Cartao CriarCartao(Guid clienteId, decimal limite)
    {
        return new Cartao
        {
            Id = Guid.NewGuid(),
            ClienteId = clienteId,
            Limite = limite,
            FinalCartao = new Random().Next(1000, 9999).ToString()
        };
    }
}