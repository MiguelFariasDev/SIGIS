import 'package:sigis_mobile/data/local/models/pending_operation.dart';

/// Resolve conflitos de sincronizacao usando "last-write-wins" pelo
/// timestamp local do dispositivo, sempre mantendo uma flag de revisao
/// humana.
///
/// Regra critica do projeto: conflito NUNCA e resolvido de forma
/// silenciosa. Mesmo quando o timestamp do dispositivo define qual
/// escrita e considerada "vencedora" para fins de reenvio, a operacao e
/// marcada com [PendingOperationStatus.conflict] e permanece visivel na
/// tela de sincronizacao (F07) ate que um profissional revise.
class ConflictResolver {
  /// Cria o resolvedor de conflitos.
  const ConflictResolver();

  /// Marca [operation] como conflito, preservando o timestamp local
  /// original (usado para a comparacao last-write-wins) e anexando a
  /// [reason] retornada pelo servidor.
  PendingOperation markAsConflict(PendingOperation operation, String reason) {
    return operation.copyWith(
      status: PendingOperationStatus.conflict,
      errorMessage: reason,
    );
  }

  /// Decide se [incoming] deveria prevalecer sobre [existing] com base
  /// no timestamp local de criacao (last-write-wins).
  ///
  /// Usado apenas como criterio auxiliar de exibicao — a fusao efetiva
  /// sempre exige confirmacao humana na tela de sincronizacao.
  bool shouldPreferIncoming(
    PendingOperation existing,
    PendingOperation incoming,
  ) =>
      incoming.createdAt.isAfter(existing.createdAt);
}
