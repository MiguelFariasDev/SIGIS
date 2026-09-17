import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:sigis_mobile/core/providers/core_providers.dart';
import 'package:sigis_mobile/core/result/result.dart';
import 'package:sigis_mobile/data/models/enums.dart';

/// Controller da tela de confirmacao de comparecimento/falta (F05).
///
/// A escrita e sempre salva localmente primeiro (fila de
/// sincronizacao) — funciona 100% offline, ver `AttendanceRepository`
/// e `SyncService`.
class CheckInController extends AutoDisposeAsyncNotifier<void> {
  @override
  Future<void> build() async {}

  /// Confirma o atendimento [attendanceId] com o [status] informado
  /// ([AttendanceStatus.attended] ou [AttendanceStatus.absent]).
  ///
  /// Retorna o [Result] da operacao para que a UI trate sucesso/erro
  /// diretamente (evita usar `ref.listen` para detectar a transicao de
  /// estado, o que disparava falsamente ao assentar o `build()` inicial).
  Future<Result<void>> confirm({
    required String attendanceId,
    required AttendanceStatus status,
  }) async {
    state = const AsyncLoading();

    final result = await ref
        .read(attendanceRepositoryProvider)
        .confirmAttendance(attendanceId: attendanceId, status: status);

    state = switch (result) {
      Success<void>() => const AsyncData(null),
      Failure<void>(error: final error) => AsyncError(error, StackTrace.current),
    };

    return result;
  }
}
