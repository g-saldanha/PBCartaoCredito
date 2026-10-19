namespace PBCartaoCredito.Messages;

public record PropostaGerada(Guid ClienteId, Guid PropostaId, int Score, string Status);