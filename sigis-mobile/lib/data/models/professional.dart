import 'package:freezed_annotation/freezed_annotation.dart';

part 'professional.freezed.dart';
part 'professional.g.dart';

/// Papel de controle de acesso (RBAC) do profissional.
///
/// O backend retorna o nome do enum C# em ingles e PascalCase (ex.:
/// `"Coordinator"`) no campo `role` de `POST /api/auth/login` — ver
/// `AuthRepository.login`, que converte esse texto para este enum.
enum ProfessionalRole {
  /// Escopo restrito a propria unidade — corresponde a `Professional`.
  professional,

  /// Escopo de rede completa, resolve duplicidades (nao usado no
  /// mobile) — corresponde a `Coordinator`.
  coordinator,

  /// Escopo somente leitura de auditoria (nao usado no mobile) —
  /// corresponde a `Auditor`.
  auditor,
}

/// Representa o profissional autenticado no app.
///
/// Corresponde a entidade `PROFISSIONAL` do backend.
@freezed
abstract class Professional with _$Professional {
  /// Cria um profissional com os dados retornados pela API de login.
  const factory Professional({
    /// Identificador unico do profissional.
    required String id,

    /// Nome do profissional.
    @JsonKey(name: 'nome') required String name,

    /// E-mail/matricula usada para login.
    required String email,

    /// Identificador da unidade de lotacao.
    @JsonKey(name: 'unidadeId') required String unitId,

    /// Sigla da unidade de lotacao (ex.: "NASF").
    @JsonKey(name: 'unidadeSigla') required String unitAcronym,

    /// Papel de controle de acesso do profissional.
    @JsonKey(name: 'papelRbac') required ProfessionalRole role,
  }) = _Professional;

  /// Cria um [Professional] a partir de um mapa JSON retornado pela
  /// API.
  factory Professional.fromJson(Map<String, dynamic> json) =>
      _$ProfessionalFromJson(json);
}
