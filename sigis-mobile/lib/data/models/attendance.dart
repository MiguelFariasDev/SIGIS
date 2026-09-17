import 'package:freezed_annotation/freezed_annotation.dart';
import 'package:sigis_mobile/data/models/enums.dart';

part 'attendance.freezed.dart';
part 'attendance.g.dart';

/// Representa um atendimento (agendado ou realizado) de uma pessoa em
/// uma unidade.
///
/// Corresponde a entidade `ATENDIMENTO` do backend.
@freezed
abstract class Attendance with _$Attendance {
  /// Cria um atendimento com os dados retornados pela API.
  const factory Attendance({
    /// Identificador unico do atendimento.
    required String id,

    /// Identificador da pessoa atendida.
    @JsonKey(name: 'personId') required String patientId,

    /// Identificador da unidade onde ocorre o atendimento.
    @JsonKey(name: 'unitId') required String unitId,

    /// Identificador do profissional responsavel.
    required String professionalId,

    /// Data e hora do atendimento.
    required DateTime dateTime,

    /// Tipo de sessao/ficha registrada.
    required SessionType sessionType,

    /// Status de comparecimento do atendimento.
    required AttendanceStatus status,

    /// Numero sequencial da sessao para a pessoa.
    required int sessionNumber,

    /// Dados especificos do formulario da sessao, como JSON bruto (o
    /// backend guarda/retorna `FormData` como string, nao como objeto
    /// aninhado) — decodifique com `jsonDecode` quando precisar ler.
    String? formData,
  }) = _Attendance;

  /// Cria uma [Attendance] a partir de um mapa JSON retornado pela API.
  factory Attendance.fromJson(Map<String, dynamic> json) =>
      _$AttendanceFromJson(json);
}
