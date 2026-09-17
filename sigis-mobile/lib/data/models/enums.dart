import 'package:json_annotation/json_annotation.dart';

/// Prioridade de um item na fila de atendimento.
///
/// Espelha o enum `QueuePriority` do backend — o JSON traz o nome do
/// enum C# (`entry.Priority.ToString()`), em ingles e PascalCase, nao um
/// token em portugues.
enum QueuePriority {
  /// Prioridade maxima — corresponde a `Urgent` no backend.
  @JsonValue('Urgent')
  urgent,

  /// Prioridade intermediaria — corresponde a `ShortTerm` no backend.
  @JsonValue('ShortTerm')
  shortTerm,

  /// Prioridade normal — corresponde a `WaitingList` no backend.
  @JsonValue('WaitingList')
  waitingList,
}

/// Status de um item na fila de atendimento.
///
/// Espelha o enum `QueueStatus` do backend (`entry.Status.ToString()`).
enum QueueStatus {
  /// Aguardando atendimento — corresponde a `Waiting`.
  @JsonValue('Waiting')
  waiting,

  /// Em atendimento no momento — corresponde a `InAttendance`.
  @JsonValue('InAttendance')
  inAttendance,

  /// Atendimento concluido — corresponde a `Completed`.
  @JsonValue('Completed')
  completed,

  /// Paciente faltou — corresponde a `Absent`.
  @JsonValue('Absent')
  absent,

  /// Em busca ativa pela equipe — corresponde a `ActiveSearch`.
  @JsonValue('ActiveSearch')
  activeSearch,
}

/// Status de comparecimento de um atendimento agendado.
///
/// Espelha o enum `AttendanceStatus` do backend
/// (`attendance.Status.ToString()`) — usado para DECODIFICAR respostas.
/// Ao ENVIAR comparecimento (`PATCH .../comparecimento`), o backend
/// espera os tokens "COMPARECEU"/"FALTOU" em vez disso — ver
/// `_attendanceStatusWireValue` em `attendance_repository.dart`.
enum AttendanceStatus {
  /// Atendimento agendado, ainda sem confirmacao — corresponde a
  /// `Scheduled`.
  @JsonValue('Scheduled')
  scheduled,

  /// Paciente compareceu — corresponde a `Attended`.
  @JsonValue('Attended')
  attended,

  /// Paciente faltou — corresponde a `Absent`.
  @JsonValue('Absent')
  absent,
}

/// Tipo de sessao/ficha registrada em um atendimento.
///
/// Espelha o enum `SessionType` do backend
/// (`attendance.SessionType.ToString()`).
enum SessionType {
  /// Anamnese psicologica (NAPE) — corresponde a
  /// `PsychologicalAnamnesis`.
  @JsonValue('PsychologicalAnamnesis')
  psychologicalAnamnesis,

  /// Anamnese psicopedagogica (NAPE) — corresponde a
  /// `PsychopedagogicalAnamnesis`.
  @JsonValue('PsychopedagogicalAnamnesis')
  psychopedagogicalAnamnesis,

  /// Instrumental de educacao fisica (NAPE) — corresponde a
  /// `PhysicalEducationInstrument`.
  @JsonValue('PhysicalEducationInstrument')
  physicalEducationInstrument,

  /// Prontuario NASF — corresponde a `NasfRecord`.
  @JsonValue('NasfRecord')
  nasfRecord,

  /// Sintese de acompanhamento (NAPE) — corresponde a `Synthesis`.
  @JsonValue('Synthesis')
  synthesis,
}

/// Extensoes de rotulo em portugues para os enums de dominio, usadas na
/// interface do usuario.
extension QueuePriorityLabel on QueuePriority {
  /// Rotulo em portugues exibido em badges e listas.
  String get label => switch (this) {
        QueuePriority.urgent => 'Urgente',
        QueuePriority.shortTerm => 'Curto prazo',
        QueuePriority.waitingList => 'Lista de espera',
      };
}

/// Extensao de rotulo em portugues para [QueueStatus].
extension QueueStatusLabel on QueueStatus {
  /// Rotulo em portugues exibido em badges e listas.
  String get label => switch (this) {
        QueueStatus.waiting => 'Aguardando',
        QueueStatus.inAttendance => 'Em atendimento',
        QueueStatus.completed => 'Concluido',
        QueueStatus.absent => 'Faltou',
        QueueStatus.activeSearch => 'Busca ativa',
      };
}

/// Extensao de rotulo em portugues para [SessionType].
extension SessionTypeLabel on SessionType {
  /// Rotulo em portugues exibido em formularios e listas.
  String get label => switch (this) {
        SessionType.psychologicalAnamnesis => 'Anamnese Psi',
        SessionType.psychopedagogicalAnamnesis => 'Anamnese Psicoped',
        SessionType.physicalEducationInstrument =>
          'Instrumental Ed. Fisica',
        SessionType.nasfRecord => 'Prontuario NASF',
        SessionType.synthesis => 'Sintese',
      };
}
