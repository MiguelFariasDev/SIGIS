import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:sigis_mobile/core/network/api_client.dart';
import 'package:sigis_mobile/core/network/connectivity_service.dart';
import 'package:sigis_mobile/core/storage/preferences_service.dart';
import 'package:sigis_mobile/core/storage/secure_storage_service.dart';
import 'package:sigis_mobile/data/local/dao/cached_patient_dao.dart';
import 'package:sigis_mobile/data/local/dao/pending_operation_dao.dart';
import 'package:sigis_mobile/data/local/dao/sync_metadata_dao.dart';
import 'package:sigis_mobile/data/local/database.dart';
import 'package:sigis_mobile/data/remote/attendance_api.dart';
import 'package:sigis_mobile/data/remote/auth_api.dart';
import 'package:sigis_mobile/data/remote/patient_api.dart';
import 'package:sigis_mobile/data/repositories/attendance_repository.dart';
import 'package:sigis_mobile/data/repositories/auth_repository.dart';
import 'package:sigis_mobile/data/repositories/patient_repository.dart';
import 'package:sigis_mobile/features/auth/providers.dart';
import 'package:sigis_mobile/sync/sync_queue.dart';
import 'package:sigis_mobile/sync/sync_service.dart';

/// Provider do armazenamento seguro (token JWT).
final secureStorageServiceProvider = Provider<SecureStorageService>(
  (ref) => SecureStorageService(),
);

/// Provider das preferencias locais nao sensiveis.
final preferencesServiceProvider = Provider<PreferencesService>(
  (ref) => const PreferencesService(),
);

/// Provider do cliente Dio configurado (interceptors de auth/log/retry).
///
/// `onUnauthorized` usa `ref.read` (nao `ref.watch`) de proposito: so
/// precisa disparar a limpeza da sessao no momento de um 401, sem criar
/// dependencia reativa entre este provider e `currentUserProvider`.
final apiClientProvider = Provider<ApiClient>(
  (ref) => ApiClient(
    secureStorage: ref.watch(secureStorageServiceProvider),
    onUnauthorized: () => ref.read(currentUserProvider.notifier).clear(),
  ),
);

/// Provider do servico de deteccao de conectividade.
final connectivityServiceProvider = Provider<ConnectivityService>(
  (ref) => ConnectivityService(),
);

/// Provider do status de conectividade atual, como stream reativa.
final connectivityStatusProvider = StreamProvider<bool>(
  (ref) => ref.watch(connectivityServiceProvider).onStatusChange,
);

/// Provider do banco de dados local SQLite (singleton por sessao do
/// app).
final databaseProvider = Provider<AppDatabase>((ref) => AppDatabase());

/// Provider do DAO de operacoes pendentes.
final pendingOperationDaoProvider = Provider<PendingOperationDao>(
  (ref) => PendingOperationDao(ref.watch(databaseProvider)),
);

/// Provider do DAO de pacientes em cache.
final cachedPatientDaoProvider = Provider<CachedPatientDao>(
  (ref) => CachedPatientDao(ref.watch(databaseProvider)),
);

/// Provider do DAO de metadados de sincronizacao.
final syncMetadataDaoProvider = Provider<SyncMetadataDao>(
  (ref) => SyncMetadataDao(ref.watch(databaseProvider)),
);

/// Provider da fila local de operacoes pendentes.
final syncQueueProvider = Provider<SyncQueue>(
  (ref) => SyncQueue(ref.watch(pendingOperationDaoProvider)),
);

/// Provider do cliente HTTP de autenticacao.
final authApiProvider = Provider<AuthApi>(
  (ref) => AuthApi(ref.watch(apiClientProvider).dio),
);

/// Provider do cliente HTTP de pessoas.
final patientApiProvider = Provider<PatientApi>(
  (ref) => PatientApi(ref.watch(apiClientProvider).dio),
);

/// Provider do cliente HTTP de fila e atendimentos.
final attendanceApiProvider = Provider<AttendanceApi>(
  (ref) => AttendanceApi(ref.watch(apiClientProvider).dio),
);

/// Provider do servico de sincronizacao (singleton, mantem a assinatura
/// de conectividade viva durante toda a sessao do app).
final syncServiceProvider = Provider<SyncService>((ref) {
  final service = SyncService(
    api: ref.watch(attendanceApiProvider),
    syncQueue: ref.watch(syncQueueProvider),
    connectivity: ref.watch(connectivityServiceProvider),
    syncMetadataDao: ref.watch(syncMetadataDaoProvider),
  );
  ref.onDispose(service.dispose);
  return service;
});

/// Provider da contagem de operacoes pendentes de sincronizacao, usado
/// pelo badge do [SyncIndicator] no AppBar.
final pendingSyncCountProvider = StreamProvider<int>(
  (ref) => ref.watch(syncServiceProvider).pendingCountStream,
);

/// Provider do repositorio de autenticacao.
final authRepositoryProvider = Provider<AuthRepository>(
  (ref) => AuthRepository(
    authApi: ref.watch(authApiProvider),
    secureStorage: ref.watch(secureStorageServiceProvider),
    preferences: ref.watch(preferencesServiceProvider),
    database: ref.watch(databaseProvider),
  ),
);

/// Provider do repositorio de pacientes.
final patientRepositoryProvider = Provider<PatientRepository>(
  (ref) => PatientRepository(
    api: ref.watch(patientApiProvider),
    cachedPatientDao: ref.watch(cachedPatientDaoProvider),
    connectivity: ref.watch(connectivityServiceProvider),
  ),
);

/// Provider do repositorio de fila e atendimentos.
final attendanceRepositoryProvider = Provider<AttendanceRepository>(
  (ref) => AttendanceRepository(
    api: ref.watch(attendanceApiProvider),
    syncQueue: ref.watch(syncQueueProvider),
    syncService: ref.watch(syncServiceProvider),
  ),
);
