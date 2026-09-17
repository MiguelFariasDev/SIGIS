import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:sigis_mobile/features/checkin/presentation/checkin_page.dart';

void main() {
  testWidgets('CheckInPage mostra botoes de confirmacao', (tester) async {
    await tester.pumpWidget(
      const ProviderScope(
        child: MaterialApp(
          home: CheckInPage(
            attendanceId: 'attendance-1',
            args: CheckInArgs(patientName: 'Ana Souza', details: 'NASF'),
          ),
        ),
      ),
    );
    await tester.pump();

    expect(find.text('Ana Souza'), findsOneWidget);
    expect(find.widgetWithText(ElevatedButton, 'Compareceu'), findsOneWidget);
    expect(find.widgetWithText(OutlinedButton, 'Faltou'), findsOneWidget);
  });

  testWidgets('exige confirmacao dupla ao tocar em Faltou', (tester) async {
    await tester.pumpWidget(
      const ProviderScope(
        child: MaterialApp(
          home: CheckInPage(
            attendanceId: 'attendance-1',
            args: CheckInArgs(patientName: 'Ana Souza'),
          ),
        ),
      ),
    );
    await tester.pump();

    await tester.tap(find.widgetWithText(OutlinedButton, 'Faltou'));
    await tester.pumpAndSettle();

    expect(find.text('Confirmar falta'), findsWidgets);
    expect(find.text('Cancelar'), findsOneWidget);
  });
}
