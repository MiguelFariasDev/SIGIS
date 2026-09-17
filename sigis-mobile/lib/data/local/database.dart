import 'package:path/path.dart' as p;
import 'package:path_provider/path_provider.dart';
import 'package:sqflite/sqflite.dart';

/// Nomes das tabelas do banco local SQLite.
abstract final class DbTables {
  /// Fila de operacoes de escrita pendentes de sincronizacao.
  static const String pendingOperations = 'pending_operations';

  /// Cache minimizado de pessoas, para busca offline (LGPD: sem
  /// prontuario).
  static const String cachedPatients = 'cached_patients';

  /// Metadados de sincronizacao (ex.: instante da ultima sync).
  static const String syncMetadata = 'sync_metadata';
}

/// Gerencia a abertura e criacao do schema do banco local SQLite.
///
/// As tres tabelas seguem a regra offline-first e de minimizacao de
/// dados (LGPD) descritas no README do projeto: nenhuma delas guarda
/// prontuario ou historico completo do paciente.
class AppDatabase {
  /// Cria o banco local. Os parametros [databaseFactory] e
  /// [databasePath] existem para permitir a injecao de um banco em
  /// memoria (`sqflite_common_ffi`) nos testes — em producao, ambos sao
  /// omitidos e o app usa o diretorio de documentos do dispositivo.
  AppDatabase({DatabaseFactory? databaseFactory, String? databasePath})
      : _databaseFactory = databaseFactory,
        _databasePath = databasePath;

  final DatabaseFactory? _databaseFactory;
  final String? _databasePath;

  Database? _database;

  /// Retorna a instancia aberta do banco, abrindo-a (e criando o
  /// schema, se necessario) na primeira chamada.
  Future<Database> get database async {
    _database ??= await _open();
    return _database!;
  }

  Future<Database> _open() async {
    final path = _databasePath ?? await _defaultPath();
    final options = OpenDatabaseOptions(
      version: 1,
      onCreate: (db, version) async {
        await db.execute('''
          CREATE TABLE ${DbTables.pendingOperations} (
            id TEXT PRIMARY KEY,
            operation_type TEXT NOT NULL,
            payload TEXT NOT NULL,
            created_at INTEGER NOT NULL,
            retry_count INTEGER NOT NULL DEFAULT 0,
            status TEXT NOT NULL,
            error_message TEXT,
            server_id TEXT
          )
        ''');

        await db.execute('''
          CREATE TABLE ${DbTables.cachedPatients} (
            id TEXT PRIMARY KEY,
            name TEXT NOT NULL,
            birth_date TEXT NOT NULL,
            cns TEXT,
            unit_id TEXT,
            cached_at INTEGER NOT NULL
          )
        ''');

        await db.execute('''
          CREATE TABLE ${DbTables.syncMetadata} (
            key TEXT PRIMARY KEY,
            value TEXT NOT NULL
          )
        ''');
      },
    );

    if (_databaseFactory != null) {
      return _databaseFactory.openDatabase(path, options: options);
    }
    return openDatabase(path, version: options.version, onCreate: options.onCreate);
  }

  Future<String> _defaultPath() async {
    final directory = await getApplicationDocumentsDirectory();
    return p.join(directory.path, 'sigis_mobile.db');
  }

  /// Fecha a conexao com o banco e apaga todos os dados locais.
  ///
  /// Usado no logout para cumprir a regra de LGPD de nao reter dados
  /// apos o encerramento da sessao.
  Future<void> wipeAllData() async {
    final db = await database;
    await db.delete(DbTables.pendingOperations);
    await db.delete(DbTables.cachedPatients);
    await db.delete(DbTables.syncMetadata);
  }
}
