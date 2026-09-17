import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:sigis_mobile/features/auth/presentation/login_page.dart';

void main() {
  testWidgets('LoginPage renderiza campos e botao de entrar', (tester) async {
    await tester.pumpWidget(
      const ProviderScope(
        child: MaterialApp(home: LoginPage()),
      ),
    );
    await tester.pump();

    expect(find.text('SIGIS'), findsOneWidget);
    expect(find.widgetWithText(TextFormField, 'Matricula ou e-mail'), findsOneWidget);
    expect(find.widgetWithText(TextFormField, 'Senha'), findsOneWidget);
    expect(find.widgetWithText(ElevatedButton, 'Entrar'), findsOneWidget);
    expect(find.text('Lembrar-me'), findsOneWidget);
  });
}
