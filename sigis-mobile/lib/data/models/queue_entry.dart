import 'package:freezed_annotation/freezed_annotation.dart';
import 'package:sigis_mobile/data/models/enums.dart';

part 'queue_entry.freezed.dart';
part 'queue_entry.g.dart';

/// Representa um item na fila de atendimento de uma unidade.
///
/// Corresponde a entidade `FILA_ATENDIMENTO` do backend.
@freezed
abstract class QueueEntry with _$QueueEntry {
  /// Cria um item de fila com os dados retornados pela API.
  const factory QueueEntry({
    /// Identificador unico do item de fila.
    required String id,

    /// Identificador da pessoa na fila.
    @JsonKey(name: 'personId') required String patientId,

    /// Nome da pessoa na fila (desnormalizado para exibicao rapida).
    @JsonKey(name: 'personName') required String patientName,

    /// Data de nascimento da pessoa — o backend nao inclui isso no
    /// resumo da fila (so nome e id), entao fica `null` quando vindo da
    /// API; so e preenchido ao combinar com um [Patient] ja carregado.
    DateTime? patientBirthDate,

    /// Identificador da unidade da fila.
    @JsonKey(name: 'unitId') required String unitId,

    /// Especialidade/servico da fila (ex.: "NASF", "Psicologia").
    required String specialty,

    /// Prioridade do item na fila.
    required QueuePriority priority,

    /// Instante em que a pessoa entrou na fila.
    @JsonKey(name: 'enteredAt') required DateTime enteredAt,

    /// Status atual do item de fila.
    required QueueStatus status,
  }) = _QueueEntry;

  /// Cria um [QueueEntry] a partir de um mapa JSON retornado pela API.
  factory QueueEntry.fromJson(Map<String, dynamic> json) =>
      _$QueueEntryFromJson(json);
}
