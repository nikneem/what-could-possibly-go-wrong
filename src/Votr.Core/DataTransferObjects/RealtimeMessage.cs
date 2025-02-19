namespace Votr.Core.DataTransferObjects;

public record RealtimeMessage<TPayload>(string MessageType, TPayload Payload) where TPayload: class;