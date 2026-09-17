import 'package:uuid/uuid.dart';
import 'package:sigis_mobile/data/local/dao/pending_operation_dao.dart';
import 'package:sigis_mobile/data/local/models/pending_operation.dart';

const _uuid = Uuid();

/// Fila local de operacoes de escrita pendentes de sincronizacao.
///
/// Encapsula o [PendingOperationDao], oferecendo uma API de fila
/// (enfileirar, listar pendentes, atualizar status) usada tanto pelos
/// repositorios (para enfileirar escritas) quanto pelo [SyncService]
/// (para processar a fila).
class SyncQueue {
  /// Cria a fila sobre o [PendingOperationDao] informado.
  SyncQueue(this._dao);

  final PendingOperationDao _dao;

  /// Adiciona uma nova operacao do tipo [type] com o [payload] JSON
  /// serializado, retornando a operacao criada.
  Future<PendingOperation> enqueue({
    required PendingOperationType type,
    required String payload,
  }) async {
    final operation = PendingOperation(
      id: _uuid.v4(),
      operationType: type,
      payload: payload,
      createdAt: DateTime.now(),
    );
    await _dao.insert(operation);
    return operation;
  }

  /// Lista todas as operacoes pendentes de envio ou reenvio (status
  /// [PendingOperationStatus.pending] ou [PendingOperationStatus.failed]).
  Future<List<PendingOperation>> pendingOperations() async {
    final pending = await _dao.findByStatus(PendingOperationStatus.pending);
    final failed = await _dao.findByStatus(PendingOperationStatus.failed);
    return [...pending, ...failed]
      ..sort((a, b) => a.createdAt.compareTo(b.createdAt));
  }

  /// Lista todas as operacoes, independente do status — usado pela tela
  /// de sincronizacao (F07).
  Future<List<PendingOperation>> all() => _dao.findAll();

  /// Conta as operacoes ainda nao sincronizadas.
  Future<int> countPending() => _dao.countPending();

  /// Atualiza uma operacao existente na fila (mudanca de status,
  /// tentativas, erro ou identificador do servidor).
  Future<void> update(PendingOperation operation) => _dao.update(operation);

  /// Reenvia uma operacao marcada como conflito, voltando-a para
  /// [PendingOperationStatus.pending] apos revisao humana.
  Future<void> retry(PendingOperation operation) => update(
        operation.copyWith(
          status: PendingOperationStatus.pending,
          errorMessage: null,
        ),
      );

  /// Descarta definitivamente uma operacao pendente (ex.: apos revisao
  /// humana decidir nao reenviar um conflito).
  Future<void> discard(String id) => _dao.delete(id);
}
