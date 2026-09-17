import 'package:sqflite/sqflite.dart';
import 'package:sigis_mobile/data/local/database.dart';

/// Acesso a tabela `sync_metadata` do SQLite local.
///
/// Guarda pares chave/valor simples sobre o estado da sincronizacao,
/// como o instante da ultima sincronizacao bem-sucedida.
class SyncMetadataDao {
  /// Cria o DAO sobre o banco [database].
  SyncMetadataDao(this._database);

  final AppDatabase _database;

  /// Chave usada para guardar o instante da ultima sincronizacao.
  static const String lastSyncKey = 'last_sync_at';

  /// Le o valor associado a [key], ou `null` se nao existir.
  Future<String?> read(String key) async {
    final db = await _database.database;
    final rows = await db.query(
      DbTables.syncMetadata,
      where: 'key = ?',
      whereArgs: [key],
      limit: 1,
    );
    if (rows.isEmpty) {
      return null;
    }
    return rows.first['value'] as String?;
  }

  /// Salva [value] associado a [key], substituindo o valor anterior.
  Future<void> write(String key, String value) async {
    final db = await _database.database;
    await db.insert(
      DbTables.syncMetadata,
      {'key': key, 'value': value},
      conflictAlgorithm: ConflictAlgorithm.replace,
    );
  }
}
