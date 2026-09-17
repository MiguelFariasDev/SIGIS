import 'package:freezed_annotation/freezed_annotation.dart';

part 'cached_patient.freezed.dart';

/// Versao minimizada de uma pessoa, armazenada na tabela
/// `cached_patients` do SQLite local para permitir busca offline.
///
/// Regra de LGPD (minimizacao): esta tabela NUNCA guarda prontuario,
/// historico de atendimentos ou endereco — apenas o essencial para
/// localizar a pessoa e iniciar um check-in.
@freezed
abstract class CachedPatient with _$CachedPatient {
  /// Cria um registro de paciente em cache.
  const factory CachedPatient({
    /// Identificador unico da pessoa.
    required String id,

    /// Nome completo.
    required String name,

    /// Data de nascimento (ISO 8601).
    required DateTime birthDate,

    /// Numero do Cartao Nacional de Saude, quando informado.
    String? cns,

    /// Identificador da unidade associada ao cache, quando aplicavel.
    String? unitId,

    /// Instante em que o registro foi salvo em cache.
    required DateTime cachedAt,
  }) = _CachedPatient;
}
