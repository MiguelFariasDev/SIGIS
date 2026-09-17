import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import 'package:sigis_mobile/core/errors/app_error.dart';
import 'package:sigis_mobile/core/theme/app_colors.dart';
import 'package:sigis_mobile/data/models/enums.dart';
import 'package:sigis_mobile/features/session/providers.dart';
import 'package:sigis_mobile/shared/extensions/context_extensions.dart';

/// Tela de registro de sessao rapida (F06).
///
/// Formulario simplificado — apenas os campos essenciais, nao a ficha
/// completa por servico (isso fica para o backend/web em fases
/// futuras).
class SessionPage extends ConsumerStatefulWidget {
  /// Cria a tela de sessao rapida para a pessoa [patientId].
  const SessionPage({super.key, required this.patientId});

  /// Identificador da pessoa atendida.
  final String patientId;

  @override
  ConsumerState<SessionPage> createState() => _SessionPageState();
}

class _SessionPageState extends ConsumerState<SessionPage> {
  final _formKey = GlobalKey<FormState>();
  final _objectiveController = TextEditingController();
  final _notesController = TextEditingController();

  SessionType _sessionType = SessionType.nasfRecord;
  bool _usesMedication = false;

  @override
  void dispose() {
    _objectiveController.dispose();
    _notesController.dispose();
    super.dispose();
  }

  Future<void> _submit() async {
    if (!_formKey.currentState!.validate()) {
      return;
    }

    await ref.read(sessionControllerProvider.notifier).save(
          patientId: widget.patientId,
          sessionType: _sessionType,
          objective: _objectiveController.text,
          notes: _notesController.text.trim().isEmpty
              ? null
              : _notesController.text,
          usesMedication: _usesMedication,
        );

    if (!mounted) {
      return;
    }

    final state = ref.read(sessionControllerProvider);
    if (state.hasError) {
      final error = state.error;
      final message = error is AppError ? error.message : 'Erro ao salvar.';
      context.showSnackBar(message, backgroundColor: AppColors.error);
      return;
    }

    context.showSnackBar('Sessao salva');
    if (context.canPop()) {
      context.pop();
    } else {
      context.go('/home');
    }
  }

  @override
  Widget build(BuildContext context) {
    final state = ref.watch(sessionControllerProvider);
    final isLoading = state.isLoading;

    return Scaffold(
      appBar: AppBar(title: const Text('Sessao rapida')),
      body: Form(
        key: _formKey,
        child: ListView(
          padding: const EdgeInsets.all(16),
          children: [
            DropdownButtonFormField<SessionType>(
              initialValue: _sessionType,
              decoration: const InputDecoration(labelText: 'Tipo de sessao'),
              items: SessionType.values
                  .map(
                    (type) => DropdownMenuItem(
                      value: type,
                      child: Text(type.label),
                    ),
                  )
                  .toList(),
              onChanged: (value) {
                if (value != null) {
                  setState(() => _sessionType = value);
                }
              },
            ),
            const SizedBox(height: 16),
            TextFormField(
              controller: _objectiveController,
              maxLines: 3,
              decoration: const InputDecoration(
                labelText: 'Objetivo da sessao',
                alignLabelWithHint: true,
              ),
              validator: (value) => (value == null || value.trim().isEmpty)
                  ? 'Informe o objetivo da sessao.'
                  : null,
            ),
            const SizedBox(height: 16),
            TextFormField(
              controller: _notesController,
              maxLines: 3,
              decoration: const InputDecoration(
                labelText: 'Observacoes (opcional)',
                alignLabelWithHint: true,
              ),
            ),
            SwitchListTile(
              value: _usesMedication,
              onChanged: (value) => setState(() => _usesMedication = value),
              contentPadding: EdgeInsets.zero,
              title: const Text('Uso de medicacao'),
            ),
            const SizedBox(height: 24),
            ElevatedButton(
              onPressed: isLoading ? null : _submit,
              child: isLoading
                  ? const SizedBox(
                      height: 20,
                      width: 20,
                      child: CircularProgressIndicator(
                        strokeWidth: 2,
                        color: Colors.white,
                      ),
                    )
                  : const Text('Salvar sessao'),
            ),
          ],
        ),
      ),
    );
  }
}
