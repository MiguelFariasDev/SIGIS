import 'package:sigis_mobile/data/local/database.dart';
import 'package:sigis_mobile/data/local/models/pending_operation.dart';

/// Acesso a tabela `pending_operations` do SQLite local.
///
/// Toda operacao de escrita do app passa por aqui antes de ser
/// sincronizada com o servidor (ver [SyncService]).
class PendingOperationDao {
  /// Cria o DAO sobre o banco [database].
  PendingOperationDao(this._database);

  final AppDatabase _database;

  /// Insere uma nova operacao pendente.
  Future<void> insert(PendingOperation operation) async {
    final db = await _database.database;
    await db.insert(DbTables.pendingOperations, _toRow(operation));
  }

  /// Lista todas as operacoes pendentes, da mais antiga para a mais
  /// recente.
  Future<List<PendingOperation>> findAll() async {
    final db = await _database.database;
    final rows = await db.query(
      DbTables.pendingOperations,
      orderBy: 'created_at ASC',
    );
    return rows.map(_fromRow).toList();
  }

  /// Lista as operacoes com status [status].
  Future<List<PendingOperation>> findByStatus(
    PendingOperationStatus status,
  ) async {
    final db = await _database.database;
    final rows = await db.query(
      DbTables.pendingOperations,
      where: 'status = ?',
      whereArgs: [status.name],
      orderBy: 'created_at ASC',
    );
    return rows.map(_fromRow).toList();
  }

  /// Atualiza uma operacao existente (status, tentativas, erro,
  /// identificador do servidor).
  Future<void> update(PendingOperation operation) async {
    final db = await _database.database;
    await db.update(
      DbTables.pendingOperations,
      _toRow(operation),
      where: 'id = ?',
      whereArgs: [operation.id],
    );
  }

  /// Remove uma operacao pelo identificador local.
  Future<void> delete(String id) async {
    final db = await _database.database;
    await db.delete(
      DbTables.pendingOperations,
      where: 'id = ?',
      whereArgs: [id],
    );
  }

  /// Conta quantas operacoes ainda nao foram sincronizadas (status
  /// diferente de [PendingOperationStatus.synced]).
  Future<int> countPending() async {
    final db = await _database.database;
    final result = await db.rawQuery(
      'SELECT COUNT(*) as total FROM ${DbTables.pendingOperations} '
      "WHERE status != ?",
      [PendingOperationStatus.synced.name],
    );
    return (result.first['total'] as int?) ?? 0;
  }

  Map<String, Object?> _toRow(PendingOperation operation) => {
        'id': operation.id,
        'operation_type': operation.operationType.name,
        'payload': operation.payload,
        'created_at': operation.createdAt.millisecondsSinceEpoch,
        'retry_count': operation.retryCount,
        'status': operation.status.name,
        'error_message': operation.errorMessage,
        'server_id': operation.serverId,
      };

  PendingOperation _fromRow(Map<String, Object?> row) => PendingOperation(
        id: row['id']! as String,
        operationType: PendingOperationType.values.byName(
          row['operation_type']! as String,
        ),
        payload: row['payload']! as String,
        createdAt: DateTime.fromMillisecondsSinceEpoch(
          row['created_at']! as int,
        ),
        retryCount: row['retry_count']! as int,
        status: PendingOperationStatus.values.byName(row['status']! as String),
        errorMessage: row['error_message'] as String?,
        serverId: row['server_id'] as String?,
      );
}
