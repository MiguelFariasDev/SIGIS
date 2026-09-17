import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:sigis_mobile/core/network/connectivity_service.dart';
import 'package:sigis_mobile/core/providers/core_providers.dart';
import 'package:sigis_mobile/data/local/database.dart';
import 'package:sigis_mobile/data/models/enums.dart';
import 'package:sigis_mobile/data/models/professional.dart';
import 'package:sigis_mobile/data/models/queue_entry.dart';
import 'package:sigis_mobile/features/auth/providers.dart';
import 'package:sigis_mobile/features/home/presentation/home_controller.dart';
import 'package:sigis_mobile/features/home/presentation/home_page.dart';
import 'package:sigis_mobile/features/home/providers.dart';
import 'package:sqflite_common_ffi/sqflite_ffi.dart';

class _FakeConnectivityService extends ConnectivityService {
  @override
  Future<bool> isOnline() async => true;

  @override
  Stream<bool> get onStatusChange => const Stream.empty();
}

class _FakeCurrentUserNotifier extends CurrentUserNotifier {
  @override
  Future<Professional?> build() async => const Professional(
        id: 'prof-1',
        name: 'Ana Souza',
        email: 'ana@sigis.gov.br',
        unitId: 'unit-1',
        unitAcronym: 'NASF',
        role: ProfessionalRole.professional,
      );
}

class _FakeHomeController extends HomeController {
  @override
  Future<List<QueueEntry>> build() async => [
        QueueEntry(
          id: 'queue-1',
          patientId: 'patient-1',
          patientName: 'Joao Lima',
          patientBirthDate: DateTime(2016, 3, 1),
          unitId: 'unit-1',
          specialty: 'Psicologia',
          priority: QueuePriority.urgent,
          enteredAt: DateTime.now().subtract(const Duration(minutes: 20)),
          status: QueueStatus.waiting,
        ),
      ];
}

void main() {
  setUpAll(() {
    sqfliteFfiInit();
  });

  testWidgets('HomePage mostra a lista de pacientes da fila do dia', (
    tester,
  ) async {
    await tester.pumpWidget(
      ProviderScope(
        overrides: [
          databaseProvider.overrideWithValue(
            AppDatabase(
              databaseFactory: databaseFactoryFfi,
              databasePath: inMemoryDatabasePath,
            ),
          ),
          connectivityServiceProvider.overrideWithValue(
            _FakeConnectivityService(),
          ),
          currentUserProvider.overrideWith(_FakeCurrentUserNotifier.new),
          homeControllerProvider.overrideWith(_FakeHomeController.new),
        ],
        child: const MaterialApp(home: HomePage()),
      ),
    );

    await tester.pump();

    expect(find.text('Ola, Ana Souza'), findsOneWidget);
    expect(find.text('Joao Lima'), findsOneWidget);
    expect(find.widgetWithText(OutlinedButton, 'Check-in'), findsOneWidget);
  });
}
