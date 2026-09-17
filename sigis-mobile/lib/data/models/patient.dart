import 'package:freezed_annotation/freezed_annotation.dart';
import 'package:sigis_mobile/data/models/attendance.dart';

part 'patient.freezed.dart';
part 'patient.g.dart';

/// Representa uma pessoa cadastrada no nucleo de identidade do SIGIS.
///
/// Corresponde a entidade `PESSOA` do backend. No mobile, apenas os
/// campos minimos necessarios para busca e check-in sao mantidos em
/// cache local (ver `CachedPatient`) — este modelo completo e usado
/// apenas em memoria, nunca persistido integralmente em disco.
@freezed
abstract class Patient with _$Patient {
  /// Cria uma pessoa com os dados retornados pela API.
  const factory Patient({
    /// Identificador unico da pessoa.
    required String id,

    /// Nome completo.
    @JsonKey(name: 'name') required String fullName,

    /// Data de nascimento.
    @JsonKey(name: 'birthDate') required DateTime birthDate,

    /// Numero do Cartao Nacional de Saude, quando informado (so presente
    /// no detalhe — a busca nao retorna CNS).
    String? cns,

    /// Nome da mae, quando informado.
    @JsonKey(name: 'motherName') String? motherName,

    /// Identificador da unidade de referencia — o backend nao expoe esse
    /// campo em `Pessoa` (vinculo e via fila/atendimento/encaminhamento),
    /// entao fica sempre `null` quando vindo da API.
    @JsonKey(name: 'unitId', includeToJson: false) String? unitId,

    /// Ultimos atendimentos da pessoa — o backend nao inclui isso no
    /// detalhe da pessoa; usar `GET /api/atendimentos/pessoa/{id}`
    /// separadamente quando precisar.
    @JsonKey(includeFromJson: false, includeToJson: false)
    List<Attendance>? recentAttendances,
  }) = _Patient;

  /// Cria uma [Patient] a partir de um mapa JSON retornado pela API.
  factory Patient.fromJson(Map<String, dynamic> json) =>
      _$PatientFromJson(json);
}
