import 'package:freezed_annotation/freezed_annotation.dart';

part 'pending_operation.freezed.dart';

/// Tipo de operacao pendente de sincronizacao.
enum PendingOperationType {
  /// Confirmacao de comparecimento ou falta.
  checkin,

  /// Registro de sessao rapida.
  session,
}

/// Status de sincronizacao de uma operacao pendente.
enum PendingOperationStatus {
  /// Aguardando envio ao servidor.
  pending,

  /// Envio em andamento.
  syncing,

  /// Envio falhou (sera retentado).
  failed,

  /// Servidor rejeitou por conflito — exige revisao humana.
  conflict,

  /// Sincronizada com sucesso.
  synced,
}

/// Operacao de escrita pendente de sincronizacao, armazenada na tabela
/// `pending_operations` do SQLite local.
///
/// Toda escrita do app (check-in, sessao rapida) passa primeiro por
/// aqui, garantindo funcionamento 100% offline (ver [SyncService]).
@freezed
abstract class PendingOperation with _$PendingOperation {
  /// Cria uma operacao pendente.
  const factory PendingOperation({
    /// Identificador local unico da operacao (gerado com `uuid`).
    required String id,

    /// Tipo da operacao (check-in ou sessao rapida).
    required PendingOperationType operationType,

    /// Corpo da operacao serializado em JSON, pronto para envio a API.
    required String payload,

    /// Instante local de criacao da operacao.
    required DateTime createdAt,

    /// Numero de tentativas de sincronizacao ja realizadas.
    @Default(0) int retryCount,

    /// Status atual de sincronizacao.
    @Default(PendingOperationStatus.pending) PendingOperationStatus status,

    /// Mensagem de erro da ultima tentativa, quando houver.
    String? errorMessage,

    /// Identificador atribuido pelo servidor apos sincronizacao bem
    /// sucedida.
    String? serverId,
  }) = _PendingOperation;
}
