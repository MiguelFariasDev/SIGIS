import 'package:dio/dio.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:sigis_mobile/core/network/connectivity_service.dart';
import 'package:sigis_mobile/core/result/result.dart';
import 'package:sigis_mobile/data/local/dao/cached_patient_dao.dart';
import 'package:sigis_mobile/data/local/database.dart';
import 'package:sigis_mobile/data/remote/patient_api.dart';
import 'package:sigis_mobile/data/repositories/patient_repository.dart';
import 'package:sqflite_common_ffi/sqflite_ffi.dart';

class _FakeConnectivityService extends ConnectivityService {
  _FakeConnectivityService({this.online = true});

  bool online;

  @override
  Future<bool> isOnline() async => online;

  @override
  Stream<bool> get onStatusChange => const Stream.empty();
}

class _FakePatientApi extends PatientApi {
  _FakePatientApi({this.searchBehavior}) : super(Dio());

  Future<List<dynamic>> Function(String term)? searchBehavior;

  @override
  Future<List<dynamic>> search(String term) {
    return searchBehavior?.call(term) ?? Future.value(const []);
  }
}

Map<String, dynamic> _pessoaJson({
  required String id,
  required String nome,
}) =>
    {
      'id': id,
      'nomeCompleto': nome,
      'dataNascimento': '2015-05-10T00:00:00.000',
      'cns': '123456789012345',
    };

void main() {
  setUpAll(() {
    sqfliteFfiInit();
  });

  late CachedPatientDao cachedPatientDao;

  setUp(() async {
    final database = AppDatabase(
      databaseFactory: databaseFactoryFfi,
      databasePath: inMemoryDatabasePath,
    );
    // sqflite mantem um cache de conexoes por path; como todos os testes
    // usam o mesmo `inMemoryDatabasePath`, a conexao (e os dados) e
    // reaproveitada entre testes. Limpa explicitamente para isolar cada
    // teste.
    await database.wipeAllData();
    cachedPatientDao = CachedPatientDao(database);
  });

  test('retorna erro quando o termo de busca e muito curto', () async {
    final repository = PatientRepository(
      api: _FakePatientApi(),
      cachedPatientDao: cachedPatientDao,
      connectivity: _FakeConnectivityService(online: true),
    );

    final result = await repository.search('a');

    expect(result, isA<Failure<List<dynamic>>>());
  });

  test('busca online retorna resultados da API e atualiza o cache', () async {
    final repository = PatientRepository(
      api: _FakePatientApi(
        searchBehavior: (term) async => [
          _pessoaJson(id: 'p1', nome: 'Ana Souza'),
        ],
      ),
      cachedPatientDao: cachedPatientDao,
      connectivity: _FakeConnectivityService(online: true),
    );

    final result = await repository.search('Ana');

    expect(result, isA<Success<dynamic>>());
    final patients = (result as Success).value;
    expect(patients, hasLength(1));
    expect(patients.first.fullName, 'Ana Souza');

    final cached = await cachedPatientDao.search('Ana');
    expect(cached, hasLength(1));
  });

  test('busca offline usa o cache local previamente salvo', () async {
    final onlineRepository = PatientRepository(
      api: _FakePatientApi(
        searchBehavior: (term) async => [
          _pessoaJson(id: 'p2', nome: 'Joao Lima'),
        ],
      ),
      cachedPatientDao: cachedPatientDao,
      connectivity: _FakeConnectivityService(online: true),
    );
    await onlineRepository.search('Joao');

    final offlineRepository = PatientRepository(
      api: _FakePatientApi(),
      cachedPatientDao: cachedPatientDao,
      connectivity: _FakeConnectivityService(online: false),
    );

    final result = await offlineRepository.search('Joao');

    expect(result, isA<Success<dynamic>>());
    final patients = (result as Success).value;
    expect(patients.first.fullName, 'Joao Lima');
  });

  test('busca offline sem cache retorna paciente nao encontrado', () async {
    final repository = PatientRepository(
      api: _FakePatientApi(),
      cachedPatientDao: cachedPatientDao,
      connectivity: _FakeConnectivityService(online: false),
    );

    final result = await repository.search('Inexistente');

    expect(result, isA<Failure<dynamic>>());
  });
}
