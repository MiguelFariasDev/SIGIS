import 'package:sqflite/sqflite.dart';
import 'package:sigis_mobile/data/local/database.dart';
import 'package:sigis_mobile/data/local/models/cached_patient.dart';

/// Acesso a tabela `cached_patients` do SQLite local.
///
/// Guarda apenas os campos minimos necessarios para busca offline
/// (regra de minimizacao de dados da LGPD) — nunca prontuario ou
/// historico.
class CachedPatientDao {
  /// Cria o DAO sobre o banco [database].
  CachedPatientDao(this._database);

  final AppDatabase _database;

  /// Insere ou substitui um paciente em cache.
  Future<void> upsert(CachedPatient patient) async {
    final db = await _database.database;
    await db.insert(
      DbTables.cachedPatients,
      _toRow(patient),
      conflictAlgorithm: ConflictAlgorithm.replace,
    );
  }

  /// Insere ou substitui varios pacientes em cache de uma vez.
  Future<void> upsertAll(List<CachedPatient> patients) async {
    final db = await _database.database;
    final batch = db.batch();
    for (final patient in patients) {
      batch.insert(
        DbTables.cachedPatients,
        _toRow(patient),
        conflictAlgorithm: ConflictAlgorithm.replace,
      );
    }
    await batch.commit(noResult: true);
  }

  /// Busca pacientes em cache cujo nome ou CNS contenha [term].
  Future<List<CachedPatient>> search(String term) async {
    final db = await _database.database;
    final likeTerm = '%$term%';
    final rows = await db.query(
      DbTables.cachedPatients,
      where: 'name LIKE ? OR cns LIKE ?',
      whereArgs: [likeTerm, likeTerm],
      orderBy: 'name ASC',
    );
    return rows.map(_fromRow).toList();
  }

  Map<String, Object?> _toRow(CachedPatient patient) => {
        'id': patient.id,
        'name': patient.name,
        'birth_date': patient.birthDate.toIso8601String(),
        'cns': patient.cns,
        'unit_id': patient.unitId,
        'cached_at': patient.cachedAt.millisecondsSinceEpoch,
      };

  CachedPatient _fromRow(Map<String, Object?> row) => CachedPatient(
        id: row['id']! as String,
        name: row['name']! as String,
        birthDate: DateTime.parse(row['birth_date']! as String),
        cns: row['cns'] as String?,
        unitId: row['unit_id'] as String?,
        cachedAt: DateTime.fromMillisecondsSinceEpoch(
          row['cached_at']! as int,
        ),
      );
}
